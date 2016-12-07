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
        private string nodeName { set; get; }
        private int portN { set; get; }
        public int digme = 0;
        private List<INodeOperator> replicas { get; set; } = new List<INodeOperator>();


        public delegate int RemoteAsyncDelegate(int t);

        /*Node_Name -> used to create nodeCommunication*/
        public NodeOperator(string operator_id, int port, List<INodeOperator> ops) {
            nodeName = operator_id;
            portN = port;
            replicas = ops;
            }

        public void uniqThread()
        {
            throw new NotImplementedException();
        }

        List<DADTuple> Input = new List<DADTuple>();
        public void countThread()
        {
            //HACK for templating:
            var output = Input.Count;
            throw new NotImplementedException();
        }

        public void dupThread()
        {
            var output = Input;
            throw new NotImplementedException();
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
            throw new NotImplementedException();
            List<string> operargs = new List<string>();
            int index = Convert.ToInt32(operargs[0]);
            OperationSymbol oper = StringToOperation(operargs[1]);
            string value = operargs[2];

            Input.Where(i =>
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
            });
        }

        public void customThread()
        {
            throw new NotImplementedException();
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
            RemotingServices.Marshal(this, "Op" + portN);
        }

        private void replicationConnection() {
            foreach (INodeOperator no in replicas) {
                AsyncCallback asyncCallback = new AsyncCallback(this.CallBack);
                RemoteAsyncDelegate remoteDel = new RemoteAsyncDelegate(no.replicate);
                IAsyncResult ar = remoteDel.BeginInvoke(10,
                                            asyncCallback, null);
            }
        }

        public void CallBack(IAsyncResult ar)
        {
            int p = 0;
            RemoteAsyncDelegate rad = (RemoteAsyncDelegate)((AsyncResult)ar).AsyncDelegate;
            p = (int)rad.EndInvoke(ar);
            System.Console.WriteLine("it has propagated to " + p + " nodes.");
        }

        public int replicate(int digger)
        {
            digme = digger;
            return digme;
        }
    }
}
