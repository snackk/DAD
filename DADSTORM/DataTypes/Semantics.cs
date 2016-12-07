using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DADSTORM.DataTypes
{
    public enum SemanticsType
    {
        [Description("At-most-once")]
        at_most_once,
        [Description("At-least-once")]
        at_least_once,
        [Description("Exactly-once")]
        exactly_once
    }
    public static class SemanticsTypeExtensions
    {
        public static string ToDescriptionString(this SemanticsType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
