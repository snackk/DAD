using System;
using System.Collections;

namespace NodeManager
{
    public interface NodeManager
    {
        void start(int operatorID, string operation, int operatorPort);

        void interval(int operatorID, int x_ms);

        string status(int operatorID);

        void crash(int operatorID);

        void freeze(int operatorID);

        void unfreeze(int operatorID);
    }
}
