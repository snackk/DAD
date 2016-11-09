using ProcessCreationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace DADSTORM
{
    class PuppetMaster
    {
        static void Main(string[] args)
        {

            NodeManagerService pcs;
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            pcs = (NodeManagerService)Activator.GetObject(typeof(NodeManagerService),
                "tcp://localhost:10000/NodeManagerService");

            for (int i = 0; i < 100; i++) {
                pcs.start(i);
            }

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(pcs.status());
            }
            System.Console.ReadLine();
        }
    }
}
