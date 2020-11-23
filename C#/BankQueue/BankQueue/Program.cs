using System;

namespace BankQueue
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

        
        static void Main(string[] args)
        {
            string userInput = Console.ReadLine();
            string[] input = userInput.Split(' ');
            int numberOfPeople = Int32.Parse(input[0]);
            int totalTime = Int32.Parse(input[1]);
            int[,] peopleArray = new int[2, numberOfPeople];
            int[] bestValue = new int[totalTime];
            //create initial array
            for (int i = 0; i < numberOfPeople; i++)
            {
                userInput = Console.ReadLine();
                input = userInput.Split(' ');
                peopleArray[0, i] = Int32.Parse(input[1]); //time
                peopleArray[1, i] = Int32.Parse(input[0]); //price
            }

            //sort the array
            int sum = 0;
            //iterate through array to find maximum value
            for(int i = 0; i < numberOfPeople; i++)
            {
                ChangeBestValue(bestValue, peopleArray[0, i], peopleArray[1, i], ref sum);
            }
            Print2DArray(peopleArray);

            for(int i = 0; i < bestValue.Length; i++)
            {
                Console.WriteLine(i + ": " + bestValue[i]);
            }

            Console.Write(sum);
            //Console.ReadLine();
        }
        public static void ChangeBestValue(int[] bestValueArray, int currentIndex, int currentPrice, ref int sum)
        {
            if (currentIndex >= 0)
            {
                if (currentPrice > bestValueArray[currentIndex])
                {
                    sum += currentPrice - bestValueArray[currentIndex];
                    if(bestValueArray[currentIndex] > 0)
                    {
                        ChangeBestValue(bestValueArray, currentIndex, bestValueArray[currentIndex], ref sum);
                    }
                    bestValueArray[currentIndex] = currentPrice;
                }
                else
                {
                    ChangeBestValue(bestValueArray, currentIndex - 1, currentPrice, ref sum);
                }
            }
        }

    }
}
