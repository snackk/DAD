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
using static ProcessCreationService.ProcessCreationService;

namespace DADSTORM
{
    class PuppetMaster
    {
        private static Dictionary<string, INodeManager> pcsServers { set; get; } = new Dictionary<string, INodeManager>();
        private static Dictionary<string, string> adressMapping  { set; get; } = new Dictionary<string, string>();


        static void Main(string[] args)
        {

            var config = new DataTypes.ConfigurationFileObject("test.config"); //Use this to read configuration files.
            //var v = DataTypes.ConfigurationFileObject.ReadConfig("Test.config"); //or this
            var pcsaddresses = config.ConfigurationNodes.SelectMany(i => i.PCSAddress).Distinct().ToList();
            

            INodeManager pcs = null;
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            int port = 10000;
            int portNodes = 11000;

            foreach (var uniqAddress in pcsaddresses) {
                adressMapping.Add(uniqAddress,"localhost:" + port++);

                pcs = (INodeManager)Activator.GetObject(typeof(INodeManager), "tcp://" + adressMapping[uniqAddress] + "/NodeManagerService");

                List<DADStorm.DataTypes.NodeOperatorData> ListOfNodeInformations = new List<DADStorm.DataTypes.NodeOperatorData>();
                var referenceNodes = config.getNodesFromPCS(uniqAddress);
                foreach (var add in referenceNodes)
                {
                    var pcsNodesForOperation = add.Addresses.Where(i => i.Contains(uniqAddress)).ToList();
                    foreach(var pcsNode in pcsNodesForOperation)
                    {
                        DADStorm.DataTypes.NodeOperatorData data = new DADStorm.DataTypes.NodeOperatorData()
                        {
                            ConnectionPort = portNodes++,
                            OperatorName = "op",    //TODO:Change this to come from config
                            TypeofRouting = add.Routing
                        };
                        
                        ListOfNodeInformations.Add(data);
                    }
                }
                //DADStorm.DataTypes.NodeOperatorData nop = new DADStorm.DataTypes.NodeOperatorData()
                //{
                //}

                var bo = pcs.init(ListOfNodeInformations);
                pcsServers.Add(uniqAddress, pcs);
            }

            //var test = config.ConfigurationNodes.Where(i => i.NodeName == "OP1").ToList().First().PCSAddress.First();
           
       

        /*    if (pcsLocalhost == null) {
                System.Console.WriteLine("Could not locate server.");
                return;
            }*/

            
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
                INodeManager currentPcs = null;



                try
                {
                    if (command[1] != "")
                    {
                   
                        switch (command[0])
                        {
                                                     

                            case "start":
                                //foreach (var vps in opPcs[command[1]]) {
                                //    commandRes = vps.start(command[1]);
                                //}
                                
                                
                                break;

                            case "status":
                                commandRes = currentPcs.status(command[1]);
                                break;

                            case "interval":
                                commandRes = currentPcs.interval(command[1],Int32.Parse(command[2]));
                                break;

                            case "crash":
                                commandRes = currentPcs.crash(command[1]);
                                break;

                            case "freeze":
                                commandRes = currentPcs.freeze(command[1]);
                                break;

                            case "unfreeze":
                                commandRes = currentPcs.unfreeze(command[1]);
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
