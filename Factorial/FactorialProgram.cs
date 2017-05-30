namespace Factorial
{
    using System;
    using System.Diagnostics;
    using System.Numerics;

    /*Factorial (n!) is the product of a positive integer and all the positive integers below it. 
     * Negative numbers are another case that i will not investigate here, and 0! = 1
     * 
     * Factorials can grow in size very rapidly, 12! is already bigger than an int.
     * Double will also only store up till 170! but looses precision at 15-16 digits.
     * ulong can store upto 20 digits before overflowing.
     * System.Numerics.BigInteger will store any number, but computations are expensive.
     * 
     * The goal of this excercise will be to find a sufficiently fast way to calculate 40000!
     * 
     * The least complex way to compute n! is to itterate over the preceeding positive integers and
     * multiply by them. The most basic way of doing this is with a generic loop so i'll start there.
     * 
     * This Program contains defines for debug and release versions and offers a diferent menu for each.
     * 
     */

    public class FactorialProgram
    {
        public enum MethodType
        {
            ForLoop = 1,

            ForLoopPreMultiply,

            ForLoopPreMultiplyEnds,

            Parallel,

            ParallelPreMultiplyEnds,

            Recursive,

            LogApproximation
        }

#if DEBUG
        public static BigInteger CalculateFactorial(string sn, MethodType methodtype)
        {
            int n;
            if (!int.TryParse(sn, out n))
            {
                throw new ArgumentOutOfRangeException(nameof(sn), "The value provided is not a valid number");
            }

            return CalculateFactorial(n, methodtype);
        }

#endif

        public static BigInteger CalculateFactorial(int n, MethodType methodType)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), "The number must be positive");
            }

            switch (methodType)
            {
                case MethodType.ForLoop:
                    return FactorialMath.CalculateFactorialForLoop(n);
                case MethodType.ForLoopPreMultiply:
                    return FactorialMath.CalculateFactorialForLoopPreMultiply(n);
                case MethodType.ForLoopPreMultiplyEnds:
                    return FactorialMath.CalculateFactorialForLoopReducedPreMultiplyEnds(n);
                case MethodType.Parallel:
                    return FactorialMath.CalculateFactorialParallel(n);
                case MethodType.ParallelPreMultiplyEnds:
                    return FactorialMath.CalculateFactorialPreMultiplyEndsParallel(n);
                case MethodType.Recursive:
                    return FactorialMath.CalculateFactorialRecursive(n);
                case MethodType.LogApproximation:
                    return FactorialMath.CalculateApproximateFactorialFromLog(n);
                default:
                    throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null);
            }
        }

        private static void CalculateFactorial(string input)
        {
            int n;
            BigInteger result;
            if (!int.TryParse(input, out n))
            {
                throw new ArgumentException("The value provided is not a valid number");
            }

            Console.WriteLine("Processing...");
            var stopwatch = Stopwatch.StartNew();
            if (n > 40000)
            {
                Console.WriteLine("Values above 40000 are approximated to 5 decimal places");
                result = CalculateFactorial(n, MethodType.LogApproximation);
            }
            else if (n > 1000)
            {
                result = CalculateFactorial(n, MethodType.ParallelPreMultiplyEnds);
            }
            else if (n > 500)
            {
                result = CalculateFactorial(n, MethodType.Parallel);
            }
            else if (n > 100)
            {
                result = CalculateFactorial(n, MethodType.ForLoopPreMultiply);
            }
            else
            {
                result = CalculateFactorial(n, MethodType.ForLoop);
            }

            var elapsedMethod = stopwatch.ElapsedMilliseconds;
            var outputString = $"{input}! = {result.ToString("E15")} in {elapsedMethod}ms";
            if (result > 0)
            {
                Console.Clear();
                Console.WriteLine(outputString);
                Console.WriteLine($"Calculation took: {elapsedMethod}ms");
                stopwatch.Stop();
                Console.WriteLine($"Display took: {stopwatch.ElapsedMilliseconds}ms");
            }
            else
            {
                Console.WriteLine("Function returned an invalid result");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void Main(string[] args)
        {
            Console.Title = "Factorial Calculator";

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Please input a number.");
                var input = Console.ReadLine();

#if DEBUG
                var returnToMainFlag = false;
                while (!returnToMainFlag)
                {
                    Console.Clear();
                    Console.Write(
                        "Choose the prefered method call\n" + "\n" + "1) For Loop\n" + "2) For Loop - Pre Multiplied\n" + "3) For Loop - Pre Multiplied Ends\n"
                        + "4) Parallel For Loops\n" + "5) Parallel For Loops - Pre Multiplied Ends\n" + "6) Recursive\n"
                        + "7) Logarithmic Approximation - Accurate to 5 decimal places.\n" + "0) Return to start screen" + "\n");
                    var choice = Console.ReadKey().Key;
                    var stopwatch = new Stopwatch();
                    BigInteger result = 0;

                    try
                    {
                        switch (choice)
                        {
                            case ConsoleKey.D0:
                                returnToMainFlag = true;
                                break;
                            case ConsoleKey.D1:
                                stopwatch.Reset();
                                stopwatch.Start();
                                result = CalculateFactorial(input, MethodType.ForLoop);
                                stopwatch.Stop();
                                break;
                            case ConsoleKey.D2:
                                stopwatch.Reset();
                                stopwatch.Start();
                                result = CalculateFactorial(input, MethodType.ForLoopPreMultiply);
                                stopwatch.Stop();
                                break;
                            case ConsoleKey.D3:
                                stopwatch.Reset();
                                stopwatch.Start();
                                result = CalculateFactorial(input, MethodType.ForLoopPreMultiplyEnds);
                                stopwatch.Stop();
                                break;
                            case ConsoleKey.D4:
                                stopwatch.Reset();
                                stopwatch.Start();
                                result = CalculateFactorial(input, MethodType.Parallel);
                                stopwatch.Stop();
                                break;
                            case ConsoleKey.D5:
                                stopwatch.Reset();
                                stopwatch.Start();
                                result = CalculateFactorial(input, MethodType.ParallelPreMultiplyEnds);
                                break;
                            case ConsoleKey.D6:
                                stopwatch.Reset();
                                stopwatch.Start();
                                result = CalculateFactorial(input, MethodType.Recursive);
                                stopwatch.Stop();
                                break;
                            case ConsoleKey.D7:
                                stopwatch.Reset();
                                stopwatch.Start();
                                result = CalculateFactorial(input, MethodType.LogApproximation);
                                stopwatch.Stop();
                                break;
                            default:
                                continue;
                        }

                        Console.Clear();
                        if (result > 0)
                        {
                            Console.WriteLine(input + "! = " + result.ToString("E15") + " in " + stopwatch.ElapsedMilliseconds + "ms");
                        }
                        else
                        {
                            Console.WriteLine("Function returned an invalid result");
                        }

                        Console.WriteLine("\n\nPress any key to continue...");
                        Console.ReadKey();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("\n" + e.Message);
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        break;
                    }
                }

#endif

#if !DEBUG
                try {
                    CalculateFactorial(input);
                }
                catch (Exception e) {
                    Console.WriteLine("\n" + e.Message);
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }

#endif
            }
        }
    }
}