using NodeManager;
using ProcessCreationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text.RegularExpressions;

namespace DADSTORM
{
    class PuppetMaster
    {
        private static Dictionary<string, INodeManager> pcsServers { set; get; } = new Dictionary<string, INodeManager>();

        static void Main(string[] args)
        {
            var v = new DataTypes.ConfigurationFileObject("test.config"); //Use this to read configuration files.
            //var v = DataTypes.ConfigurationFileObject.ReadConfig("Test.config"); //or this
            
            INodeManager pcsLocalhost = null;
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            pcsLocalhost = (INodeManager)Activator.GetObject(typeof(INodeManager),
                "tcp://localhost:10000/NodeManagerService");

            if (pcsLocalhost == null) {
                System.Console.WriteLine("Could not locate server.");
                return;
            }

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
                string commandRes = "";
                try
                {
                    if (command[1] != "")
                    {
                        switch (command[0])
                        {

                            case "start":
                                commandRes = pcsLocalhost.start(command[1]);
                                break;

                            case "status":
                                commandRes = pcsLocalhost.status(command[1]);
                                break;

                            case "interval":
                                commandRes = pcsLocalhost.interval(command[1],Int32.Parse(command[2]));
                                break;

                            case "crash":
                                commandRes = pcsLocalhost.crash(command[1]);
                                break;

                            case "freeze":
                                commandRes = pcsLocalhost.freeze(command[1]);
                                break;

                            case "unfreeze":
                                commandRes = pcsLocalhost.unfreeze(command[1]);
                                break;
                        }
                        Console.WriteLine(commandRes);
                    }
                }
                catch (IndexOutOfRangeException) {

                }
            }
        }
    }
}
