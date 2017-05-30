namespace Factorial {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    public static class FactorialMath {
        internal const string MustBeANonNegativeNumber = "Input must be a non-negative number";
        private static readonly int NumberOfLogicalProcessors;

        static FactorialMath() {
            var processorCount = Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS");
            if (processorCount != null) {
                NumberOfLogicalProcessors = int.Parse(processorCount);
            }
        }

        #region Approximation

        /// <summary>
        /// Approximation of n!
        /// </summary>
        /// <remarks>Only Accurate to about 5 decimal places
        /// Only slow because BigInteger.Parse is currently broken
        /// Speed Profiles:
        /// small   : 
        /// medium  : 
        /// large   : 4.703 (4.600 of that is BigInteger.pow workaround)
        /// crazy   : 
        /// </remarks>
        /// <param name="n"></param>
        /// <returns></returns>
        public static BigInteger CalculateApproximateFactorialFromLog(int n) {
            if (n < 0) {
                throw new ArgumentOutOfRangeException(nameof(n), MustBeANonNegativeNumber);
            }

            var t = 0d;
            for (var i = 1; i <= n; i++) {
                t += Math.Log10(i);
            }

            var m = (int)t;
            var f = t - m;

            f = Math.Round(Math.Pow(10, f), 5);

            // var s = f + "E+" + m;
            // return BigInteger.Parse(s, NumberStyles.Any);
            // Workaround because BigInteger.Parse will not parse an exponent above 1000
            // https://github.com/dotnet/corefx/issues/8567
            BigInteger ret;
            if (m > 1000) {
                ret = BigInteger.Parse(f + "E+" + 1000, NumberStyles.Any);
                m -= 1000;
                ret *= BigInteger.Pow(10, m);
            }
            else {
                ret = BigInteger.Parse(f + "E+" + m, NumberStyles.Any);
            }

            return ret;
        }

        #endregion

        /// <summary>
        /// Calculate the factorial of a number.
        /// </summary>
        /// <remarks>Generic Factorial Method using a For Loop.
        /// Speed Profiles:
        /// small   : 4e-6
        /// medium  : 5.53E-06 *fastest*
        /// large   : 1.24E-03 *slowest*
        /// crazy   : 3.029    *slowest* 
        /// insane  : 23.478   *slowest*
        /// </remarks>
        /// <param name="n">Number to generate the factorial of</param>
        public static BigInteger CalculateFactorialForLoop(int n) {
            if (n < 0) {
                throw new ArgumentOutOfRangeException(nameof(n), MustBeANonNegativeNumber);
            }

            BigInteger currentTotal = 1;
            for (var i = 2; i <= n; i++) {
                currentTotal *= i;
            }

            return currentTotal;
        }

        /// <summary>
        /// Calculate the factorial of a number.
        /// </summary>
        /// <remarks>Reduce BigInteger usage by pre-multiplying in small groups.
        /// 40000*39999*39998*39997 falls just inside of ulong.maxValue = 1.8e19 
        /// Speed Profiles:
        /// small   : 3.61e-6 *fastest"
        /// medium  : 1.37E-05
        /// large   : 8.34E-04
        /// crazy   : 2.143
        /// instane : 16.198
        /// </remarks>
        /// <param name="n">Number to generate the factorial of</param>
        public static BigInteger CalculateFactorialForLoopPreMultiply(int n) {
            if (n < 0) {
                throw new ArgumentOutOfRangeException(nameof(n), MustBeANonNegativeNumber);
            }

            var ulongPremultipliedList = new List<ulong>() { 1UL };

            // Calculate the number of groups
            var groupsCount = Math.Ceiling(n / 4f);

            var totalLoopCounter = 0;

            // for each group
            for (var j = 0; j < groupsCount; j++) {
                ulong loopCalculatedValue = 1;

                var innerLoopCounter = 0;
                while (innerLoopCounter < 4 && totalLoopCounter < n) {
                    totalLoopCounter++;
                    loopCalculatedValue *= (ulong)totalLoopCounter;
                    innerLoopCounter++;
                }

                ulongPremultipliedList.Add(loopCalculatedValue);
            }

            var bigIntegerTotal = BigInteger.One;

            for (var i = 0; i < ulongPremultipliedList.Count; i++) {
                bigIntegerTotal = bigIntegerTotal * ulongPremultipliedList[i];
            }

            return bigIntegerTotal;
        }

        /// <summary>
        /// Calculate the factorial of a number.
        /// </summary>
        /// <remarks>Reduce BigInteger usage by pre-multiplying the start and ends of the value list.
        /// 40000*39999*39998*39997*1*2*3*4 falls just inside of ulong.maxValue = 1.8e19 
        /// Speed Profiles:
        /// small   : 4.29e-6
        /// medium  : 3.71E-05
        /// large   : 9.38E-04
        /// crazy   : 2.098
        /// insane  : 15.860
        /// </remarks>
        /// <param name="n">Number to generate the factorial of</param>
        public static BigInteger CalculateFactorialForLoopReducedPreMultiplyEnds(int n) {
            if (n < 0) {
                throw new ArgumentOutOfRangeException(nameof(n), MustBeANonNegativeNumber);
            }

            // Single PreMultiply
            // var ulongPremultipliedList = PreMultiplyUlongList(Enumerable.Range(1, n).Select((i => (ulong)i )).ToList());

            // Double Premultiply - (4 low with 4 high) Any more wull result in errors at high values
            var ulongPremultipliedList =
                PreMultiplyUlongList(PreMultiplyUlongList(Enumerable.Range(1, n).Select(i => (ulong)i).ToList()));

            return ulongPremultipliedList.Aggregate(
                BigInteger.One,
                (current, intermediateValue) => current * intermediateValue);
        }

        /// <summary>
        /// Calculate the factorial of a number.
        /// </summary>
        /// <remarks>Multithreaded version of CalculateFactorialForLoop.
        /// Speed Profiles:
        /// small   : 4.61e-6
        /// medium  : 1.29E-05
        /// large   : 6.05E-04 *fastest*
        /// crazy   : 1.209
        /// insane  : 8.717
        /// </remarks>
        /// <param name="n">Number to generate the factorial of</param>
        public static BigInteger CalculateFactorialParallel(int n) {
            if (n < 0) {
                throw new ArgumentOutOfRangeException(nameof(n), MustBeANonNegativeNumber);
            }

            if (n < 2) {
                return BigInteger.One;
            }

            var threads = n > NumberOfLogicalProcessors ? NumberOfLogicalProcessors : 1;

            var ptasks =
                Enumerable.Range(1, threads)
                    .Select(i => Task.Factory.StartNew(() => CalculatePartialFactorial(i, n, threads)))
                    .ToArray();
            Task.WaitAll(ptasks.Cast<Task>().ToArray());
            var finalResult = BigInteger.One;
            foreach (var partialResult in ptasks.Select(t => t.Result)) {
                finalResult *= partialResult;
            }

            return finalResult;
        }

        /// <summary>
        /// Calculate the factorial of a number.
        /// </summary>
        /// <remarks>Multithreaded version of CalculateFactorialPreMultiplyEnds.
        /// Speed Profiles:
        /// small   : 446e-6
        /// medium  : 1.09E-04
        /// large   : 1.01E-03
        /// crazy   : 0.906     *fastest*
        /// insane  : 7.263     *fastest*
        /// </remarks>
        /// <param name="n">Number to generate the factorial of</param>
        public static BigInteger CalculateFactorialPreMultiplyEndsParallel(int n) {
            if (n < 0) {
                throw new ArgumentOutOfRangeException(nameof(n), MustBeANonNegativeNumber);
            }

            if (n < 2) {
                return BigInteger.One;
            }

            var ulongPremultipliedList =
                PreMultiplyUlongListParallel(
                    PreMultiplyUlongListParallel(Enumerable.Range(1, n).Select(i => (ulong)i).ToList()));

            var threads = n > NumberOfLogicalProcessors ? NumberOfLogicalProcessors : 1;

            var ptasks =
                Enumerable.Range(1, threads)
                    .Select(
                        i =>
                            Task.Factory.StartNew(
                                () => CalculatePartialFactorial(i - 1, ulongPremultipliedList, threads)))
                    .ToArray();

            Task.WaitAll(ptasks.Cast<Task>().ToArray());

            var finalResult = BigInteger.One;

            foreach (var partialResult in ptasks.Select(t => t.Result)) {
                finalResult *= partialResult;
            }

            return finalResult;
        }

        /// <summary>
        /// Calculate the factorial of a number.
        /// </summary>
        /// <remarks>Recursive function.
        /// This will throw stack overflows at high values.
        /// Speed Profiles:
        /// small   : 4.69e-6 *slowest*
        /// medium  : 6.59E-05 *slowest"
        /// large   : 9.85E-04
        /// crazy   : n/a stack overflow
        /// </remarks>
        /// <param name="n">Number to generate the factorial of</param>
        public static BigInteger CalculateFactorialRecursive(int n) {
            if (n < 0) {
                throw new ArgumentOutOfRangeException(nameof(n), MustBeANonNegativeNumber);
            }

            if (n > 15000) {
                throw new ArgumentOutOfRangeException(
                    "15000 is the maximum supported " + nameof(n) + " in the recursive function");
            }

            if (n == 0) {
                return 1;
            }

            return n * CalculateFactorialRecursive(n - 1);
        }

        /// <summary>
        /// Helper function that multiplies every nth (<paramref name="increment"/>) value of an indexed series
        /// </summary>
        /// <param name="startIndex">Index of the first value in the series</param>
        /// <param name="valueList">List of values</param>
        /// <param name="increment">Offset between values</param>
        /// <returns></returns>
        public static BigInteger CalculatePartialFactorial(int startIndex, List<ulong> valueList, int increment = 1) {
            if (startIndex < 0 || valueList.Count < 0 || valueList.Count < startIndex) {
                return 1;
            }

            if (increment < 1) {
                throw new ArgumentException("CalculatePartialFactorial has an illegal increment value");
            }

            BigInteger currentTotal = 1;

            for (var i = startIndex; i < valueList.Count; i += increment) {
                currentTotal *= valueList[i];
            }

            return currentTotal;
        }

        /// <summary>
        /// Helper function that multiplies every nth (<paramref name="increment"/>) value of a integer series
        /// </summary>
        /// <param name="startIndex">First value in the series</param>
        /// <param name="endValue">Last value of the series </param>
        /// <param name="increment">Offset between values</param>
        /// <returns></returns>
        public static BigInteger CalculatePartialFactorial(int startIndex, int endValue, int increment = 1) {
            if (startIndex < 0 || endValue < 0 || endValue < startIndex) {
                throw new ArgumentOutOfRangeException(
                    string.Empty,
                    "Input values are out of range, the first n must be 0 or greater, and the second must be larger than the first");
            }

            if (increment < 1) {
                throw new ArgumentException("CalculatePartialFactorial has an illegal increment value");
            }

            BigInteger currentTotal = 1;
            if (startIndex == 0) {
                startIndex = 1;
            }

            for (var i = startIndex; i <= endValue; i += increment) {
                currentTotal *= i;
            }

            return currentTotal;
        }

        /// <summary>
        /// Helper function to pre-multiply the ends of a list of values.
        /// </summary>
        /// <param name="listOfn">List containing values to multiply together</param>
        /// <returns></returns>
        private static List<ulong> PreMultiplyUlongList(List<ulong> listOfn) {
            // add a missing value from the middle if value is odd so n%2 == 0;
            if (listOfn.Count % 2 == 1) {
                listOfn.Add(1);
            }

            var listOfnCount = listOfn.Count;

            // Get the first half of the list and multiply by the back half;
            return
                listOfn.GetRange(0, listOfnCount / 2).Select((n, i) => n * listOfn[listOfnCount - 1 - i]).ToList();
        }

        /// <summary>
        /// Helper a multithreaded function to pre-multiply the ends of a list of values.
        /// </summary>
        /// <param name="listOfn">List containing values to multiply together</param>
        /// <returns></returns>
        private static List<ulong> PreMultiplyUlongListParallel(List<ulong> listOfn) {
            // add a missing value from the middle if value is odd so n%2 == 0;
            if (listOfn.Count % 2 == 1) {
                listOfn.Add(1);
            }

            var listOfnCount = listOfn.Count;

            // Get the first half of the list and multiply by the back half;
            return
                listOfn.GetRange(0, listOfnCount / 2)
                    .AsParallel()
                    .Select((n, i) => n * listOfn[listOfnCount - 1 - i])
                    .ToList();
        }
    }
}