namespace NodeOperator
{
    public interface INodeOperator
    {
        void uniqThread();
        void countThread();
        void dupThread();
        void filterThread();
        void customThread();
        int replicate(int digger);
    }
}
