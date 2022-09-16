/// This test is made by SangYoon Cho for testing Formula class

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {

        // ########################### TEST SINGLE VARIABLE AND NUM ###########################  

        [TestMethod]
        [TestCategory("1")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            Formula f = new Formula("");
        }

        [TestMethod]
        [TestCategory("1")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNull()
        {
            Formula f = new Formula(null);
        }

        [TestMethod]
        [TestCategory("2")]
        public void TestSingleNum()
        {
            Formula f = new Formula("1");

            Assert.AreEqual("1", f.ToString());
        }

        [TestMethod]
        [TestCategory("3")]
        public void TestSingleVar()
        {
            Formula f = new Formula("x1");

            Assert.AreEqual("x1", f.ToString());
        }

        [TestMethod]
        [TestCategory("3")]
        public void TestSingleVar2()
        {
            Formula f = new Formula("x1", s => s.ToUpper(), s => ( s == "X1"));
            HashSet<string> testSet = new HashSet<string>() { "X" };

            Assert.AreEqual("X1", f.ToString());
            CollectionAssert.Equals(testSet, f.GetVariables());
        }

        [TestMethod]
        [TestCategory("3")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleVar3()
        {
            Formula f = new Formula("1x");
        }

        [TestMethod]
        [TestCategory("3")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleVar4()
        {
            Formula f = new Formula("+");
        }

        [TestMethod]
        [TestCategory("4")]
        public void TestCompareSingleVar1()
        {
            Formula f1 = new Formula("1");
            Formula f2 = new Formula("1");

            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod]
        [TestCategory("4")]
        public void TestCompareSingleVar2()
        {
            Formula f1 = new Formula("1");
            Formula f2 = new Formula("1");

            Assert.IsTrue(f1 == f2);
        }

        [TestMethod]
        [TestCategory("4")]
        public void TestCompareSingleVar3()
        {
            Formula f1 = new Formula("1");
            Formula f2 = new Formula("2");

            Assert.IsFalse(f1.Equals(f2));
        }

        [TestMethod]
        [TestCategory("4")]
        public void TestCompareSingleVar4()
        {
            Formula f1 = new Formula("1");
            Formula f2 = new Formula("2");

            Assert.IsTrue(f1 != f2);
        }

        [TestMethod]
        [TestCategory("4")]
        public void TestCompareSingleVar5()
        {
            Formula f1 = new Formula("1");
            Formula f2 = new Formula("1.0");

            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod]
        [TestCategory("4")]
        public void TestCompareSingleVar6()
        {
            Formula f1 = new Formula("1");
            Formula f2 = new Formula("1.0");

            Assert.IsTrue(f1.GetHashCode().Equals(f2.GetHashCode()));
        }



        // ########################### TEST SINGLE FORMULA ###########################  

        [TestMethod]
        [TestCategory("5")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOneFormula()
        {
            Formula f = new Formula("1+x", s => s.ToUpper(), s => false);
        }

        [TestMethod]
        [TestCategory("5")]
        public void TestOneFormula2()
        {
            Formula f = new Formula("1+x", s => s, s => true);

            Assert.AreEqual("1+x", f.ToString());
        }

        [TestMethod]
        [TestCategory("5")]
        public void TestOneFormula3()
        {
            Formula f = new Formula("1  +  x", s => s, s => true);

            Assert.AreEqual("1+x", f.ToString());
        }

        [TestMethod]
        [TestCategory("5")]
        public void TestOneFormula4()
        {
            Formula f = new Formula("1+x", s => s, s => ( s == "x" ));

            Assert.AreEqual("1+x", f.ToString());
        }

        [TestMethod]
        [TestCategory("5")]
        public void TestOneFormula5()
        {
            Formula f = new Formula("1+x", s => s.ToUpper(), s => (s == "X"));

            Assert.AreEqual("1+X", f.ToString());
        }

        [TestMethod]
        [TestCategory("5")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOneFormula6()
        {
            Formula f = new Formula("1+1x", s => s.ToUpper(), s => (s == "x"));
        }

        [TestMethod]
        [TestCategory("5")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOneFormula7()
        {
            Formula f = new Formula("1+x1", s => s.ToUpper(), s => (s == "y1"));
        }

        [TestMethod]
        [TestCategory("5")]
        public void TestOneFormula8()
        {
            Formula f = new Formula("1+x1", s => s.ToUpper(), s => (s == "X1"));

            Assert.AreEqual(2.0, f.Evaluate(s => 1));
        }

        [TestMethod]
        [TestCategory("5")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOneFormula9()
        {
            Formula f = new Formula("1++x", s => s.ToUpper(), s => false);
        }

        [TestMethod]
        [TestCategory("5")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOneFormula10()
        {
            Formula f = new Formula("1 x", s => s.ToUpper(), s => false);
        }

        [TestMethod]
        [TestCategory("6")]
        public void TestOneFormula_GetVariable()
        {
            Formula f = new Formula("1+x", s => s.ToUpper(), s => (s == "X"));
            HashSet<string> testSet = new HashSet<string>() { "X" };

            CollectionAssert.Equals(testSet, f.GetVariables());
        }

        [TestMethod]
        [TestCategory("7")]
        public void TestOneFormula_Equals()
        {
            Formula f1 = new Formula("1+x", s => s.ToUpper(), s => (s == "X"));
            Formula f2 = new Formula("1+x", s => s.ToUpper(), s => (s == "X"));

            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod]
        [TestCategory("7")]
        public void TestOneFormula_Equals2()
        {
            Formula f1 = new Formula("1+x", s => s.ToUpper(), s => (s == "X"));
            Formula f2 = new Formula("1+x", s => s.ToUpper(), s => (s == "X"));

            Assert.IsTrue(f1.GetHashCode().Equals(f2.GetHashCode()));
        }

        [TestMethod]
        [TestCategory("8")]
        public void TestOneFormula_Div0()
        {
            Formula f1 = new Formula("1/0", s => s, s => (s == "X"));

            Assert.IsInstanceOfType(f1.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod]
        [TestCategory("8")]
        public void TestOneFormula_Div0_2()
        {
            Formula f1 = new Formula("1/x1", s => s, s => ( s == "x1"));

            Assert.IsInstanceOfType(f1.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod]
        [TestCategory("8")]
        public void TestOneFormula_NAME_3()
        {
            Formula f1 = new Formula("1/x1", s => s, s => ( s == "x1" ));

            Assert.IsInstanceOfType(f1.Evaluate(s => throw new ArgumentException("Undefined Variable.")), typeof(FormulaError));
        }

        [TestMethod]
        [TestCategory("8")]
        public void TestOneFormula_NAME_4()
        {
            Formula f1 = new Formula("1/_x1", s => s.ToUpper(), s => true);

            Assert.IsInstanceOfType(f1.Evaluate(s => throw new ArgumentException("Undefined Variable.")), typeof(FormulaError));
        }

        [TestMethod]
        [TestCategory("8")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOneFormula_NAME_5()
        {
            Formula f1 = new Formula("1/1_x1", s => s.ToUpper(), s => true);
        }

        [TestMethod]
        [TestCategory("8")]
        public void TestOneFormula_NAME_6()
        {
            Formula f1 = new Formula("1/x_1", s => s.ToUpper(), s => true);
            
            Assert.IsInstanceOfType(f1.Evaluate(s => throw new ArgumentException("Undefined Variable.")), typeof(FormulaError));
        }

        // ########################### TEST MULTIPLE FORMULA ###########################  

        [TestMethod]
        [TestCategory("9")]
        public void TestMultiFormula()
        {
            Formula f = new Formula("1+X-y*z", s => s, s => true);

            Assert.AreEqual("1+X-y*z", f.ToString());
        }

        [TestMethod]
        [TestCategory("9")]
        public void TestMultiFormula2()
        {
            Formula f = new Formula("1+x-y*z", s => s.ToUpper(), s => true);

            Assert.AreEqual("1+X-Y*Z", f.ToString());
        }

        [TestMethod]
        [TestCategory("9")]
        public void TestMultiFormula3()
        {
            Formula f = new Formula("1+x-y*z", s => s.ToUpper(), s => true);

            Assert.AreEqual(1.0, f.Evaluate(s => (s == "Y" ? 1 : 2)));
        }

        [TestMethod]
        [TestCategory("9")]
        public void TestMultiFormula4()
        {
            Formula f = new Formula("1e+2+x-y*z", s => s.ToUpper(), s => true);

            Assert.AreEqual(100.0, f.Evaluate(s => (s == "Y" ? 1 : 2)));
        }

        [TestMethod]
        [TestCategory("9")]
        public void TestMultiFormula_GetVar()
        {
            Formula f = new Formula("1+x-y*z", s => s.ToUpper(), s => true);
            HashSet<string> testSet = new HashSet<string>();
            testSet.Add("X"); testSet.Add("Y"); testSet.Add("Z");

            CollectionAssert.Equals(testSet, f.GetVariables());
        }

        [TestMethod]
        [TestCategory("10")]
        public void TestMultiFormula_Equal()
        {
            Formula f = new Formula("1+x", s => s, s => true);
            Formula f2 = new Formula("1+x-y*z", s => s, s => true);

            Assert.IsFalse(f.Equals(f2));
        }

        [TestMethod]
        [TestCategory("10")]
        public void TestMultiFormula_Equal2()
        {
            Formula f1 = new Formula("1e+2+x-y*z", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("100.0+x-y*z", s => s.ToUpper(), s => true);

            CollectionAssert.Equals(f1, f2);
        }

        [TestMethod]
        [TestCategory("10")]
        public void TestMultiFormula_Div()
        {
            Formula f = new Formula("(1+x-y)/(x-2*z)", s => s, s => true);

            Assert.AreEqual(4.0, f.Evaluate(s => (s == "x" ? 5 : 2)));
        }

        [TestMethod]
        [TestCategory("10")]
        public void TestMultiFormula_Div0()
        {
            Formula f = new Formula("(1+x-y)/(x-2*z)", s => s, s => true);

            Assert.IsInstanceOfType(f.Evaluate(s => (s == "x" ? 4 : 2)), typeof(FormulaError));
        }

        [TestMethod]
        [TestCategory("11")]
        public void TestMultiFormulaDuplicate()
        {
            Formula f = new Formula("4*x+2+x-x*x", s => s.ToUpper(), s => true);
            HashSet<string> testSet = new HashSet<string>();
            testSet.Add("X");

            CollectionAssert.Equals(testSet, f.GetVariables());
            Assert.AreEqual(8.0, f.Evaluate(s => 3));
        }

        [TestMethod]
        [TestCategory("12")]
        public void TestComplicateFormula()
        {
            Formula f = new Formula("(1+x-4)*(x-2*50)+34-x*8+1e+2", s => s, s => true);

            Assert.AreEqual(6.0, f.Evaluate(s => 4));
        }

        [TestMethod]
        [TestCategory("13")]
        public void TestComplicateFormula2()
        {
            Formula f = new Formula("(((5.19293 + 3.1493)*4.3595+x/y-4.5694*x)-y)+(1.5547*3-7.3242)", s => s, s => true);
            object actual = f.Evaluate(s => (s == "x" ? 2 : 3));

            Assert.AreEqual(22.23571835, Math.Round((double)f.Evaluate(s => (s == "x" ? 2 : 3)),8));
        }

        // ########################### TEST MISSING FROM CODE COVERAGE ###########################  

        // FROM CONSTRUCTOR
        [TestMethod]
        [TestCategory("14")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOneFormula_Last()
        {
            Formula f = new Formula("1+x+", s => s.ToUpper(), s => (s == "X"));
        }

        [TestMethod]
        [TestCategory("14")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOneFormula_Last2()
        {
            Formula f = new Formula("(1+x)+", s => s.ToUpper(), s => (s == "X"));
        }

        [TestMethod]
        [TestCategory("15")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOneFormula_BeforeRP()
        {
            Formula f = new Formula("(1+x+)", s => s.ToUpper(), s => (s == "X"));
        }

        [TestMethod]
        [TestCategory("16")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOneFormula_AfterRP()
        {
            Formula f = new Formula("(1+x)5", s => s.ToUpper(), s => (s == "X"));
        }

        [TestMethod]
        [TestCategory("17")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOneFormula_Inaccepted()
        {
            Formula f = new Formula("(1+#)5", s => s.ToUpper(), s => (s == "X"));
        }

        [TestMethod]
        [TestCategory("18")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOneFormula_BalancedParenthe()
        {
            Formula f = new Formula("((1+3)", s => s.ToUpper(), s => (s == "X"));
        }

        // FROM Evaluate()
        [TestMethod]
        [TestCategory("19")]
        public void TestOneFormula_Cal()
        {
            Formula f = new Formula("(1+x)/5", s => s, s => (s == "x"));
            Assert.AreEqual(1.0, f.Evaluate(s => 4));
        }
        [TestMethod]
        [TestCategory("20")]
        public void TestOneFormula_Cal2()
        {
            Formula f = new Formula("5/(1*1+x)+5*(3-y)-1*(y+2)", s => s, s => true);
            Assert.AreEqual(2.0, f.Evaluate(s => (s == "x" ? 4 : 2)));
        }

        [TestMethod]
        [TestCategory("21")]
        public void TestEquals_Null()
        {
            Formula f = new Formula("x+1", s => s, s => true);

            Assert.IsFalse(f.Equals(null));
        }

        [TestMethod]
        [TestCategory("22")]
        public void TestEquals_Empty()
        {
            Formula f = new Formula("x+1", s => s, s => true);

            Assert.IsFalse(f.Equals(1));
        }

        [TestMethod]
        [TestCategory("23")]
        public void TestEquals_operator()
        {
            Formula f1 = new Formula("x+z", s => s, s => true);
            Formula f2 = new Formula("x+y", s => s, s => true);

            Assert.IsFalse(f1.Equals(f2));
        }

        [TestMethod]
        [TestCategory("24")]
        public void TestConstructionALot()
        {
            for (int i = 0; i < 10000; i++)
            {
                Formula f1 = new Formula("x1", s => s, s => true);
            }
        }
    }

    /// <summary>
    /// This tests are from Evaluator class for just checking caculation of Formula class.
    /// </summary>
    [TestClass]
    public class FromEvaluatorTest
    {
        [TestMethod(), Timeout(5000)]
        [TestCategory("1")]
        public void TestLeftToRight()
        {
            Formula f = new Formula("2*6+3", s => s, s => true);
            Assert.AreEqual(15.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("2")]
        public void TestOrderOperations()
        {
            Formula f = new Formula("2+6*3", s => s, s => true);
            Assert.AreEqual(20.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("3")]
        public void TestParenthesesTimes()
        {
            Formula f = new Formula("(2+6)*3", s => s, s => true);
            Assert.AreEqual(24.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("4")]
        public void TestTimesParentheses()
        {
            Formula f = new Formula("2*(3+5)", s => s, s => true);
            Assert.AreEqual(16.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("5")]
        public void TestPlusParentheses()
        {
            Formula f = new Formula("2+(3+5)", s => s, s => true);
            Assert.AreEqual(10.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("6")]
        public void TestPlusComplex()
        {
            Formula f = new Formula("2+(3+5*9)", s => s, s => true);
            Assert.AreEqual(50.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("7")]
        public void TestOperatorAfterParens()
        {
            Formula f = new Formula("(1*1)-2/2", s => s, s => true);
            Assert.AreEqual(0.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("8")]
        public void TestComplexTimesParentheses()
        {
            Formula f = new Formula("2+3*(3+5)", s => s, s => true);
            Assert.AreEqual(26.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("9")]
        public void TestComplexAndParentheses()
        {
            Formula f = new Formula("2+3*5+(3+4*8)*5+2", s => s, s => true);
            Assert.AreEqual(194.0, f.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("10")]
        public void TestComplexNestedParensRight()
        {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))", s => s, s => true);
            Assert.AreEqual(6.0, f.Evaluate(s => 1));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("11")]
        public void TestComplexNestedParensLeft()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6", s => s, s => true);
            Assert.AreEqual(6.0, f.Evaluate(s => 1));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("12")]
        public void TestRepeatedVar()
        {
            Formula f = new Formula("a4-a4*a4/a4", s => s, s => true);
            Assert.AreEqual(0.0, f.Evaluate(s => 3));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("13")]
        public void TestRepeatedVar2()
        {
            Formula f = new Formula("a4*a4*(a4*a4)*a4", s => s, s => true);
            Assert.AreEqual(243.0, f.Evaluate(s => 3));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("14")]
        public void TestRepeatedVar3()
        {
            Formula f = new Formula("a4/a4/(a4/a4)/a4", s => s, s => true);
            Assert.AreEqual(1.0, f.Evaluate(s => 1));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("15")]
        public void TestRepeatedVar4()
        {
            Formula f = new Formula("a4*a4/(a4*a4*a4/a4*a4/a4*a4*a4)/a4", s => s, s => true);
            Assert.AreEqual(1.0, f.Evaluate(s => 1));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("16")]
        public void TestRepeatedVar5()
        {
            Formula f = new Formula("a4/a4*(a4/a4/a4*a4/a4*a4/a4*a4/a4/a4)*a4", s => s, s => true);
            Assert.AreEqual(1.0, f.Evaluate(s => 1));
        }
    }
}