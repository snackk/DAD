using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DADSTORM.DataTypes
{
    public enum ScriptCommand
    {
        [Description("Start")]
        start,
        [Description("Interval")]
        interval,
        [Description("Status")]
        status,
        [Description("Crash")]
        crash,
        [Description("Freeze")]
        freeze,
        [Description("Unfreeze")]
        unfreeze,
        [Description("Wait")]
        wait
    }

    public static class ScritCommandEnumExtensions
    {
        public static string ToDescriptionString(this ScriptCommand val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
        class ConfigScriptLine
    {
        private string nodeID;
        private string rep_ID;
        private string unparsedLine;
        private int opValue;
        public ScriptCommand Operation { get; set; }
        public string NodeName
        {
            get
            {
                switch (Operation)
                {
                    case ScriptCommand.status:
                    case ScriptCommand.wait:
                        return null;
                    default:
                        return nodeID;
                };
            }
            set { nodeID = value; }
        }
        public string Rep_ID
        {
            get
            {
                switch (Operation)
                {
                    case ScriptCommand.crash:
                    case ScriptCommand.freeze:
                    case ScriptCommand.unfreeze:
                        return rep_ID;
                    default:
                        return null;
                }
            }
            set { rep_ID = value; }
        }
        public int Value
        {
            get
            { switch (Operation)
                {
                    case ScriptCommand.freeze:
                    case ScriptCommand.interval:
                        return opValue;
                    default:
                        return -1;
                }
            }
            set { opValue = value; }
        }

        public override string ToString()
        {
            return unparsedLine;
        }

        public ConfigScriptLine(List<string> words)
        {
            bool first = true;
            foreach (string word in words)
            {
                unparsedLine += (first ? "" : " ") + word;
                first = false;
            }
            switch (words[0].ToUpperInvariant())
            {
                case "INTERVAL":
                    Operation = ScriptCommand.interval;
                    Value = Convert.ToInt32(words[2]);
                    goto case "nodename";
                case "START":
                    Operation = ScriptCommand.start;
                    goto case "nodename";
                case "STATUS":
                    Operation = ScriptCommand.status;
                    break;
                case "CRASH":
                    Operation = ScriptCommand.crash;
                    goto case "repID";
                case "FREEZE":
                    Operation = ScriptCommand.freeze;
                    goto case "repID";
                case "UNFREEZE":
                    Operation = ScriptCommand.unfreeze;
                    goto case "repID";
                case "WAIT":
                    Operation = ScriptCommand.wait;
                    Value = Convert.ToInt32(words[1]);
                    break;


                case "repID":
                    Rep_ID = words[2];
                    goto case "nodename";
                case "nodename":
                    NodeName = words[1];
                    break;
            }
        }
    }
}
