using System;
using System.Collections;

namespace NodeManager
{
    public interface INodeManager
    {
        string start(string operatorID);//, string operation, int operatorPort);

        string interval(string operatorID, int x_ms);

        string status(string operatorID);

        string crash(string operatorID);

        string freeze(string operatorID);

        string unfreeze(string operatorID);
    }
}
