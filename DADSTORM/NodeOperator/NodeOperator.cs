using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging;
using DADStorm.DataTypes;
using System.Linq;

namespace NodeOperator
{
    public class NodeOperator : MarshalByRefObject, INodeOperator
    {
        private int portN { set; get; }
        private NodeOperatorData nodeData;
        private List<INodeOperator> replicas { get; set; } = new List<INodeOperator>();

        private List<DADTuple> InputTuples = new List<DADTuple>();
        private List<DADTuple> OutputTuples = new List<DADTuple>();
        private List<DADTuple> nodeTuples { get; set; } = new List<DADTuple>();

        private int replicateDepth = 0;

        public delegate bool nodeReplicationAsync(List<DADTuple> replicatedTuples);
        public delegate void doWork();
        public doWork nodeDoWork { get; set; } = null;

        public NodeOperator(int port, List<INodeOperator> ops) {
            portN = port;
            replicas = ops;
            OperatorType ot = nodeData.TypeofOperation;
            switch (nodeData.TypeofOperation) {

                case OperatorType.count:
                    nodeDoWork =  new doWork(countThread);
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

        public NodeOperator(NodeOperatorData node)
        {
            portN = node.ConnectionPort;
            nodeData = node;
        }

        public void replicationAndDownstreaming() {

            if (nodeData.TypeofRouting == RoutingType.primary || nodeData.TypeofRouting == RoutingType.random)
            {
                foreach (Downstream dw in nodeData.Downstream)
                {
                    downStreamOutput(dw);
                }
                siblingsReplication();
            }
        }

        public void uniqThread()
        {
            bool isccurrentUnique = true;
           

            for (int i = 0; i < nodeTuples.Count; i++)
            {
                isccurrentUnique = true;
                for (int j = i + 1; j < nodeTuples.Count; j++)
                {
                    if (nodeTuples[i].Equals(nodeTuples[j]))
                    {
                        isccurrentUnique = false;
                        break;
                    }
                }
                if (isccurrentUnique)
                    OutputTuples.Add(nodeTuples[i]);

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
            List<string> operargs = new List<string>();
            int index = Convert.ToInt32(operargs[0]);
            OperationSymbol oper = StringToOperation(operargs[1]);
            string value = operargs[2];

            OutputTuples = InputTuples.Where(i =>
            {
                switch (oper)
                {
                    case OperationSymbol.equals:
                        return i.getIndex(index).Equals(value);
                    case OperationSymbol.lesser:
                        return i.getIndex(index).CompareTo(value) < 0;
                    case OperationSymbol.greater:
                        return i.getIndex(index).CompareTo(value) > 0;
                    default: return false;
                }
            }).ToList();

            replicationAndDownstreaming();
        }

        public void customThread()
        {


            
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
            nodeTuples.AddRange(replicatedTuples);
            replicateDepth++;
            /*if(replicateDepth == LISTA_NEVES.length)*/
            return true;
        }
    }
}
