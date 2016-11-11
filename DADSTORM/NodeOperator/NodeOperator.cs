using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace NodeOperator
{
    public class NodeOperator : MarshalByRefObject
    {
        private int nodeN { set; get; }
        private int portN { set; get; }

        public NodeOperator(int operator_id, int port) {
                nodeN = operator_id;
                portN = port;
            }

        public void nodeCommunication() {
            TcpChannel channel = new TcpChannel(portN);
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(NodeOperator),
                "NodeOperatorCommunication",
                WellKnownObjectMode.SingleCall);/*Singlecall or singleton?*/
        }

        /*THIS IS FOR PURE DEBUGGING ON THE FIRST STAGE!*/
        public void debug() {   
            while (true) { }
        }

        public void uniqThread() { }
        public void countThread() { }
        public void dupThread() { }
        public void filterThread() { }
        public void customThread() { }

    }
}
