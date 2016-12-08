using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DADStorm.DataTypes
{
    [Serializable]
    public class Downstream
    {
        public RoutingType Routing { get; set; }
        public List<string> TargetIPs { get; set; }
    }
}
