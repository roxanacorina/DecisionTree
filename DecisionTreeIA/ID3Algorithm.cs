using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeIA
{
    public class ID3Algorithm
    {

        List<Attribute> Tree;

        bool writeTabs;

        public ID3Algorithm(List<ProcesedAttribute> procesedAttributes, List<Attribute> tree)
        {
            Tree = tree;
            // calculate the entropy of the target
            int posValTarget = procesedAttributes.Last().values.First().PossitiveValue;
            int negValTarget = procesedAttributes.Last().values.Last().NegativeValue;

            double targetEntropy = ComputeEntropy(posValTarget, negValTarget);

            // calculate the entropy of the target + each attribute
            double entropy = 0;
            List<double> listOfEntropyForAllAttributes = new List<double>();

            for (int i = 0; i < procesedAttributes.Count - 1; i++)
            {
                entropy = ComputeSpecificConditionalEntropy(procesedAttributes.Last(), procesedAttributes[i]);
                listOfEntropyForAllAttributes.Add(entropy);
            }


            //calculate information gain

            double infoGainTemp = 0;
            double infoGain = 0;
            List<double> listOfInformationGainForAllAttributes = new List<double>();

            for (int i = 0; i < procesedAttributes.Count - 1; i++)
            {
                infoGainTemp = targetEntropy - listOfEntropyForAllAttributes[i];
                if (infoGainTemp > infoGain)
                {
                    infoGain = infoGainTemp;
                    //indexOfAttributeWithTheBestInformationGain = i;
                }
                listOfInformationGainForAllAttributes.Add(infoGain);
            }

            //choose attribute with the largest information gain as the decision node


            List<int> AttributesIndexes = new List<int>();
            for (int i = 0; i < procesedAttributes.Count - 1; i++)
            {
                AttributesIndexes.Add(ReturnMaxIndex(listOfInformationGainForAllAttributes));
            }

            List<ProcesedAttribute> SortedAttributes = new List<ProcesedAttribute>();
            for (int i = 0; i < AttributesIndexes.Count(); i++)
            {
                SortedAttributes.Add(procesedAttributes[i]);
            }
            int currentAttributeIndex = 1;
            Node root = new Node
            {
                Parent = null,
                Attribute = SortedAttributes.First(),
                Value = SortedAttributes.First().Name,
                EdgeValue = string.Empty,
                Children = new List<Node>()
            };

            PopulateChildren(root, currentAttributeIndex, SortedAttributes);
            writeTabs = false;
            PrintTree(root, 0);

            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine("");

        }

        private void PrintTree(Node node, int level)
        {
            if (node.Children == null || node.Children.Count == 0)
            {
                if (writeTabs)
                {
                    string tabsInFront = "";
                    for (int i = 0; i < level; i++)
                    {
                        tabsInFront += "   \t    ";
                    }
                    for (int i = 0; i < level; i++)
                    {
                        tabsInFront += "    ";
                    }
                    Console.Write(tabsInFront);
                    writeTabs = false;
                }
                Console.Write("-" + node.EdgeValue + "-" + node.Value);
                Console.WriteLine();
                writeTabs = true;
            }
            else
            {
                if (writeTabs)
                {
                    string tabsInFront = "";
                    for (int i = 0; i < level; i++)
                    {
                        tabsInFront += "   \t    ";
                        
                    }

                    for(int i = 0; i< level; i++)
                    {
                        tabsInFront += "    ";
                    }
                    Console.Write(tabsInFront);
                    writeTabs = false;
                }
                if(node.Parent != null)
                {
                    Console.Write("-" + node.EdgeValue + "-<" + node.Attribute.Name+">");
                }
                else
                {
                    Console.Write("<"+node.Attribute.Name+">\t        ");
                }
                foreach (var child in node.Children)
                {
                    PrintTree(child, level + 1);
                }

            }
        }

        private void PopulateChildren(Node node, int currentAttributeIndex, List<ProcesedAttribute> SortedAttributes)
        {
            if (currentAttributeIndex <= SortedAttributes.Count)
            {
                foreach (var attributeValue in node.Attribute.values)
                {
                    if (attributeValue.PossitiveValue > 0 && attributeValue.NegativeValue == 0)
                    {
                        node.Children.Add(new Node
                        {
                            Parent = node,
                            Value = "True",
                            Attribute = null,
                            EdgeValue = attributeValue.Name,
                            Children = null
                        });
                    }
                    else if (attributeValue.PossitiveValue == 0 && attributeValue.NegativeValue > 0)
                    {
                        node.Children.Add(new Node
                        {
                            Parent = node,
                            Value = "False",
                            Attribute = null,
                            EdgeValue = attributeValue.Name,
                            Children = null
                        });
                    }
                    else if (node.Parent == null) // tratez cazul in care node este radacina
                    {
                        ProcesedAttribute nextAttribute = SortedAttributes[currentAttributeIndex];
                        currentAttributeIndex++;
                        Node nodeToAdd = new Node
                        {
                            Parent = node,
                            Value = nextAttribute.Name,
                            Attribute = nextAttribute,
                            EdgeValue = attributeValue.Name,
                            Children = new List<Node>()
                        };
                        node.Children.Add(nodeToAdd);
                        PopulateChildren(nodeToAdd, currentAttributeIndex, SortedAttributes);

                    }
                    else // node NU este radacina
                    {
                        int positiveValue = GetPositiveValueFor(node, attributeValue);
                        int negativeValue = GetNegativeValueFor(node, attributeValue);
                        if (positiveValue > 0 && negativeValue == 0)
                        {
                            node.Children.Add(new Node
                            {
                                Parent = node,
                                Value = "True",
                                Attribute = null,
                                EdgeValue = attributeValue.Name,
                                Children = null
                            });
                        }
                        else if (positiveValue == 0 && negativeValue > 0)
                        {
                            node.Children.Add(new Node
                            {
                                Parent = node,
                                Value = "False",
                                Attribute = null,
                                EdgeValue = attributeValue.Name,
                                Children = null
                            });
                        }
                        else
                        {
                            ProcesedAttribute nextAttribute = SortedAttributes[currentAttributeIndex];
                            currentAttributeIndex++;
                            Node nodeToAdd = new Node
                            {
                                Parent = node,
                                Value = nextAttribute.Name,
                                Attribute = nextAttribute,
                                EdgeValue = attributeValue.Name,
                                Children = new List<Node>()
                            };
                            node.Children.Add(nodeToAdd);
                            PopulateChildren(nodeToAdd, currentAttributeIndex, SortedAttributes);
                        }
                    }
                }
            }
        }

        private int GetNegativeValueFor(Node node, Value attributeValue)
        {
            int negativeValue = 0;
            string valuesToLookFor = node.Attribute.Name + "," + attributeValue.Name;
            Node auxiliarNode = node;
            Node auxiliarParent = node.Parent;
            while (auxiliarParent != null)
            {
                valuesToLookFor = auxiliarParent.Attribute.Name + "," + auxiliarNode.EdgeValue + ":" + valuesToLookFor;
                auxiliarNode = auxiliarNode.Parent;
                auxiliarParent = auxiliarNode.Parent;

            }

            string[] AttributesAndValues = valuesToLookFor.Split(':');
            for (int index = 0; index < Tree.First().values.Count; index++)
            {
                bool foundMatch = true;
                for (int i = 0; i < AttributesAndValues.Length; i++)
                {
                    string[] currentAttributeAndValue = AttributesAndValues[i].Split(',');
                    if (Tree.First(a => a.Name == currentAttributeAndValue[0]).values[index] == currentAttributeAndValue[1])
                    {
                        continue;
                    }
                    else
                    {
                        foundMatch = false;
                        break;
                    }
                }
                if (foundMatch)
                {
                    if (Tree.Last().values[index] == "F")
                    {
                        negativeValue++;
                    }
                }
            }

            return negativeValue;
        }

        private int GetPositiveValueFor(Node node, Value attributeValue)
        {
            int positiveValue = 0;
            string valuesToLookFor = node.Attribute.Name + "," + attributeValue.Name;
            Node auxiliarNode = node;
            Node auxiliarParent = node.Parent;
            while (auxiliarParent != null)
            {
                valuesToLookFor = auxiliarParent.Attribute.Name + "," + auxiliarNode.EdgeValue + ":" + valuesToLookFor;
                auxiliarNode = auxiliarNode.Parent;
                auxiliarParent = auxiliarNode.Parent;

            }

            string[] AttributesAndValues = valuesToLookFor.Split(':');
            for (int index = 0; index < Tree.First().values.Count; index++)
            {
                bool foundMatch = true;
                for (int i = 0; i < AttributesAndValues.Length; i++)
                {
                    string[] currentAttributeAndValue = AttributesAndValues[i].Split(',');
                    if (Tree.First(a => a.Name == currentAttributeAndValue[0]).values[index] == currentAttributeAndValue[1])
                    {
                        continue;
                    }
                    else
                    {
                        foundMatch = false;
                        break;
                    }
                }
                if (foundMatch)
                {
                    if (Tree.Last().values[index] == "T")
                    {
                        positiveValue++;
                    }
                }
            }

            return positiveValue;
        }

        public int ReturnMaxIndex(List<double> listOfInformationGainForAllAttributes)
        {
            int nextAttributeWithBestInformationGainIndex = 0;
            for (int i = 0; i < listOfInformationGainForAllAttributes.Count(); i++)
            {
                if (listOfInformationGainForAllAttributes[i] == listOfInformationGainForAllAttributes.Max())
                {
                    nextAttributeWithBestInformationGainIndex = i;
                    double x = listOfInformationGainForAllAttributes[i];
                    listOfInformationGainForAllAttributes[i] = -1;
                    return nextAttributeWithBestInformationGainIndex;
                }
            }
            return nextAttributeWithBestInformationGainIndex;
        }
        public double ComputeEntropy(int posValues, int negValues)
        {
            double result = 0;

            double ratioPossitive = (double)(posValues) / (double)(posValues + negValues);
            double ratioNegative = (double)(negValues) / (double)(posValues + negValues);
            double ratioPossitiveLog = Math.Log(ratioPossitive, 2);
            double ratioNegativeLog = Math.Log(ratioNegative, 2);
            if (posValues == 0 || negValues == 0)
            {
                result = 0;
            }
            else
            {
                result = -(ratioPossitive * ratioPossitiveLog) - (ratioNegative * ratioNegativeLog);
            }

            return result;
        }

        public double ComputeSpecificConditionalEntropy(ProcesedAttribute target, ProcesedAttribute attribute)
        {
            double result = 0;
            double probability;


            foreach (var item in attribute.values)
            {
                probability = ((double)(item.PossitiveValue + item.NegativeValue) / (double)(target.values[0].PossitiveValue + target.values[1].NegativeValue));
                result = result + (probability * ComputeEntropy(item.PossitiveValue, item.NegativeValue));
            }
            return result;
        }


    }
}
