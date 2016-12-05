using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DADSTORM.DataTypes
{
    enum RoutingType
    {
        hashing,
        random,
        primary
    }
    enum OperatorType
    {
        dup,
        filter,
        custom,
        uniq,
        count
    }

    /// <summary>
    /// Class that holds a configuration node of the configuration file.
    /// </summary>
    class ConfigurationData
    {
        public string NodeName { get; set; }
        public string TargetData { get; set; }

        public int NumberofReplicas { get; set; }
        public RoutingType Routing { get; set; }
        public int RoutingArg { get; set; }

        public List<string> Addresses { get; set; }

        public OperatorType Operation { get; set; }
        public List<string> OperationArgs { get; set; }
    }
}
