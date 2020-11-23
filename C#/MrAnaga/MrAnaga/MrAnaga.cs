using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrAnaga
{
    class MrAnaga
    {
        static void Main(string[] args)
        {
            HashSet<int> uniqueWords = new HashSet<int>();
            HashSet<int> rejectedWords = new HashSet<int>();
            string sortedWord;
            int sortedHashCode;
            
            string userInput = Console.ReadLine();
            string[] firstLine = userInput.Split(' ');
            int n = Int32.Parse(firstLine[0]);
            int k = Int32.Parse(firstLine[1]);
            char[] letters = new char[k];

            for(int i = 0; i < n; i++)
            {
                userInput = Console.ReadLine();
                letters = userInput.ToCharArray();
                Array.Sort(letters);
                sortedWord = new string(letters);
                sortedHashCode = sortedWord.GetHashCode();

                if (uniqueWords.Contains(sortedHashCode))
                {
                    uniqueWords.Remove(sortedHashCode);
                    rejectedWords.Add(sortedHashCode);
                }
                else if (rejectedWords.Contains(sortedHashCode))
                {
                    continue;
                }
                else
                {
                    uniqueWords.Add(sortedHashCode);
                }
            }
            Console.WriteLine(uniqueWords.Count());
        }
    }
}
