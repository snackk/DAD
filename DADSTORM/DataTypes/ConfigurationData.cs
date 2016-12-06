using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DADSTORM.DataTypes
{
    public enum RoutingType
    {
        [Description("Hashing")]
        hashing,
        [Description("Random")]
        random,
        [Description("Primary")]
        primary
    }
    public enum OperatorType
    {
        [Description("Dup")]
        dup,
        [Description("Filter")]
        filter,
        [Description("Custom")]
        custom,
        [Description("Uniq")]
        uniq,
        [Description("Count")]
        count
    }

    public static class EnumExtensions
    {
        public static string ToDescriptionString(this RoutingType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
        public static string ToDescriptionString(this OperatorType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
        public static string ToDescriptionString(this LoggingLevel val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
        public static string ToDescriptionString(this SemanticsType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
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

        public override string ToString()
        {
            return "Node: " + NodeName + " | Routing: " + Routing.ToDescriptionString() + " | Data: " + TargetData + " | Operation: " + Operation.ToDescriptionString();
        }
    }
}
