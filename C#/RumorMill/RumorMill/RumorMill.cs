using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RumorMill
{
    class RumorMill
    {
        public class Node : IComparable<Node>
        {
            public SortedSet<Node> friends = new SortedSet<Node>();
            //public List<Node> friends = new List<Node>();
            public string name;
            public int index;

            public Node(string name, int index)
            {
                this.name = name;
                this.index = index;
            }
            public int CompareTo(Node other)
            {
                return this.index.CompareTo(other.index);
            }
        }
        static void Main(string[] args)
        {
            Dictionary<string, Node> nodes= new Dictionary<string, Node>();
            int n = Int32.Parse(Console.ReadLine());
            List<string> names = new List<string>();
            for(int i = 0; i < n; i++)
            {
                string studentName = Console.ReadLine();
                names.Add(studentName);
            }
            names.Sort();
            for(int i = 0; i < names.Count; i++)
            {
                Node newNode = new Node(names[i], i);
                nodes.Add(names[i], newNode);
            }
            int f = Int32.Parse(Console.ReadLine());
            for (int i = 0; i < f; i++)
            {
                string userInput = Console.ReadLine();
                string[] friendPair = userInput.Split(" ");
                Node personOne = nodes[friendPair[0]];
                Node personTwo = nodes[friendPair[1]];
                personOne.friends.Add(personTwo);
                personTwo.friends.Add(personOne);
            }

            int r = Int32.Parse(Console.ReadLine());
            for (int i = 0; i < r; i++)
            {
                string rumorStarter = Console.ReadLine();
                Node startPerson = nodes[rumorStarter];
                SortedSet<string> friendless = new SortedSet<string>();
                //List<string> friendless = new List<string>();
                bool[] visited = new bool[n];

                //work for the first person
                Console.Write(startPerson.name + " ");
                visited[startPerson.index] = true;

                //call to get rest of students
                FindRumorPath(startPerson, visited);
                //getting students out of the rumormill
                for (int j = 0; j < visited.Length; j++)
                {
                    if(!visited[j])
                    {
                        friendless.Add(names[j]);
                    }
                }
                //friendless.Sort();
                //sorting of students that didnt have friends in rumormill
                foreach(string friend in friendless)
                {
                    Console.Write(friend + " ");
                }
                Console.WriteLine();
            }
        }

        private static void FindRumorPath(Node startPerson, bool[] visited)
        {
            SortedSet<Node> friendsToVisitToday = new SortedSet<Node>();
            int count = 1;
            foreach (Node friend in startPerson.friends)
            {
                friendsToVisitToday.Add(friend);
            }
            while(friendsToVisitToday.Count > 0)
            {
                SortedSet<Node> friendsToVisitTomorrow = new SortedSet<Node>();
                //friendsToVisitToday.Sort((a, b) => a.name.CompareTo(b.name));
                foreach (Node friend in friendsToVisitToday)
                {
                    if (!visited[friend.index])
                    {
                        visited[friend.index] = true;
                        Console.Write(friend.name + " ");
                        count++;
                    }
                    if (count == visited.Length)
                        break;
                    foreach (Node friendOfFriend in friend.friends)
                    {
                        if (!visited[friendOfFriend.index])
                        {
                            friendsToVisitTomorrow.Add(friendOfFriend);
                        }
                    }
                }
                friendsToVisitToday = friendsToVisitTomorrow;
            }
        }
    }
}
