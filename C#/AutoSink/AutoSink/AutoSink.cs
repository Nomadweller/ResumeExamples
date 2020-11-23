    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
    using System.Threading;
using System.Xml;

namespace AutoSink
    {
        public class Node
        {
            public List<Node> children = new List<Node>();
            public int cityToll;
            public string cityName;
            public int index;
            public bool visited;
            public Node(string cityName, int cityToll, int index)
            {
                this.cityName = cityName;
                this.cityToll = cityToll;
                this.index = index;
            }
        }
        class AutoSink
        {
            static void Main(string[] args)
            {
                //number of nodes
                int v = Int32.Parse(Console.ReadLine());
                string userInput;
                string[] input;
                List<Node> nodes = new List<Node>();
                int[] topSort = new int[v];
                int totalVertices = v - 1;
                bool[] visited = new bool[v];

                //add nodes
                for (int i = 0; i < v; i++)
                {
                    userInput = Console.ReadLine();
                    input = userInput.Split(' ');
                    string cityName =   input[0];
                    int cityToll = Int32.Parse(input[1]);
                    nodes.Add(new Node(cityName, cityToll, i));
                }
                    //edges
                int e = Int32.Parse(Console.ReadLine());
                for (int i = 0; i < e; i++)
                {
                    userInput = Console.ReadLine();
                    input = userInput.Split(' ');
                    string origin = input[0];
                    string destination = input[1];
                    Node currentNode = nodes.Find(x => x.cityName.Equals(origin));
                    Node destNode = nodes.Find(x => x.cityName.Equals(destination));
                    currentNode.children.Add(destNode);
                }

            //PrintGraph(nodes);
                foreach(Node node in nodes)
                {
                    if(!node.visited)
                        TopOrder(node, topSort, nodes, ref totalVertices, visited);
                }
            //trips
                int n = Int32.Parse(Console.ReadLine());
                for (int i = 0; i < n; i++)
                {
                    int[] bestPrices = new int[v];
                    //bool[] visited = new bool[v];
                    userInput = Console.ReadLine();
                    input = userInput.Split(' ');
                    string origin = input[0];
                    string destination = input[1];
                    Node currentNode = nodes.Find(x => x.cityName.Equals(origin));
                    Node destNode = nodes.Find(x => x.cityName.Equals(destination));
                    for (int j = 0; j < bestPrices.Length; j++)
                    {
                        if(j != currentNode.index)
                             bestPrices[j] = int.MaxValue;
                    }
                    if (currentNode.index == destNode.index)
                    {
                        Console.WriteLine(0);
                    }
                    else
                    {
                        for(int j = currentNode.index; j <= destNode.index; j++)
                        {
                            foreach(Node child in nodes[topSort[j]].children)
                            {
                                if (bestPrices[child.index] > bestPrices[j] + child.cityToll && bestPrices[j] != int.MaxValue)
                                    bestPrices[child.index] = bestPrices[j] + child.cityToll;
                            }
                        }
                        if (bestPrices[destNode.index] != int.MaxValue)
                            Console.WriteLine(bestPrices[destNode.index]);
                        else
                            Console.WriteLine("NO");
                        /*Tuple<int, bool> tollPrice_isPath = Djikstras(currentNode, destNode, visited, bestPrices);
                        if(!tollPrice_isPath.Item2)
                        {
                            Console.WriteLine("NO");
                        }
                        else
                        {
                            Console.WriteLine(tollPrice_isPath.Item1);
                        }*/
                    }
                }
            }

            private static Tuple<int, bool> Djikstras(Node startNode, Node destNode, bool[] visited, int[] bestPrices)
            { 
                foreach (Node child in startNode.children)
                {
                    if (bestPrices[child.index] > bestPrices[startNode.index] + child.cityToll /*&& bestPrices[startNode.index] != int.MaxValue*/) 
                    {
                        bestPrices[child.index] = bestPrices[startNode.index] + child.cityToll;
                    }
                }
                visited[startNode.index] = true;
                foreach (Node child in startNode.children)
                    {
                        if (!visited[child.index])
                        {
                            Djikstras(child, destNode, visited, bestPrices);
                        }
                    }
                    Tuple<int, bool> retVal = new Tuple<int, bool>(bestPrices[destNode.index], visited[destNode.index]);
                    return retVal;
                }

        private static void TopOrder(Node start, int[] topSort, List<Node> allNodes, ref int totalVertices, bool[] visited)
        {
            foreach (Node node in start.children)
            {
                if(!node.visited)
                    TopOrder(node, topSort, allNodes, ref totalVertices, visited);
            }
            if (!start.visited)
            {
                topSort[totalVertices] = start.index;
                start.visited = true;
                start.index = totalVertices;
                totalVertices--;
            }
        }

            private static void PrintGraph(List<Node> nodes)
            {
                foreach (Node n in nodes)
                {
                    Console.WriteLine("Node : " + n.cityName);
                    foreach(Node city in n.children)
                    {
                        Console.WriteLine("City: " + city.cityName + " || Toll: " + city.cityToll);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
