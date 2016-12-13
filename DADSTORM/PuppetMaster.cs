using NodeManager;
using ProcessCreationService;
using System;
using NodeOperator;// 
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text.RegularExpressions;
using DADStorm.DataTypes;
using static ProcessCreationService.ProcessCreationService;
using System.IO;
using LoggingService;

namespace DADSTORM
{
    class PuppetMaster
    {
        private static Dictionary<string, INodeManager> pcsServers { set; get; } = new Dictionary<string, INodeManager>();
        private static Dictionary<string, string> pcsAddressMapping  { set; get; } = new Dictionary<string, string>();
        private static Dictionary<string, string> nodeAddressMapping { get; set; } = new Dictionary<string, string>();

        private static int Interval { get; set; } = 0;

        static void Main(string[] args)
        {

            var config = new DataTypes.ConfigurationFileObject(PuppetSettings.Default.ConfigFile); //Use this to read configuration files.
            //var v = DataTypes.ConfigurationFileObject.ReadConfig("Test.config"); //or this
            var pcsaddresses = config.ConfigurationNodes.SelectMany(i => i.PCSAddress).Distinct().ToList();
            

            INodeManager pcs = null;
            TcpChannel channel = new TcpChannel(9999);
            ChannelServices.RegisterChannel(channel, true);
            int port = 10000;
            int portNodes = 11000;

            foreach (var address in config.ConfigurationNodes.SelectMany(i => i.Addresses))   //Config node = details of one operation
            {
                nodeAddressMapping.Add(address, "tcp://localhost:" + portNodes + "/op" + portNodes);  //Creates the same mapping but for the nodes instead of the pcs
                portNodes += 1;
            }
            portNodes = 11000;
            foreach (var uniqAddress in pcsaddresses) {     //Iteration over all the PCSs
                pcsAddressMapping.Add(uniqAddress,"localhost:" + port++);   //Create a mapping for the real (localhost) address

                pcs = (INodeManager)Activator.GetObject(typeof(INodeManager), "tcp://" + pcsAddressMapping[uniqAddress] + "/NodeManagerService");

                List<DADStorm.DataTypes.NodeOperatorData> ListOfNodeInformations = new List<DADStorm.DataTypes.NodeOperatorData>();
                var ConfigNodesWherePCSIsIncluded = config.getNodesFromPCS(uniqAddress);
                foreach (var ConfigNode in ConfigNodesWherePCSIsIncluded)  //Iteration over all the configuration nodes for that PCS
                {
                    var NodeAddresses = ConfigNode.Addresses.Where(i => i.Contains(uniqAddress)).ToList();  //Fake (remote) Addresses of the node
                    foreach(var NodeAddress in NodeAddresses)
                    {
                        List<DADStorm.DataTypes.Downstream> downstream = new List<DADStorm.DataTypes.Downstream>();
                        List<string> siblings = new List<string>();
                        foreach (var node in ConfigNode.Addresses) //Creation of a list of siblings
                        {
                            if(!node.Equals(NodeAddress))
                                siblings.Add(nodeAddressMapping[node]);
                        }
                        foreach (var OperatorConfig in config.getNodesRequiringOPName(ConfigNode.NodeName))    //Configuration nodes that require our operation (downstreams)
                        {
                            var listOfDownstreamNodes = new List<string>();
                            foreach (var downstreamnode in OperatorConfig.Addresses)
                            {
                                listOfDownstreamNodes.Add(nodeAddressMapping[downstreamnode]);
                            }
                            downstream.Add(new DADStorm.DataTypes.Downstream()
                            {
                                Routing = OperatorConfig.Routing,
                                TargetIPs = listOfDownstreamNodes
                            });
                        }
                        List<DADTuple> initialTuples;
                        if (ConfigNode.TargetData.Contains("."))
                        {
                            initialTuples = DADTuple.InputFileReader(ConfigNode.TargetData);
                        }
                        else initialTuples = new List<DADTuple>();

                        List<string> Opargs;
                        if (ConfigNode.Operation == DADStorm.DataTypes.OperatorType.custom)
                        {
                            byte[] code = File.ReadAllBytes(ConfigNode.OperationArgs[0]);
                            Opargs = new List<string>()
                            {
                                code.ToString(),
                                ConfigNode.OperationArgs[1]
                            };
                        }
                        else
                        {
                            Opargs = ConfigNode.OperationArgs;
                        }
                        DADStorm.DataTypes.NodeOperatorData data = new DADStorm.DataTypes.NodeOperatorData()
                        {
                            OperatorID = ConfigNode.NodeName,
                            ConnectionPort = portNodes++,
                            OperatorName = "op",    //TODO:Change this to come from config
                            TypeofRouting = ConfigNode.Routing,
                            Siblings = siblings,
                            Downstream = downstream,
                            TypeofOperation = ConfigNode.Operation,
                            OperationArgs = Opargs,
                            Initialtuples = initialTuples,
                            NodeAddress = NodeAddress,
                            LogTuples = (config.Logging == DataTypes.LoggingLevel.full)
                        };
                        
                        ListOfNodeInformations.Add(data);   //New node that will be created by the PCS with the data stated right up here.
                    }
                }
                //DADStorm.DataTypes.NodeOperatorData nop = new DADStorm.DataTypes.NodeOperatorData()
                //{
                //}

                var bo = pcs.init(ListOfNodeInformations);
                pcsServers.Add(uniqAddress, pcs);
            }

            //TcpChannel channelLog = new TcpChannel(9999);
            //ChannelServices.RegisterChannel(channelLog, true);
            System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Logger),
                "logger",
                    System.Runtime.Remoting.WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Logging Service open on port: " + port);

            ILogger Logger = (ILogger)Activator.GetObject(typeof(INodeOperator), "tcp://localhost:9999/logger");
            //var test = config.ConfigurationNodes.Where(i => i.NodeName == "OP1").ToList().First().PCSAddress.First();



            /*    if (pcsLocalhost == null) {
                    System.Console.WriteLine("Could not locate server.");
                    return;
                }*/

            foreach(var line in config.Script)
            {
                Logger.log(line.ToString());
                string[] command = line.ToString().Split(null);

                try
                {
                    handleInput(config, command);
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Invalid Arguments");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Available commands are:");
            Console.WriteLine("     start OP_ID");
            Console.WriteLine("     status");
            Console.WriteLine("     interval time");
            Console.WriteLine("     crash OP_ID Rep_ID");
            Console.WriteLine("     freeze OP_ID Rep_ID");
            Console.WriteLine("     unfreeze OP_ID Rep_ID");
            Console.WriteLine();

            while (true) {

                string commandLine = Console.ReadLine();
                Logger.log(commandLine);
                string[] command = commandLine.Split(null);

                try
                {
                    handleInput(config, command);
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Invalid Arguments");
                }
            }
        }

        private static void handleInput(DataTypes.ConfigurationFileObject config, string[] command)
        {
            List<string> psCandidate = null;
            List<string> nodeCandidate = null;
            switch (command[0].ToLower())
            {
                case "start":
                    if (config.ConfigurationNodes.Where(i => i.NodeName.Equals(command[1])).Select(i => i.TargetData).First().Contains("."))
                    {   //HACK: So that start other nodes does not do anything
                        psCandidate = config.getAddressesFromOPName(command[1]);
                        var res = psCandidate.First();
                        pcsServers[res].start(command[1]);
                    }
                    //foreach (var p in ps)
                    //{
                    //    pcsServers[p].start(command[1]);
                    //}

                    break;

                case "status":
                    foreach (var pcss in pcsServers.Select(i => i.Value))
                    {
                        pcss.status();
                    }
                    break;

                case "interval":
                    Interval = Convert.ToInt32(command[2]);
                    break;

                case "crash":
                    psCandidate = config.getAddressesFromOPName(command[1]);
                    nodeCandidate = config.ConfigurationNodes.Where(i => i.NodeName == command[1]).SelectMany(n => n.Addresses).ToList();
                    var toCrash = psCandidate[Convert.ToInt32(command[2])];
                    pcsServers[toCrash].crash(nodeAddressMapping[nodeCandidate[Convert.ToInt32(command[2])]]);
                    break;

                case "freeze":
                    psCandidate = config.getAddressesFromOPName(command[1]);
                    nodeCandidate = config.ConfigurationNodes.Where(i => i.NodeName == command[1]).SelectMany(n => n.Addresses).ToList();
                    var toFreeze = psCandidate[Convert.ToInt32(command[2])];
                    pcsServers[toFreeze].freeze(nodeAddressMapping[nodeCandidate[Convert.ToInt32(command[2])]]);
                    break;

                case "unfreeze":
                    psCandidate = config.getAddressesFromOPName(command[1]);
                    nodeCandidate = config.ConfigurationNodes.Where(i => i.NodeName == command[1]).SelectMany(n => n.Addresses).ToList();
                    var toThaw = psCandidate[Convert.ToInt32(command[2])];
                    pcsServers[toThaw].unfreeze(nodeAddressMapping[nodeCandidate[Convert.ToInt32(command[2])]]);
                    break;
                case "wait":
                    Thread.Sleep(Convert.ToInt32(command[1]));
                    break;
                default:
                    Console.WriteLine("Invalid command");
                    break;
            }
            Thread.Sleep(Interval);
        }
    }
}
