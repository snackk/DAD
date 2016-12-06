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
}
