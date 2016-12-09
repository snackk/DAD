using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging;
using DADStorm.DataTypes;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NodeOperator
{
    public class NodeOperator : MarshalByRefObject, INodeOperator
    {
        private int portN { set; get; }
        private NodeOperatorData nodeData;
        
        private List<DADTuple> OutputTuples = new List<DADTuple>();
        private List<DADTuple> InputTuples { get; set; } = new List<DADTuple>();

        private int replicateDepth = 0;

        public delegate bool nodeReplicationAsync(List<DADTuple> replicatedTuples);
        public delegate void doWork();
        public doWork nodeDoWork { get; set; } = null;

        public NodeOperator(NodeOperatorData node) {
            portN = node.ConnectionPort;
            nodeData = node;
            InputTuples = node.Initialtuples;

            OperatorType ot = nodeData.TypeofOperation;
            switch (nodeData.TypeofOperation) {

                case OperatorType.count:
                    nodeDoWork = new doWork(countThread);
                    break;
                case OperatorType.custom:
                    nodeDoWork = new doWork(customThread);
                    break;
                case OperatorType.dup:
                    nodeDoWork = new doWork(dupThread);
                    break;
                case OperatorType.filter:
                    nodeDoWork = new doWork(filterThread);
                    break;
                case OperatorType.uniq:
                    nodeDoWork = new doWork(uniqThread);
                    break;
            }
        }

        public void makeNodeWork() {
            nodeDoWork.BeginInvoke(null,null);
        }

        public void replicationAndDownstreaming() {

            if (nodeData.TypeofRouting == RoutingType.primary || nodeData.TypeofRouting == RoutingType.random)
            {
                foreach (Downstream dw in nodeData.Downstream)
                {
                    downStreamOutput(dw);
                }
            }
            siblingsReplication();
        }

        public void uniqThread()
        {
            bool isccurrentUnique = true;
           

            for (int i = 0; i < InputTuples.Count; i++)
            {
                isccurrentUnique = true;
                for (int j = i + 1; j < InputTuples.Count; j++)
                {
                    if (InputTuples[i].Equals(InputTuples[j]))
                    {
                        isccurrentUnique = false;
                        break;
                    }
                }
                if (isccurrentUnique)
                    OutputTuples.Add(InputTuples[i]);

            }
            replicationAndDownstreaming();
        }

        

        public void countThread()
        {
           
            OutputTuples[0] = new DADTuple(InputTuples.Count.ToString());
            replicationAndDownstreaming();
            
        }

        public void dupThread()
        {
            var output = InputTuples;
            OutputTuples[0] = new DADTuple(output.ToString());
            replicationAndDownstreaming();
           
        }

        private enum OperationSymbol
        {
            lesser,
            greater,
            equals
        }
        private OperationSymbol StringToOperation(string input)
        {
            switch (input)
            {
                case "=": return OperationSymbol.equals;
                case "<": return OperationSymbol.lesser;
                case ">": return OperationSymbol.greater;
                default: throw new InvalidOperationException();
            }
        }

        public void filterThread()
        {
            int index = Convert.ToInt32(nodeData.OperationArgs[0]);
            OperationSymbol oper = StringToOperation(nodeData.OperationArgs[1]);
            string value = nodeData.OperationArgs[2];
            OutputTuples = InputTuples.Where(i =>
            {
                switch (oper)
                {
                    case OperationSymbol.equals:
                        return i.getIndex(index-1).Equals(value);
                    case OperationSymbol.lesser:
                        return i.getIndex(index-1).CompareTo(value) < 0;
                    case OperationSymbol.greater:
                        return i.getIndex(index-1).CompareTo(value) > 0;
                    default: return false;
                }
            }).ToList();

            replicationAndDownstreaming();
        }

        public void customThread()
        {
            byte[] code = Encoding.ASCII.GetBytes(nodeData.OperationArgs[0]);
            string className = nodeData.OperationArgs[1];

            Assembly assembly = Assembly.Load(code);
            // Walk through each type in the assembly looking for our class
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass == true)
                {
                    if (type.FullName.EndsWith("." + className))
                    {
                        // create an instance of the object
                        object ClassObj = Activator.CreateInstance(type);

                        // Dynamically Invoke the method

                        object[] args = new object[] { InputTuples.Select(i => i.Items) };
                        object resultObject = type.InvokeMember("CustomOperation",
                          BindingFlags.Default | BindingFlags.InvokeMethod,
                               null,
                               ClassObj,
                               args);
                        IList<IList<string>> result = (IList<IList<string>>)resultObject;
                        OutputTuples = new List<DADTuple>();
                        foreach(var res in result)
                        {
                            OutputTuples.Add(new DADTuple(res.ToArray()));
                        }
                        //Console.WriteLine("Map call result was: ");
                        //foreach (IList<string> tuple in result)
                        //{
                        //    Console.Write("tuple: ");
                        //    foreach (string s in tuple)
                        //        Console.Write(s + " ,");
                        //    Console.WriteLine();
                        //}
                        return;
                    }
                }
            }
            throw (new System.Exception("could not invoke method"));
        }

    



        public void debug() {
            while (true) { }
        }
        
        public void nodeCommunication()
        {
            IDictionary prop = new Hashtable();
            prop["name"] = "tcp" + portN;
            prop["port"] = portN;
            TcpChannel channel = new TcpChannel(prop, null, null);
            ChannelServices.RegisterChannel(channel, true);
            RemotingServices.Marshal(this, nodeData.OperatorName + nodeData.ConnectionPort); // "Op" + portN);
        }

        private void downStreamOutput(Downstream down) {
            switch (down.Routing) {
                case RoutingType.hashing:
                    break;
                case RoutingType.primary:
                    replicateTuplesToNode(down.TargetIPs.First()).makeNodeWork();/*TODO - make it work*/
                    break;
                case RoutingType.random:
                    Random rnd1 = new Random();
                    replicateTuplesToNode(down.TargetIPs[rnd1.Next(down.TargetIPs.Count)]).makeNodeWork();
                    break;
            }

        }

        private INodeOperator replicateTuplesToNode(string remoteLocation)
        {
            INodeOperator nodeOp = (INodeOperator)Activator.GetObject(typeof(INodeOperator), remoteLocation);
            AsyncCallback asyncCallback = new AsyncCallback(this.nodeReplicationCallBack);
            nodeReplicationAsync remoteDel = new nodeReplicationAsync(nodeOp.replicateTuples);
            IAsyncResult ar = remoteDel.BeginInvoke(OutputTuples,
                                        asyncCallback, null);
            return nodeOp;

        }

        private void siblingsReplication()
        {  
            foreach (string pass in nodeData.Siblings) {
                replicateTuplesToNode(pass);
            }

        }

        public void nodeReplicationCallBack(IAsyncResult ar)
        {
            bool p = false;
            nodeReplicationAsync rad = (nodeReplicationAsync)((AsyncResult)ar).AsyncDelegate;
            p = (bool)rad.EndInvoke(ar);

               
        }

        public bool replicateTuples(List<DADTuple> replicatedTuples) {
            InputTuples.AddRange(replicatedTuples);
            replicateDepth++;
            /*if(replicateDepth == LISTA_NEVES.length)*/
            return true;
        }
    }
}
