using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DADStorm.DataTypes
{
    public class DADTuple
    {
        public string Item1 { get { if (Items.Count > 0) return Items[0]; else return null; } }
        public string Item2 { get { if (Items.Count > 1) return Items[1]; else return null; } }
        public string Item3 { get { if (Items.Count > 2) return Items[2]; else return null; } }
        public string Item4 { get { if (Items.Count > 3) return Items[3]; else return null; } }
        public string Item5 { get { if (Items.Count > 4) return Items[4]; else return null; } }

        public List<string> Items { get; private set; }
        public DADTuple(params string[] inputs)
        {
            Items = inputs.ToList();
        }
        public void addItems(params string[] inputs)
        {
            Items.AddRange(inputs);
        }

        public string getIndex(int index)
        {
            if (Items.Count > index)
                return Items[index];
            else return "";
        }
    }
}
