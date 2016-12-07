using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DADStorm.DataTypes
{
    public class DADTuple
    {
        private const string SplitWordRegexString = @"[^\p{L}0-9\=]*[\p{Z}\(\)\,\""][^\p{L}0-9\=]*";
        public string Item1 { get { if (Items.Count > 0) return Items[0]; else return null; } }
        public string Item2 { get { if (Items.Count > 1) return Items[1]; else return null; } }
        public string Item3 { get { if (Items.Count > 2) return Items[2]; else return null; } }
        public string Item4 { get { if (Items.Count > 3) return Items[3]; else return null; } }
        public string Item5 { get { if (Items.Count > 4) return Items[4]; else return null; } }
        public bool IsNull { get { if (Items.Count <= 0) return true; return false; } }
        public int Count { get { return Items.Count; } }

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

        public DADTuple(string inputLine)
        {
            Items = new List<string>();

            string[] readLine = inputLine.Split('%');
            string information = readLine[0];

            Regex regex;
            List<string> words;

            regex = new Regex(SplitWordRegexString);
            words = regex.Split(information).Where(x => !string.IsNullOrEmpty(x)).ToList();

            foreach(string word in words)
            {
                Items.Add(word);
            }
        }

        public static List<DADTuple> InputFileReader(string inputFileName)
        {
            var output = new List<DADTuple>();
            
            string[] lines = System.IO.File.ReadAllLines(@inputFileName);

            foreach (string line in lines)
            {
                var newTuple = new DADTuple(line);
                if (!newTuple.IsNull)
                {
                    output.Add(newTuple);
                }
            }
            return output;
        }

        public override string ToString()
        {
            return "Tuple count: " + Count + " | Item 1: " + Item1;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(this.GetType()))
            {
                DADTuple other = (DADTuple)obj;
                if (other.Items != null && other.Items != null)
                    return other.Items.SequenceEqual(this.Items);   //HACK: This implies that there are no equal ids in a tuple.
                
                if (other.Items == null && this.Items == null)
                    return true;

                return false;
            }
            return base.Equals(obj);
        }
    }
}
