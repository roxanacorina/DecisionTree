using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeIA
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> AttributesName = new List<string> { "Color", "Size", "Act", "Age" };
            string Target = "Inflated";
            string[] linesTrain = System.IO.File.ReadAllLines(@"C:\Users\Charlize\Documents\Visual Studio 2015\Projects\DecisionTreeIA\yellow-small+adult-stretch.txt");
            string[] linesTest = System.IO.File.ReadAllLines(@"C:\Users\Charlize\Documents\Visual Studio 2015\Projects\DecisionTreeIA\yellow-small+adult-stretch-test.txt");


            List<Attribute> tree = TreeInitializer(AttributesName, Target, linesTrain);

            List<ProcesedAttribute> ProcesedAttributes = CreateProcesedAttributeList(AttributesName,  Target,  tree);

            ID3Algorithm alg = new ID3Algorithm(ProcesedAttributes, tree);

            List<Attribute> tree2 = TreeInitializer(AttributesName, Target, linesTest);

            List<ProcesedAttribute> ProcesedAttributes2 = CreateProcesedAttributeList(AttributesName, Target, tree2);

            ID3Algorithm alg2 = new ID3Algorithm(ProcesedAttributes2, tree2);

        }

        public static List<ProcesedAttribute> CreateProcesedAttributeList (List<string> AttributesName, string Target, List<Attribute> tree)
        {
            List<ProcesedAttribute> ProcesedAttributes = new List<ProcesedAttribute>();

            foreach (var value in AttributesName)
            {
                ProcesedAttributes.Add(new ProcesedAttribute { Name = value });
            }

            ProcesedAttributes.Add(new ProcesedAttribute { Name = Target });

            foreach (var attribute in tree)
            {
                //set possitive and negative values
                ProcesedAttribute currentProcessedAttribute = ProcesedAttributes.First(p => p.Name == attribute.Name);
                for (int index = 0; index < attribute.values.Count; index++)
                {
                    var value = attribute.values[index];
                    if (!currentProcessedAttribute.values.Any(v => v.Name == value))
                    {
                        currentProcessedAttribute.values.Add(new Value { Name = value });
                    }

                    Value currentValue = currentProcessedAttribute.values.First(v => v.Name == value);
                    if (tree[AttributesName.Count].values[index] == "T")
                    {
                        currentValue.PossitiveValue += 1;
                    }
                    else
                    {
                        currentValue.NegativeValue += 1;
                    }
                    currentValue.AllValues++;
                }
            }
            return ProcesedAttributes;
        }

        public static List<Attribute> TreeInitializer(List<string> AttributesName, string target, string[] lines)
        {
            List<Attribute> tree = new List<Attribute>();
            Attribute a;
            string[] data;
            
            for (int i = 0; i < AttributesName.Count(); i++)
            {
                a = new Attribute();
                a.Name = AttributesName[i];
                tree.Add(a);
            }
            a = new Attribute();
            a.Name = target;
            tree.Add(a);

            
            foreach (var line in lines)
            {
                data = line.Split(',');
                
                
                for (int i = 0; i <= AttributesName.Count; i++)
                {
                    tree[i].values.Add(data[i]);
                }
                
            }
            return tree;
        }
    }
}
