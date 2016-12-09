using DADStorm.DataTypes;
using System.Collections.Generic;

namespace NodeManager
{
    public interface INodeManager
    {


        string interval(string operatorID, int x_ms);

        string status();

        string crash(string operatorID);

        string freeze(string operatorID);

        string unfreeze(string operatorID);

        bool init(List<DADStorm.DataTypes.NodeOperatorData> node);
        void start(string v);
    }
}
