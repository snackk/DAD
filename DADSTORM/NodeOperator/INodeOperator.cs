using DADStorm.DataTypes;
using System.Collections.Generic;

namespace NodeOperator
{
    public interface INodeOperator
    {
        void makeNodeWork();
        void uniqThread();
        void countThread();
        void dupThread();
        void filterThread();
        void customThread();
        bool replicateTuples(List<DADTuple> replicatedTuples);
    }
}
