using System;

namespace NodeOperator
{
    public class NodeOperator : MarshalByRefObject
    {
        public int nodeN { private set; get; }
        public int portN { private set; get; }

    public NodeOperator(int operator_id, int port) {
            nodeN = operator_id;
            portN = port;
        }

        public void debug() { }
        public void uniqThread() { }
        public void countThread() { }
        public void dupThread() { }
        public void filterThread() { }
        public void customThread() { }
    }
}
