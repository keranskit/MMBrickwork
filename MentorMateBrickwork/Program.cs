using System;
using System.Collections.Generic;

namespace MentorMateBrickwork
{
    public class Program
    {

        static void Main(string[] args)
        {
            int[] validatedResult = ValidateInputForWall();
            int n = validatedResult[0];
            int m = validatedResult[1];

            int[,] firstLayer = new int[n, m];
            try
            {
                firstLayer = ValidateInputForRowsAndCols(n, m);
            }
            catch (InvalidOperationException)
            {
                firstLayer = ValidateInputForRowsAndCols(n, m);
            }

            int[,] secondLayer = CreateSecondLayer(n, m, firstLayer);

            DrawTheFinalPicture(n, m, secondLayer);

        }

        //Validates the input for the wall dimensions
        public static int[] ValidateInputForWall()
        {
            int n = 0;
            int m = 0;
            Console.WriteLine("Enter the width and thickness of the wall, separated with a space.");
            string[] input = Console.ReadLine().Split(" ");
            while (!(Int32.TryParse(input[0], out n) && Int32.TryParse(input[1], out m)) && n < 2 || n > 100 || m < 2 || m > 100 || n % 2 != 0 || m % 2 != 0)
            {
                Console.WriteLine("Wall thickness shoud be a even number between 1 and 100");
                Console.WriteLine("Please try again...");
                input = Console.ReadLine().Split(" ");
            }
            int[] result = new int[] { n, m };
            return result;
        }


        //Validates the correct input for the first layer of the wall
        public static int[,] ValidateInputForRowsAndCols(int n, int m)
        {
            int[,] firstLayer = new int[n, m];
            Console.WriteLine("Enter each row of the wall");

            try
            {
                for (int i = 0; i < n; i++)
                {
                    string[] inputInt = Console.ReadLine().Split(" ");
                    for (int j = 0; j < m; j++)
                    {
                        firstLayer[i, j] = int.Parse(inputInt[j]);
                    }
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("You are using some strange bricks. We allow only bricks made out of integers. Please try again...");
                throw new InvalidOperationException();
            }

            Dictionary<int, int> dict = new Dictionary<int, int>();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (dict.ContainsKey(firstLayer[i, j]))
                    {
                        dict[firstLayer[i, j]]++;
                    }
                    else
                    {
                        dict.Add(firstLayer[i, j], 1);
                    }
                }
            }
            foreach (var item in dict)
            {
                if (item.Value != 2)
                {
                    Console.WriteLine("You are using some strange bricks. We allow only 1x2 rectangular bricks. Lets start over...");
                    throw new InvalidOperationException();
                }
            }
            if (dict.Count != n * m / 2)
            {
                Console.WriteLine("");
                throw new InvalidOperationException();
            }

            ValidateProperPositioningOfBricks(n, m, firstLayer);

            return firstLayer;
        }


        //Validates the correct positioning of bricks and their size
        public static bool ValidateProperPositioningOfBricks(int n, int m, int[,] firstLayer)
        {
            int[,] firstLayerExpanded = new int[n + 2, m + 2];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    firstLayerExpanded[i + 1, j + 1] = firstLayer[i, j];
                }
            }

            Dictionary<int, bool> dict = new Dictionary<int, bool>();
            for (int i = 0; i < n + 2; i++)
            {
                for (int j = 0; j < m + 2; j++)
                {
                    int current = firstLayerExpanded[i, j];
                    if (current == 0)
                    {
                        continue;
                    }
                    if (!dict.ContainsKey(current))
                    {
                        dict.Add(current, false);
                    }
                    if (dict.ContainsKey(current))
                    {
                        if (current == firstLayerExpanded[i, j + 1]
                            || current == firstLayerExpanded[i + 1, j]
                            || current == firstLayerExpanded[i, j - 1]
                            || current == firstLayerExpanded[i - 1, j])
                        {
                            dict[current] = true;
                        }
                    }
                }
            }
            if (dict.ContainsValue(false))
            {
                Console.WriteLine("Your bricks look very damaged. Lets try again...");
                throw new InvalidOperationException();
            }
            return true;
        }


        //Generates the second layer of the wall
        public static int[,] CreateSecondLayer(int n, int m, int[,] firstLayer)
        {
            int[,] layerTwo = new int[n, m];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    layerTwo[i, j] = 0;
                }
            }

            int counter = 1;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (j + 1 >= m && layerTwo[i, j] != 0)
                    {
                        continue;
                    }
                    if (j + 1 >= m && layerTwo[i, j] == 0)
                    {
                        layerTwo[i, j] = counter;
                        layerTwo[i + 1, j] = counter;
                        counter++;
                        continue;
                    }
                    if (firstLayer[i, j] == firstLayer[i, j + 1] && layerTwo[i, j] == 0)
                    {
                        layerTwo[i, j] = counter;
                        layerTwo[i + 1, j] = counter;
                        counter++;
                    }
                    else if (firstLayer[i, j] != firstLayer[i, j + 1] && layerTwo[i, j] == 0)
                    {
                        layerTwo[i, j] = counter;
                        layerTwo[i, j + 1] = counter;
                        counter++;
                        j++;
                    }
                }
            }

            //Prints the second layer
            Console.WriteLine("This is how the second layer looks like:");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write(layerTwo[i, j] + " ");
                }
                Console.WriteLine();
            }

            return layerTwo;
        }

        public static void DrawTheFinalPicture(int n, int m, int[,] secondFloor)
        {
            var secondLayer = secondFloor;
            string[,] finalPicture = new string[(n * 2) + 1, (m * 2) + 1];
            int i2 = 0;
            int j2 = 0;

            //Insert the asterixes on the empty spaces
            for (int i = 0; i < n * 2 + 1; i++)
            {
                for (int j = 0; j < m * 2 + 1; j++)
                {
                    if (i % 2 == 0)
                    {
                        finalPicture[i, j] = "*";
                    }
                    else
                    {
                        if (j % 2 == 0)
                        {
                            finalPicture[i, j] = "*";
                        }
                        else
                        {
                            finalPicture[i, j] = secondLayer[i2, j2].ToString();
                            j2++;
                        }

                    }
                }
                j2 = 0;
                if (!(i % 2 == 0))
                {
                    i2++;
                }
            }

            //Replace asterix inside the bricks with a dash
            for (int i = 1; i < n * 2 + 1; i+=2)
            {
                for (int j = 1; j < m * 2 + 1; j+=2)
                {
                    string current = finalPicture[i, j];
                    if (j < m * 2-1)
                    {
                        if (finalPicture[i, j] == finalPicture[i, j + 2])
                        {
                            finalPicture[i, j + 1] = "-";
                        }
                    }
                    if (i< n * 2-1)
                    {
                        if (finalPicture[i, j] == finalPicture[i + 2, j])
                        {
                            finalPicture[i + 1, j] = "-";
                        }
                    }
                    current = string.Empty;
                }
            }

            //Prints on the console the final result
            Console.WriteLine("This is the final result.");
            Console.WriteLine("The bricks are surrounded by asterixes and there is a dash in the middle of each brick.");
            for (int i = 0; i < n * 2 + 1; i++)
            {
                for (int j = 0; j < m * 2 + 1; j++)
                {
                    Console.Write(finalPicture[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}
