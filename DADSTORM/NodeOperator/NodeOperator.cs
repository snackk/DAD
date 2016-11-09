using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace NodeOperator
{
    public class NodeOperator
    {
        public int nodeN { private set; get; }
        
        public string status;

        public NodeOperator(int operator_id) {
            nodeN = operator_id;
        }

        public string getStatus() {
            return status;
        }

        public void threadRun() {
            status = "Node nº" + nodeN + " is running.";
            /*Falta executar os tuplos, seja la o que isso for.*/
        }
        public delegate void func();



    }
}
