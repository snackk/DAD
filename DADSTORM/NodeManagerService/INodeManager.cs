using System;
using System.Collections;

namespace NodeManager
{
    public interface INodeManager
    {
        string start(int operatorID);//, string operation, int operatorPort);

        string interval(int operatorID, int x_ms);

        string status(int operatorID);

        string crash(int operatorID);

        string freeze(int operatorID);

        string unfreeze(int operatorID);
    }
}
