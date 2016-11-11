using ProcessCreationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Text.RegularExpressions;
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

            Console.WriteLine("PuppetMaster connected to NodeManager.");
            /*
            for (int i = 0; i < 100; i++) {
                int a = 11000 + i;
                Console.WriteLine(a);
                pcs.start(i, "CUSTOM", a);
            }

            for (int i = 0; i < 100; i++)
            {
                NodeOperator.NodeOperator a = (NodeOperator.NodeOperator)Activator.GetObject(typeof(NodeOperator.NodeOperator),
                    "tcp://localhost:" + 11000 + i + "/Op");
                Console.WriteLine(a.status);
            }
            System.Console.ReadLine();*/
            while (true) {
                string command = Console.ReadLine();
                Regex r = new Regex(@"\d+");
                Match m = r.Match(command);
                int opID = 0;
                if (m.Success) {
                    opID = Int32.Parse(m.Value);
                }
                Regex.Replace(command, "[^0-9]+", string.Empty);
                switch (command) {
                    case "start":
                        pcs.start(opID, "", 0);
                        Console.WriteLine("Created node: " + opID);
                        break;
                    case "status":
                        Console.WriteLine(pcs.status(opID));
                        break;
                }
            }
        }
    }
}
