using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace NodeOperator
{
    public class NodeOperator : MarshalByRefObject
    {
        public int nodeN { private set; get; }
        public int portN { private set; get; }
        public string status = "";

        public NodeOperator(int operator_id, int port) {
            nodeN = operator_id;
            status = nodeN + " is running.";
            portN = port;
        }

        public void runServer()
        {
            /*ZONA EXCLUSIVA - nao garante exclusao mutua*/
            TcpChannel tcpChannel = new TcpChannel(portN);

            WellKnownServiceTypeEntry WKSTE =
               new WellKnownServiceTypeEntry(typeof(NodeOperator),
                                             "Op",
                                             WellKnownObjectMode.Singleton);
            RemotingConfiguration.RegisterWellKnownServiceType(WKSTE);

            RemotingConfiguration.ApplicationName = "Op" + portN;
        }

        public void threadRun() {
            status = "Node nº" + nodeN + " is running.";
            /*Falta executar os tuplos, seja la o que isso for.*/
        }

        public void uniqThread() { }
        public void countThread() { }
        public void dupThread() { }
        public void filterThread() { }
        public void customThread() { }
    }
}
