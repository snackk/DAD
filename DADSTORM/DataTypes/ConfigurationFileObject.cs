using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DADSTORM.DataTypes
{
    class ConfigurationFileObject
    {
        public List<ConfigurationData> ConfigurationNodes { get; set; }
        public LoggingLevel Logging { get; set; }
        public SemanticsType Semantics { get; set; }


        public ConfigurationFileObject()
        {
            ConfigurationNodes = new List<ConfigurationData>();
        }

        /// <summary>
        /// Reads a configuration file and creates an object that represents it and it's parameters.
        /// </summary>
        /// <param name="filename">The relative or absolute path of the configuration file.</param>
        public ConfigurationFileObject(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(@filename);

            // Display the file contents by using a foreach loop.
            foreach (string line in lines)
            {
                string[] readLine = line.Split('%');
                string information = readLine[0];

                if (information.Contains("Semantics") || information.Contains("semantics") || information.Contains("LoggingLevel"))
                {
                    var regex = new Regex(@"[^\p{L}]*\p{Z}[^\p{L}]*");
                    var words = regex.Split(information).Where(x => !string.IsNullOrEmpty(x)).ToList();
                    switch (words[1])
                    {
                        case "at-most-once":
                            Semantics = DataTypes.SemanticsType.at_most_once;
                            break;
                        case "at-least-once":
                            Semantics = DataTypes.SemanticsType.at_least_once;
                            break;
                        case "exactly-once":
                            Semantics = DataTypes.SemanticsType.exactly_once;
                            break;
                        case "light":
                            Logging = DataTypes.LoggingLevel.light;
                            break;
                        case "full":
                            Logging = DataTypes.LoggingLevel.full;
                            break;
                    }

                }

                if (information.Contains("INPUT_OPS") || information.Contains("input ops"))  //This is a configuration
                {
                    DataTypes.ConfigurationData data = new DataTypes.ConfigurationData();
                    var regex = new Regex(@"[^\p{L}0-9\=]*[\p{Z}\(\)\,\""][^\p{L}0-9\=]*");
                    var words = regex.Split(information).Where(x => !string.IsNullOrEmpty(x)).ToList();

                    data.NumberofReplicas = Convert.ToInt32(words[words.IndexOf("fact") + 1]);
                    data.NodeName = words[0];
                    data.TargetData = words[3];
                    switch (words[words.IndexOf("routing") + 1])
                    {
                        case "random":
                            data.Routing = DataTypes.RoutingType.random;
                            break;
                        case "primary":
                            data.Routing = DataTypes.RoutingType.primary;
                            break;
                        default:
                            data.Routing = DataTypes.RoutingType.hashing;
                            data.RoutingArg = Convert.ToInt32(words[words.IndexOf("routing") + 2]);
                            break;
                    }
                    data.Addresses = new List<string>(data.NumberofReplicas);
                    for (int i = 0; i < data.NumberofReplicas; i++)
                    {
                        data.Addresses.Add(words[words.IndexOf("address") + 1 + i]);
                    }
                    int operationIndex = words.IndexOf("spec") + 1;
                    switch (words[operationIndex])
                    {
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
                    try
                    {
                        while (!string.IsNullOrWhiteSpace(arg = words[operationIndex + pos]))
                        {
                            data.OperationArgs.Add(arg);
                            pos++;
                        }
                    }
                    catch (ArgumentOutOfRangeException) { } //it is expected to be out of range
                    ConfigurationNodes.Add(data);
                }
            }
        }

        public static DataTypes.ConfigurationFileObject ReadConfig(string filename)
        {
            return new ConfigurationFileObject(filename);
        }
    }
}
