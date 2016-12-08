using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DADStorm.DataTypes
{
    [Serializable]
    public class NodeOperatorData
    {
        public int ConnectionPort { get; set; }
        public string OperatorName { get; set; }
        public List<string> Downstream { get; set; }
        public RoutingType TypeofRouting { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
