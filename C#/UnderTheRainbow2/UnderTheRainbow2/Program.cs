using System;
using System.Collections.Generic;

namespace UnderTheRainbow
{
    class Program
    {
        public static void Print2DArray<T>(T[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }
        private static double DoMath(double bigNumber, double smallNumber)
        {
            double retVal = 400 - (bigNumber - smallNumber);
            retVal *= retVal;
            return retVal;
        }

        static void Main(string[] args)
        {
            double[] numbers = new double[] { 0, 300, 500, 600, 700, 900, 1000 };
            double[] minPenValues = new double[numbers.Length - 1];
            int[,] routes = new int[numbers.Length - 1, numbers.Length - 1];
            int j = 1;

            for (int i = 0; i < numbers.Length - 1; i++)
            {
                j = 1;
                minPenValues[i] = double.MaxValue;
                int index = 0;
                while (DoMath(numbers[i + j], numbers[i]) <= minPenValues[i])
                {
                    if (i + j > numbers.Length - 1)
                        break;
                    double bigNumber = numbers[i + j];
                    double smallNumber = numbers[i];
                    routes[j + i, i] = 1;
                    double currentPenalty = DoMath(bigNumber, smallNumber);
                    if (minPenValues[i] > currentPenalty)
                        minPenValues[i] = currentPenalty;
                    j++;
                    if (i + j > numbers.Length - 1)
                        break;
                }
            }
            Print2DArray(routes);
        }

        /*   int j = 1;
           double penalty = double.MaxValue;
           while (Math.Pow(400 - (numbers[i + j] - numbers[i]), 2) <= penalty)
           {
               penalty = Math.Pow(400 - (numbers[i + j] - numbers[i]), 2);
               //penaltyValues[j, i] = Math.Abs(400 - (numbers[i + j] - numbers[i]));
               penaltyValues[j + i, i] = penalty;
               if (i + j >= numbers.Length - 1)
                   break;
               j++;
          */
    }
}

