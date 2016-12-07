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
                            WellKnownObjectMode.Singleton);/*singlecall*/
                    System.Console.WriteLine("press <enter> to terminate pcs...");
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

        int port = 10010;//debug

        public delegate int RemoteAsyncDelegate(int t);

        public string start(string operatorID)//DEBUG
        {
            port++;
            if (nodeThreads.ContainsKey(operatorID)) {
                return "node " + operatorID + " already exists!";
            }
            NodeOperator.NodeOperator node = new NodeOperator.NodeOperator(operatorID, port, null);   

            Thread t1 = new Thread(new ThreadStart(node.nodeCommunication));
            t1.Start();
            //t1.IsBackground = true;
            t1.Join();
            nodeThreads.Add(operatorID, t1);    /*operatorID should be the machine IP*/

            INodeOperator nodeOp = (INodeOperator)Activator.GetObject(typeof(INodeOperator),
                "tcp://localhost:" + port + "/Op"+port);

            nodeOperators.Add(operatorID, nodeOp);  /*operatorID should be the machine IP*/

            AsyncCallback asyncCallback = new AsyncCallback(this.CallBack);
            RemoteAsyncDelegate remoteDel = new RemoteAsyncDelegate(nodeOp.replicate);
            IAsyncResult ar = remoteDel.BeginInvoke(port,
                                        asyncCallback, null);

            return "node " + operatorID + " is up and running.";

            /*
            switch (operation)
            {
                case "UNIQ":
                    t1 = new Thread(new ThreadStart(node.uniqThread));
                    break;
                case "COUNT":
                    t1 = new Thread(new ThreadStart(node.countThread));
                    break;
                case "DUP":
                    t1 = new Thread(new ThreadStart(node.dupThread));
                    break;
                case "FILTER":
                    t1 = new Thread(new ThreadStart(node.filterThread));
                    break;
                case "CUSTOM":
                    t1 = new Thread(new ThreadStart(node.customThread));
                    break;
            }*/
        }

        public void CallBack(IAsyncResult ar)
        {
            int p = 0;
            RemoteAsyncDelegate rad = (RemoteAsyncDelegate)((AsyncResult)ar).AsyncDelegate;
            p = (int)rad.EndInvoke(ar);
            System.Console.WriteLine("it has returned " + p + " !");
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
