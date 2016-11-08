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
        public bool procRunning { private set; get; }
        public string status;

        public NodeOperator(int operator_id) {
            nodeN = operator_id;
        }

        public string getStatus() {
            return "";
        }

        public void tryme() {
            status = "Node nº" + nodeN + " is running.";
        }
    }
}
