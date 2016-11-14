using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace NodeOperator
{
    public class NodeOperator : MarshalByRefObject, INodeOperator
    {
        private string nodeName { set; get; }
        private int portN { set; get; }
        public int digme = 0;

        /*Node_Name -> used to create nodeCommunication*/
        public NodeOperator(string operator_id, int port) {
            nodeName = operator_id;
                portN = port;
            }

        public void uniqThread()
        {
            throw new NotImplementedException();
        }

        public void countThread()
        {
            throw new NotImplementedException();
        }

        public void dupThread()
        {
            throw new NotImplementedException();
        }

        public void filterThread()
        {
            throw new NotImplementedException();
        }

        public void customThread()
        {
            throw new NotImplementedException();
        }

        public void debug() {
            while (true) { }
        }

        public void nodeCommunication() /*The call should be assyncronosly!!!*/
        {
            
            IDictionary prop = new Hashtable();
            prop["name"] = "tcp" + portN;
            prop["port"] = portN;
            TcpChannel channel = new TcpChannel(prop, null, null);
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(INodeOperator),
                "Op",
                WellKnownObjectMode.Singleton);/*Singlecall or singleton?*/
        }

        public int replicate(int digger)
        {
            digme = digger;
            return digme;
        }
    }
}
