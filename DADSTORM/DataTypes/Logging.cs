using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DADSTORM.DataTypes
{
    public enum LoggingLevel
    {
        [Description("Light")]
        light,
        [Description("Full")]
        full
    }
    public static class LoggingLevelExtension
    {
        public static string ToDescriptionString(this LoggingLevel val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
