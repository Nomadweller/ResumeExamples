using System;

namespace PlantingTrees
{
    class Program
    {
        static void Main(string[] args)
        {
            //reading user input
            int n = Convert.ToInt32(Console.ReadLine());
            int[] numbers = new int[n];
            string userInput = Console.ReadLine();
            string[] input = userInput.Split(' ');
            //this is borrowed from stack overflow post to basically convert the line of user input into a int array based upon the spaces in the line. 
            numbers = Array.ConvertAll(input, s => int.Parse(s));
            //int[] numbers = {39, 38, 9, 35, 39, 20};

            //declaring variables
            var rand = new Random();
            int length = numbers.Length;
            int plantingDay = 1;
            int minAmount = 0;
            //sort array
            MySort(numbers, 0, length - 1, rand);

            //iterate through array to find days til party
            foreach (int number in numbers)
            {
                int daysToMature = number + plantingDay;
                if (daysToMature > minAmount)
                {
                    minAmount = daysToMature;
                }
                plantingDay++;
            }
            Console.WriteLine(minAmount + 1);
            //foreach (int number in numbers)
            //{
            //    Console.WriteLine(number);
            //}
            Console.ReadLine();
        }
        /// <summary>
        /// My Sort implementation of Quick Sort Algorithm. The average complexity is nlogn while it's worst case is n^2. To mitigate worst
        /// case scenario this algorithm picks a pivot point at random each time it gets called. This was implemented after reviewing a video
        /// on quick sort https://www.youtube.com/watch?v=wygsfgtpApI but has my own code which varies from the one in video but has a similar 
        /// structure.
        /// </summary>
        /// <param name="numbers">Array that you wish to sort</param>
        /// <param name="start">Start index of Array</param>
        /// <param name="end">Ending index of array</param>
        /// <param name="rand">Random class to pass. Stack Overflow exception were happening when trying to create Random in recursive function.</param>
        private static void MySort(int[] numbers, int start, int end, Random rand)
        {
            int leftPointer = start;
            int rightPointer = end;
            int pivotLocation = rand.Next(start, end);
            var pivot = numbers[pivotLocation];

            while (leftPointer <= rightPointer)
            {
                while (numbers[leftPointer] > pivot)
                {
                    leftPointer++;
                }
                while (numbers[rightPointer] < pivot)
                {
                    rightPointer--;
                }
                if (leftPointer <= rightPointer)
                {
                    swap(ref numbers[leftPointer], ref numbers[rightPointer]);
                    leftPointer++;
                    rightPointer--;
                }
            }
            if (start < rightPointer)
            {
                MySort(numbers, start, rightPointer, rand);
            }
            if (leftPointer < end)
            {
                MySort(numbers, leftPointer, end, rand);
            }
        }
        public static void swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
    }
}