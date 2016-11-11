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

    public class NodeManagerService : MarshalByRefObject, NodeManager.NodeManager
    {
        private Dictionary<int, NodeOperator.NodeOperator> nodeOperators { get; set; } = new Dictionary<int, NodeOperator.NodeOperator>();
        private Dictionary<int, Thread> nodeThreads { get; set; } = new Dictionary<int, Thread>();

        public void start(int operatorID,string operation, int operatorPort)
        {
            NodeOperator.NodeOperator node = new NodeOperator.NodeOperator(operatorID, operatorPort);   
            nodeOperators.Add(operatorID, node);

            Thread t1 = new Thread(new ThreadStart(node.debug));
            //t1.IsBackground = true;
            t1.Start();
            nodeThreads.Add(operatorID, t1);

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

        public void crash(int operatorID)
        {
            if (nodeThreads[operatorID].IsAlive)
                nodeThreads[operatorID].Abort();
        }

        public void freeze(int operatorID)
        {

        }

        public void interval(int operatorID, int x_ms)
        {
            //if (nodeThreads[operatorID].IsAlive)
                //nodeThreads[operatorID].Sleep(x_ms);
        }

        public string status(int operatorID)
        {
            return "Node nº " + operatorID + " is " + nodeThreads[operatorID].ThreadState;
        }

        public void unfreeze(int operatorID)
        {
            throw new NotImplementedException();
        }
    }
}
