using ProcessCreationService;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text.RegularExpressions;
using System.Linq;

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

		private void readConfig(string filename)
		{
			string[] lines = System.IO.File.ReadAllLines(@filename);

            // Display the file contents by using a foreach loop.
            foreach (string line in lines)
            {
				string[] readLine = line.Split('%');
				string information = readLine[0];
				List<DataTypes.ConfigurationData> configurations = new List<DataTypes.ConfigurationData>();

				if(information.Contains("INPUT_OPS"))	//This is a configuration
				{
					DataTypes.ConfigurationData data = new DataTypes.ConfigurationData();
					var regex = new Regex(@"\b[\s,]*");
					var words = regex.Split(information).Where(x => !string.IsNullOrEmpty(x)).ToList();

					data.NumberofReplicas = Convert.ToInt32(words[6]);
					data.NodeName = words[0];
					data.TargetData = words[3];
					switch(words[8]){
						case "random":
							data.Routing = DataTypes.RoutingType.random;
							break;
						case "primary":
							data.Routing = DataTypes.RoutingType.primary;
							break;
						default:
							data.Routing = DataTypes.RoutingType.hashing;
							data.RoutingArg = words[8][8];
							break;
					}
					data.Addresses = new List<string>(data.NumberofReplicas);
					for(int i = 0; i < data.NumberofReplicas; i++)
					{
						data.Addresses.Add(words[words.IndexOf("address") + 1 + i]);
					}
					int operationIndex = words.IndexOf("spec") + 1;
					switch(words[words.IndexOf("spec") + 1]){
						case "FILTER":
							data.Operation = DataTypes.OperatorType.filter;
							break;
						case "CUSTOM":
							data.Operation = DataTypes.OperatorType.custom;
							break;
						case "UNIQ":
							data.Operation = DataTypes.OperatorType.uniq;
							break;
						case "COUNT":
							data.Operation = DataTypes.OperatorType.count;
							break;
					}
					data.OperationArgs = new List<string>();
					int pos = 1;
					string arg;
					while(!string.IsNullOrWhiteSpace(arg = words[operationIndex + pos]))
					{
						data.OperationArgs.Add(arg);
					}
					configurations.Add(data);
				}
            }
		}
    }
}
