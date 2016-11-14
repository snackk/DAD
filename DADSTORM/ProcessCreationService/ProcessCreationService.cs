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
        private Dictionary<string, NodeOperator.NodeOperator> nodeOperators { get; set; } = new Dictionary<string, NodeOperator.NodeOperator>();
        private Dictionary<string, Thread> nodeThreads { get; set; } = new Dictionary<string, Thread>();

        public string start(string operatorID)//,string operation, int operatorPort)
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
