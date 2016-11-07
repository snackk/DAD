﻿using System;

namespace NodeManager
{
    public interface NodeManager
    {
        void start(int operator_id);

        void interval(int operator_id, int x_ms);

        string status();

        void crash(string processname);

        void freeze(string processname);

        void unfreeze(string processname);
    }
}