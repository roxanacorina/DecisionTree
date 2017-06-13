using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeIA
{
    public class Value
    {
        public string Name { get; set; }
        public int PossitiveValue { get; set; }
        public int NegativeValue { get; set; }
        public int AllValues;
        public Value()
        {
            PossitiveValue = 0;
            NegativeValue = 0;
            AllValues = 0;
        }
    }
}
