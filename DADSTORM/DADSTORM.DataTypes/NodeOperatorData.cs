﻿using System;
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
        public string OperatorName { get; set; }
        public OperatorType TypeofOperation { get; set; }
        public int ConnectionPort { get; set; }
        public List<Downstream> Downstream { get; set; }
        public RoutingType TypeofRouting { get; set; }
        public List<string> Siblings { get; set; }
        public List<string> OperationArgs { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}