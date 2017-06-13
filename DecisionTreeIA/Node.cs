using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeIA
{
    public class Node
    {
        public Node Parent { get; set; }
        public string Value { get; set; }
        public ProcesedAttribute Attribute { get; set; }
        public string EdgeValue { get; set; }
        public List<Node> Children { get; set; }


    }
}
