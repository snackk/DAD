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
        /*System.Xml.Serialization.XmlSerializer serializer =
new System.Xml.Serialization.XmlSerializer(typeof());  //HELP Is this to use + ??? Or NodeOperator should not be serialized???*/

        private List<NodeOperator.NodeOperator> nodeOperators { get; set; } = new List<NodeOperator.NodeOperator>();
        private List<Thread> nodeThreads { get; set; } = new List<Thread>();

        public void start(int operator_id,string operation, int operatorPort)/*If it receives the method to call it would be much simpler*/
        {
            NodeOperator.NodeOperator node = new NodeOperator.NodeOperator(operator_id, operatorPort);    /*Probably not going to need to manage NODES*/
            nodeOperators.Add(node);
            Thread t1= null;

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
            }
            nodeThreads.Add(t1);
            
            t1.Start();
        }

        public void crash(string processname)
        {
            throw new NotImplementedException();
        }

        public void freeze(string processname)
        {
            throw new NotImplementedException();
        }

        public void interval(int operator_id, int x_ms)
        {
            throw new NotImplementedException();
        }

        public string status()
        {
            string status = "";
            foreach (NodeOperator.NodeOperator op in nodeOperators) {
                status+=op.status + System.Environment.NewLine;
                
            }
            return status;
        }

        public void unfreeze(string processname)
        {
            throw new NotImplementedException();
        }
    }
}
