using ProcessCreationService;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text.RegularExpressions;

namespace DADSTORM
{
    class PuppetMaster
    {
        private static Dictionary<string, NodeManagerService> pcsServers { set; get; } = new Dictionary<string, NodeManagerService>();

        static void Main(string[] args)
        {
            
            NodeManagerService pcsLocalhost;
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            pcsLocalhost = (NodeManagerService)Activator.GetObject(typeof(NodeManagerService),
                "tcp://localhost:10000/NodeManagerService");

            pcsServers.Add("localhost", pcsLocalhost);
            Console.WriteLine("PuppetMaster connected to NodeManagerService on localhost.");
            Console.WriteLine();
            Console.WriteLine("Available commands are:");
            Console.WriteLine("     start [OP_ID]");
            Console.WriteLine("     status [OP_ID]");
            Console.WriteLine("     crash [OP_ID]");
            Console.WriteLine("     freeze [OP_ID]");
            Console.WriteLine("     unfreeze [OP_ID]");
            Console.WriteLine();

            while (true) {
                string[] command = Console.ReadLine().Split(null);
                int opID = 0;
                string commandRes = "";

                Regex r = new Regex(@"\d+");
                Match m = r.Match(command[1]);

                if (m.Success) {
                    opID = Int32.Parse(m.Value);
                }

                switch (command[0]) {
                    
                    case "start":
                        commandRes = pcsLocalhost.start(opID);
                        break;

                    case "status":
                        commandRes = pcsLocalhost.status(opID);
                        break;

                    case "crash":
                        commandRes = pcsLocalhost.crash(opID);
                        break;

                    case "freeze":
                        commandRes = pcsLocalhost.freeze(opID);
                        break;

                    case "unfreeze":
                        commandRes = pcsLocalhost.unfreeze(opID);
                        break;
                }
                Console.WriteLine(commandRes);
            }
        }
    }
}
