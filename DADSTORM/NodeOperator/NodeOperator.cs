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
        public string tuple;    
        public string status = "";

        public NodeOperator(int operator_id, int port) {
            nodeN = operator_id;
            status = nodeN + " is running.";
            portN = port;
        }

        public void runServer()
        {
            TcpChannel channel = new TcpChannel(portN);
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(NodeOperator),
                "Op",
                WellKnownObjectMode.Singleton);
        }

        public void threadRun() {
            status = "Node nº" + nodeN + " is running.";
            /*Falta executar os tuplos, seja la o que isso for.*/
        }

        public void uniqThread() {/*TODO*/
             
        }       
        public void countThread() { /*TODO*/

        }
        public void dupThread() { /*TODO*/

        }
        public void filterThread() { /*TODO*/
  
        }
        public void customThread() { /*TODO*/

        }
        



    }
}
