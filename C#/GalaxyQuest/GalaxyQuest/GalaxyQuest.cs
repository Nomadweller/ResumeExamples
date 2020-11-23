using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GalaxyQuest
{
    class GalaxyQuest
    {
        public interface Galaxies {

        }

        public class Galaxy : Galaxies
        {
            public long  X;
            public long  Y;
        }

        private static bool IsWithinGalaxy(long x1, long y1, long x2, long y2, long distance)
        {
            x1 -= x2;
            y1 -= y2;
            x1 *= x1;
            y1 *= y1;
            distance *= distance;
            if (x1 + y1 <= distance)
                return true;
            return false;
        }

        static void Main(string[] args)
        {
            string userInput = Console.ReadLine();
            string[] input = userInput.Split(' ');
            long[] inputs = new long[1];
            inputs = Array.ConvertAll(input, s => long.Parse(s));
            long distance = inputs[0];
            long starCount = inputs[1];
            Galaxy[] galaxyList = new Galaxy[starCount];
            Galaxy majorityGalaxy = new Galaxy();
            int majorityCount = 1;
            int totalCount = 0;

            for (int i = 0; i < starCount; i++)
            {
                Galaxy currentGalaxy = new Galaxy();
                userInput = Console.ReadLine();
                input = userInput.Split(' ');
                inputs = Array.ConvertAll(input, s => long.Parse(s));


                if (i == 0)
                {
                    majorityGalaxy.X = inputs[0];
                    majorityGalaxy.Y = inputs[1];
                    galaxyList[i] = majorityGalaxy;
                }
                else
                {
                    currentGalaxy.X = inputs[0];
                    currentGalaxy.Y = inputs[1];
                    galaxyList[i] = currentGalaxy;
                }
                if(IsWithinGalaxy(currentGalaxy.X, currentGalaxy.Y, majorityGalaxy.X, majorityGalaxy.Y, distance))
                {
                    majorityCount++;
                }
                else
                {
                    majorityCount--;
                    if(majorityCount == 0)
                    {
                        majorityGalaxy = currentGalaxy;
                        majorityCount = 1;
                    }
                }
            }
            foreach (Galaxy galaxy in galaxyList)
            {
                if(IsWithinGalaxy(majorityGalaxy.X, majorityGalaxy.Y, galaxy.X, galaxy.Y, distance))
                {
                    totalCount++;
                }
            }
                if (totalCount > starCount/2)
                Console.WriteLine(totalCount);
            else
                Console.WriteLine("NO");
        }

    }
}
