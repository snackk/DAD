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
}
