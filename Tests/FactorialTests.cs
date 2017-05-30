//  --------------------------------------------------------------------------------------------------------------------
//     <copyright file="FactorialTests.cs">
//         Copyright (c) Nathan Bowman. All rights reserved.
//         Licensed under the MIT License. See LICENSE file in the project root for full license information.
//     </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace Factorial.Tests {
    using System;
    using System.Numerics;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class FactorialTests {
        protected const string LargeNumberTestResultE10 = "2.0916924222E+166713";
        protected const string LargeNumberTestResultE5 = "2.09169E+166713";
        protected const string LargeNumberTestResultE50 = "2.09169242221213236332045525676432702648837354438753E+166713";
        protected const int LargeNumberTestValue = 40000;

        // large numbers will take forever
        protected const int SmallSpeedTestItterations = 100000000;
        protected const int SmallSpeedTestMaxValue = 0;
        protected const int SmallSpeedTestMinValue = 100;

        protected Func<int, BigInteger> MethodToTest;

        private int MediumSpeedTestItterations = 100;
        private int MediumSpeedTestMaxValue = 500;
        private int MediumSpeedTestMinValue = 100;

        private int LargeSpeedTestItterations = 10;
        private int LargeSpeedTestMaxValue = 1000;
        private int LargeSpeedTestMinValue = 500;

        private int HugeSpeedTestItterations = 4;
        private int HugeSpeedTestValue = 40000;

        private int InsaneSpeedTestItterations = 1;
        private int InsaneSpeedTestValue = 100000;

        [TestClass()]
        public class CalculateApproximateFactorialFromLog : GeneralFactorialHugeNumber {
            [TestMethod()]
            public override void CorrectLargeNumberPrecision() {
                BigInteger testOutput = 0;
                testOutput = this.MethodToTest(LargeNumberTestValue);
                Assert.AreEqual(LargeNumberTestResultE5, testOutput.ToString("E5"), "Calculation Failed");
            }

            [TestInitialize()]
            public override void InitTest() {
                this.MethodToTest = FactorialMath.CalculateApproximateFactorialFromLog;
            }
        }

        [TestClass()]
        public class CalculateFactorial {
            [TestMethod()]
            public void CalculateCorrectFactorialsWithIntInput() {
                foreach (var methodType in Enum.GetNames(typeof(FactorialProgram.MethodType))) {
                    Assert.AreEqual(
                        1 * 2 * 3 * 4 * 5 * 6 * 7 * 8 * 9 * 10,
                        FactorialProgram.CalculateFactorial(
                            10,
                            (FactorialProgram.MethodType)Enum.Parse(typeof(FactorialProgram.MethodType), methodType)),
                        "Failed on: " + methodType);
                }
            }

            [TestMethod()]
            public void CalculateCorrectFactorialsWithStringInput() {
                foreach (var methodType in Enum.GetNames(typeof(FactorialProgram.MethodType))) {
                    Assert.AreEqual(
                        1 * 2 * 3 * 4 * 5 * 6 * 7 * 8 * 9 * 10,
                        FactorialProgram.CalculateFactorial(
                            "10",
                            (FactorialProgram.MethodType)Enum.Parse(typeof(FactorialProgram.MethodType), methodType)),
                        "Failed on: " + methodType);
                }
            }

            [TestMethod()]
            [ExpectedException(typeof(ArgumentOutOfRangeException))]
            public virtual void InvalidStringInputThrows_ArgumentOutOfRangeException() {
                FactorialProgram.CalculateFactorial("-10", FactorialProgram.MethodType.ForLoop);
                Assert.Fail("Should have thrown an exception");
            }

            [TestMethod()]
            [ExpectedException(typeof(ArgumentOutOfRangeException))]
            public virtual void NegativeNumbersThrows_ArgumentOutOfRangeException_WithIntInput() {
                FactorialProgram.CalculateFactorial(-10, FactorialProgram.MethodType.ForLoop);
                Assert.Fail("Should have thrown an exception");
            }

            [TestMethod()]
            [ExpectedException(typeof(ArgumentOutOfRangeException))]
            public virtual void NegativeNumbersThrows_ArgumentOutOfRangeException_WithStringInput() {
                FactorialProgram.CalculateFactorial("-10", FactorialProgram.MethodType.ForLoop);
                Assert.Fail("Should have thrown an exception");
            }
        }

        [TestClass()]
        public class CalculateFactorialForLoopPreMultiply : GeneralFactorialHugeNumber {
            [TestInitialize()]
            public override void InitTest() {
                this.MethodToTest = FactorialMath.CalculateFactorialForLoopPreMultiply;
            }
        }

        [TestClass()]
        public class CalculateFactorialForLoopReducedPreMultiplyEnds : GeneralFactorialHugeNumber {
            [TestInitialize()]
            public override void InitTest() {
                this.MethodToTest = FactorialMath.CalculateFactorialForLoopReducedPreMultiplyEnds;
            }
        }

        [TestClass()]
        public class CalculateFactorialParallel : GeneralFactorialHugeNumber {
            [TestInitialize()]
            public override void InitTest() {
                this.MethodToTest = FactorialMath.CalculateFactorialParallel;
            }
        }

        [TestClass()]
        public class CalculateFactorialPreMultiplyEndsParallel : GeneralFactorialHugeNumber {
            [TestInitialize()]
            public override void InitTest() {
                this.MethodToTest = FactorialMath.CalculateFactorialPreMultiplyEndsParallel;
            }
        }

        [TestClass()]
        public class CalculateFactorialRecursive : GeneralFactorialTests {
            [TestInitialize()]
            public override void InitTest() {
                this.MethodToTest = FactorialMath.CalculateFactorialRecursive;
            }
        }

        [TestClass()]
        public class CalculatePatialFactorial {
            [TestMethod()]
            public void CorrectBetween0And5() {
                Assert.AreEqual(1 * 2 * 3 * 4 * 5, FactorialMath.CalculatePartialFactorial(0, 5));
            }

            [TestMethod()]
            public void CorrectBetween3And8() {
                Assert.AreEqual(3 * 4 * 5 * 6 * 7 * 8, FactorialMath.CalculatePartialFactorial(3, 8));
            }

            [TestMethod()]
            public void CorrectEveryOtherBetween0And5() {
                Assert.AreEqual(1 * 3 * 5, FactorialMath.CalculatePartialFactorial(0, 5, 2));
            }

            [TestMethod()]
            public void CorrectEveryOtherBetween3And8() {
                Assert.AreEqual(3 * 5 * 7, FactorialMath.CalculatePartialFactorial(3, 8, 2));
            }

            [TestMethod()]
            public void CorrectEveryThirdBetween0And10() {
                Assert.AreEqual(1 * 4 * 7 * 10, FactorialMath.CalculatePartialFactorial(0, 10, 3));
            }

            [TestMethod()]
            public void CorrectEveryThirdBetween3And9() {
                Assert.AreEqual(3 * 6 * 9, FactorialMath.CalculatePartialFactorial(3, 9, 3));
            }

            [TestMethod()]
            [ExpectedException(typeof(ArgumentException))]
            public void InvalidIncrementThrows_ArgumentException() {
                FactorialMath.CalculatePartialFactorial(1, 9, 0);
            }

            [TestMethod()]
            [ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void NegativeInputThrows_ArgumentOutOfRangeException() {
                FactorialMath.CalculatePartialFactorial(-1, 9);
            }

            [TestMethod()]
            [ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void OutOfOrderInputThrows_ArgumentOutOfRangeException() {
                FactorialMath.CalculatePartialFactorial(2, 1);
            }
        }

        [TestClass()]
        public abstract class GeneralFactorialHugeNumber : GeneralFactorialTests {
            [TestMethod()]
            public virtual void CorrectLargeNumberPrecision() {
                BigInteger testOutput = 0;
                testOutput = this.MethodToTest(LargeNumberTestValue);
                Assert.AreEqual(LargeNumberTestResultE50, testOutput.ToString("E50"), "Calculation Failed");
            }

            [TestMethod()]
            public void LargeNumberShouldNotThrowExceptions() {
                try {
                    this.MethodToTest(LargeNumberTestValue);
                }
                catch (Exception) {
                    // This will not trigger on stack overflow as it only triggers on user thrown SO
                    Assert.Fail("Exception Fail");
                }
            }

            [TestMethod()]
            public void SpeedProfilableTestHuge() {
                for (var j = 0; j < this.HugeSpeedTestItterations; j++) {
                    this.MethodToTest(this.HugeSpeedTestValue);
                }
            }

            [TestMethod()]
            public void SpeedProfilableTestInsane() {
                // should prob not run this too often
                for (var j = 0; j < this.InsaneSpeedTestItterations; j++) {
                    this.MethodToTest(this.InsaneSpeedTestValue);
                }
            }
        }

        [TestClass()]
        public abstract class GeneralFactorialTests : FactorialTests {
            [TestMethod()]
            public void CorrectValue0() {
                Assert.AreEqual(1, this.MethodToTest(0));
            }

            [TestMethod()]
            public void CorrectValue1() {
                Assert.AreEqual(1, this.MethodToTest(1));
            }

            [TestMethod()]
            public void CorrectValue2() {
                Assert.AreEqual(1 * 2, this.MethodToTest(2));
            }

            [TestMethod()]
            public void CorrectValue3() {
                Assert.AreEqual(1 * 2 * 3, this.MethodToTest(3));
            }

            [TestMethod()]
            public void CorrectValue8() {
                Assert.AreEqual(1 * 2 * 3 * 4 * 5 * 6 * 7 * 8, this.MethodToTest(8));
            }

            [TestInitialize()]
            public abstract void InitTest();

            [TestMethod()]
            [ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void NegativeInputThrows_ArgumentOutOfRangeException() {
                this.MethodToTest(-1);
            }

            [TestMethod()]
            public void SpeedProfilableTestLarge() {
                for (var j = 0; j < this.LargeSpeedTestItterations; j++)
                for (var i = this.LargeSpeedTestMinValue; i < this.LargeSpeedTestMaxValue; i++) {
                    this.MethodToTest(i);
                }
            }

            [TestMethod()]
            public void SpeedProfilableTestMedium() {
                for (var j = 0; j < this.MediumSpeedTestItterations; j++)
                for (var i = this.MediumSpeedTestMinValue; i < this.MediumSpeedTestMaxValue; i++) {
                    this.MethodToTest(i);
                }
            }

            [TestMethod()]
            public void SpeedProfilableTestSmall() {
                for (var j = 0; j < SmallSpeedTestItterations; j++)
                for (var i = SmallSpeedTestMinValue; i < SmallSpeedTestMaxValue; i++) {
                    this.MethodToTest(i);
                }
            }
        }
    }
}