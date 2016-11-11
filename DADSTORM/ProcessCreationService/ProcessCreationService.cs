using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

namespace ProcessCreationService
{
    public class ProcessCreationService
    {
        static void Main(string[] args)
        {
            
            TcpChannel channel = new TcpChannel(10000);
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(NodeManagerService),
                "NodeManagerService",
                WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Press <enter> to terminate PCS...");
            System.Console.ReadLine();
        }
    }

    public class NodeManagerService : MarshalByRefObject, NodeManager.INodeManager
    {
        private Dictionary<int, NodeOperator.NodeOperator> nodeOperators { get; set; } = new Dictionary<int, NodeOperator.NodeOperator>();
        private Dictionary<int, Thread> nodeThreads { get; set; } = new Dictionary<int, Thread>();

        public string start(int operatorID)//,string operation, int operatorPort)
        {
            if (nodeThreads.ContainsKey(operatorID)) {
                return "node " + operatorID + " already exists!";
            }
            NodeOperator.NodeOperator node = new NodeOperator.NodeOperator(operatorID, 0);   
            nodeOperators.Add(operatorID, node);

            Thread t1 = new Thread(new ThreadStart(node.debug));
            //t1.IsBackground = true;
            t1.Start();
            nodeThreads.Add(operatorID, t1);
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

        public string crash(int operatorID)
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

        public string freeze(int operatorID)
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

        public string interval(int operatorID, int x_ms) 
        {
            return "";
        }

        public string status(int operatorID)
        {
            try
            {
                return "node " + operatorID + " is " + nodeThreads[operatorID].ThreadState;
            }
            catch (KeyNotFoundException) {
                return "node " + operatorID + " does not exist!";
            }
        }

        public string unfreeze(int operatorID)
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
