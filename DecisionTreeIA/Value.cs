using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeIA
{
    public class Value //Label
    {
        public string Name { get; set; }
        public int possitiveValue { get; set; }
        public int negativeValue { get; set; }
        public int allValues;
        public Value()
        {
            possitiveValue = 0;
            negativeValue = 0;
            allValues = 0;
        }
    }
}
