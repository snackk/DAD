using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace NodeOperator
{
    public class NodeOperator : MarshalByRefObject
    {
        public int nodeN { private set; get; }
        public bool procRunning { private set; get; }
        public string status;

        public NodeOperator(int operator_id) {
            nodeN = operator_id;
        }

        public string getStatus() {
            return "";
        }

        public void runServer() {
            TcpChannel channel = new TcpChannel(10000);
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(NodeOperator),
                "NodeOperator",
                WellKnownObjectMode.SingleCall);
        }

        public void tryme() {
            status = "Node nº" + nodeN + " is running.";
        }
    }
}
