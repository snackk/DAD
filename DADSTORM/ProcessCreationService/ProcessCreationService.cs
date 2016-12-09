using System;
using NodeOperator;
using NodeManager;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Collections;
using System.Net.Sockets;
using System.Linq;
using DADStorm.DataTypes;

namespace ProcessCreationService
{
    public class ProcessCreationService
    {
        private int _portN { set; get; }
        public ProcessCreationService(int portN) {
            _portN = portN;
        }


        static void Main(string[] args)
        {
            int port;

            if (args.Length == 0) {
                port = 10000;
            }
            else
            {
                port = Convert.ToInt32(args[0]);
            }

            while (true)
            {

                try
                {
                    TcpChannel channel = new TcpChannel(port);
                    ChannelServices.RegisterChannel(channel, true);
                    RemotingConfiguration.RegisterWellKnownServiceType(
                        typeof(NodeManagerService),
                        "nodemanagerservice",
                            WellKnownObjectMode.Singleton);
                    System.Console.WriteLine("press <enter> to terminate pcs...");
                    System.Console.WriteLine("Port : " + port );
                    System.Console.ReadLine();
                    break;
                    
                }
                catch (SocketException) {
                    System.Console.WriteLine("O porto foi incrementado  ");
                    port++;
                }

            }
        }
    }




    public class NodeManagerService : MarshalByRefObject, INodeManager
    {
        private Dictionary<string, INodeOperator> nodeOperators { get; set; } = new Dictionary<string, INodeOperator>();
        private Dictionary<string, Thread> nodeThreads { get; set; } = new Dictionary<string, Thread>();
        private Dictionary<string, List<string>> OPtoNodes { get; set; } = new Dictionary<string, List<string>>();
        public bool IsStarted { get; private set; }

        int port = 10010;   //debug

        public delegate int RemoteAsyncDelegate(int t);

        public bool init(List<NodeOperatorData> nodesInformation)
        {
            if (!IsStarted)
            {
                foreach (var OpID in nodesInformation.GroupBy(group=>group.OperatorID).Select(group => group.First()))
                {
                    OPtoNodes.Add(OpID.OperatorID, new List<string>());
                }
                foreach (var node in nodesInformation)
                {
                    NodeOperator.NodeOperator newNode = new NodeOperator.NodeOperator(node);

                    Thread t1 = new Thread(new ThreadStart(newNode.nodeCommunication));
                    t1.Start();
                    //t1.IsBackground = true;
                    t1.Join();
                    var pass = "tcp://localhost:" + node.ConnectionPort + "/" + node.OperatorName + node.ConnectionPort;
                    nodeThreads.Add(pass, t1);    /*operatorID should be the machine IP*/

                    INodeOperator nodeOp = (INodeOperator)Activator.GetObject(typeof(INodeOperator), pass);
                    OPtoNodes[node.OperatorID].Add(pass);
                    nodeOperators.Add(pass, nodeOp);  /*operatorID should be the machine IP*/
                }
                IsStarted = true;
            }
            return IsStarted;
        }

        public void start(string v)
        {
            foreach(var node in OPtoNodes[v])
            {
                //INodeOperator nodeOp = nodeOperators[node];
                //nodeOp.makeNodeWork();
                //AsyncCallback asyncCallback = new AsyncCallback(testCallBack);
                //nodeReplicationAsync remoteDel = new nodeReplicationAsync(nodeOp.makeNodeWork);
                //IAsyncResult ar = remoteDel.BeginInvoke(null,
                //                            asyncCallback, null);

            }
        }

        public void testCallBack(IAsyncResult ar)
        {

        }
        public string crash(string operatorID)
        {
            try
            {
                nodeThreads[operatorID].Abort();
                return "node " + operatorID + " was crashed.";
            }
            catch (KeyNotFoundException)
            {
                return "node " + operatorID + " does not exist!";
            }
        }
        public string freeze(string operatorID)
        {
            try
            {
                if (nodeThreads[operatorID].IsAlive)
                {
                    nodeThreads[operatorID].Suspend();
                    return "node " + operatorID + " is now frozen.";
                }
                return "node " + operatorID + " was already frozen.";
            }
            catch (KeyNotFoundException)
            {
                return "node " + operatorID + " does not exist!";
            }
        }

        public string interval(string operatorID, int x_ms) 
        {   
            try
            {
                if (nodeThreads[operatorID].IsAlive)
                {
                    nodeThreads[operatorID].Suspend();

                    Thread.Sleep(x_ms);
                    nodeThreads[operatorID].Resume();
                }
                
            }
            catch (KeyNotFoundException)
            {
                return "node " + operatorID + " does not exist!";
            }
            return "node " + operatorID + " is frozen for " + x_ms + " seconds.";
        }

        public string status(string operatorID)
        {
            try
            {
                return "node " + operatorID + " is " + nodeThreads[operatorID].ThreadState;
            }
            catch (KeyNotFoundException) {
                return "node " + operatorID + " does not exist!";
            } 
        }

        public string unfreeze(string operatorID)
        {
            try
            {
                if (nodeThreads[operatorID].IsAlive)
                {
                    nodeThreads[operatorID].Resume();
                    return "node " + operatorID + " is resuming.";
                }
                return "node " + operatorID + " is not frozen.";
            }
            catch (KeyNotFoundException)
            {
                return "node " + operatorID + " does not exist!";
            }
        }
    }
}
