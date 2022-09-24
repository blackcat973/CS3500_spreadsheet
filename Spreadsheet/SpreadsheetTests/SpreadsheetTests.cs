using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    /// <summary>
    /// This test is for Spreadsheet. 
    /// </summary>
    [TestClass]
    public class SpreadsheetTests
    {
        // ########################### TEST SINGLE CONTENT ###########################  
        [TestMethod]
        [TestCategory("1")]
        public void TestSingleContent_Valid()
        {
            Spreadsheet s = new Spreadsheet();

            Assert.AreEqual("", s.GetCellContents("x"));
            Assert.AreEqual("", s.GetCellContents("_"));
            Assert.AreEqual("", s.GetCellContents("x2"));
            Assert.AreEqual("", s.GetCellContents("y_15"));
            Assert.AreEqual("", s.GetCellContents("___"));
        }

        [TestMethod]
        [TestCategory("1")]
        public void TestSingleContent_Valid2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", "");

            Assert.AreEqual("", s.GetCellContents("A1"));
        }

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("25");
        }

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid2()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("2x");
        }

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid3()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("&");
        }

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid4()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("&", 1.0);
        }

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid5()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("&", "cant");
        }

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid6()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("&", new Formula("A3+A1"));
        }

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid7()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("+", new Formula("A3+A1"));
        }

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid8()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("+X", new Formula("A3+A1"));
        }

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleContent_inValid10()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("+"));
        }

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleContent_inValid11()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("X+"));
        }

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleContent_inValid12()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("X+"));
        }

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleContent_inValid13()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("X("));
        }

        // Test cell changing
        [TestMethod]
        [TestCategory("3")]
        public void TestSingleContent_SetContent()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("B1+C1");

            s.SetCellContents("A1", 2.0);
            Assert.AreEqual(2.0, s.GetCellContents("A1"));

            s.SetCellContents("A1", 1e+2);
            Assert.AreEqual(100.0, s.GetCellContents("A1"));

            s.SetCellContents("A1", "test");
            Assert.AreEqual("test", s.GetCellContents("A1"));

            s.SetCellContents("A1", testfma);
            Assert.AreEqual(testfma, s.GetCellContents("A1"));
        }

        // Return GetNamesOfAll
        [TestMethod]
        [TestCategory("3")]
        public void TestSingleContent_GetAll()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("B1+C1");

            s.SetCellContents("A1", testfma);

            List<string> testlist = new List<string>();
            testlist.Add("A1");

            Assert.IsTrue(testlist.SequenceEqual(s.GetNamesOfAllNonemptyCells()));
        }

        [TestMethod]
        [TestCategory("3")]
        public void TestSingleContent_GetAll2()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("B1+C1");

            s.SetCellContents("A1", "");

            s.SetCellContents("A2", 1e+2);

            s.SetCellContents("A3", "");

            s.SetCellContents("A4", testfma);

            List<string> testlist = new List<string>();
            testlist.Add("A2"); testlist.Add("A4");

            Assert.IsTrue(testlist.SequenceEqual(s.GetNamesOfAllNonemptyCells()));
        }

        [TestMethod]
        [TestCategory("3")]
        public void TestSingleContent_GetAll3()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("B1+C1");

            s.SetCellContents("A1", 2.0);

            s.SetCellContents("A2", 1e+2);

            s.SetCellContents("A3", "test");

            s.SetCellContents("A4", testfma);

            List<string> testlist = new List<string>();
            testlist.Add("A1"); testlist.Add("A2"); testlist.Add("A3"); testlist.Add("A4");

            Assert.IsTrue(testlist.SequenceEqual(s.GetNamesOfAllNonemptyCells()));
        }

        // Return SetCellContents
        [TestMethod]
        [TestCategory("4")]
        public void TestSingleContent_ReturnSetCellList()
        {
            Spreadsheet s = new Spreadsheet();

            List<string> testlist = new List<string>();
            testlist.Add("A1");
            Assert.IsTrue(testlist.SequenceEqual(s.SetCellContents("A1", 2.0)));
        }

        [TestMethod]
        [TestCategory("4")]
        public void TestSingleContent_ReturnSetCellList2()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("B1");
            Formula testfma2 = new Formula("C1");

            List<string> testlist = new List<string>();
            testlist.Add("A1"); 
            Assert.IsTrue(testlist.SequenceEqual(s.SetCellContents("A1", testfma)));
            
            List<string> testlist2 = new List<string>() { "B1", "A1" };
            Assert.IsTrue(testlist2.SequenceEqual(s.SetCellContents("B1", testfma2)));

            List<string> testlist3 = new List<string>() { "C1", "B1", "A1" };
            Assert.IsTrue(testlist3.SequenceEqual(s.SetCellContents("C1", 1.0)));
        }

        // ########################### TEST DEPENDENCY CONTENT ###########################  

        // Check CircularException
        [TestMethod]
        [TestCategory("5")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Circular()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("A1");

            s.SetCellContents("A1", testfma);
        }

        [TestMethod]
        [TestCategory("5")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Circular2()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("A1");
            Formula testfma2 = new Formula("B1");

            s.SetCellContents("A1", testfma2);
            s.SetCellContents("B1", testfma);
        }

        [TestMethod]
        [TestCategory("5")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Circular3()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("A1");
            Formula testfma2 = new Formula("B1");
            Formula testfma3 = new Formula("C1");
            Formula testfma4 = new Formula("B1");

            s.SetCellContents("A1", testfma2);
            s.SetCellContents("B1", testfma3);
            s.SetCellContents("A1", testfma3);
            s.SetCellContents("B1", testfma);
            s.SetCellContents("C1", testfma2);
        }

        [TestMethod]
        [TestCategory("5")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Circular4()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("A1");
            Formula testfma2 = new Formula("B1");
            Formula testfma3 = new Formula("D1");

            s.SetCellContents("A1", testfma2);
            s.SetCellContents("B1", testfma3);
            s.SetCellContents("C1", testfma);
            s.SetCellContents("B1", testfma);
        }

        [TestMethod]
        [TestCategory("5")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Circular5()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("A1");
            Formula testfma2 = new Formula("B1");
            Formula testfma3 = new Formula("C1");
            Formula testfma4 = new Formula("D1");
            Formula testfma5 = new Formula("E1");

            s.SetCellContents("A1", testfma2);
            s.SetCellContents("B1", testfma3);
            s.SetCellContents("C1", testfma4);
            s.SetCellContents("D1", testfma5);
            s.SetCellContents("E1", testfma);
        }

        [TestMethod]
        [TestCategory("5")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Circular6()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("A1+A2");
            Formula testfma2 = new Formula("B1+C1");
            Formula testfma3 = new Formula("C1+D1");
            Formula testfma4 = new Formula("E1");
            Formula testfma5 = new Formula("D1+B1");

            s.SetCellContents("A1", testfma2);
            s.SetCellContents("B1", testfma3);
            s.SetCellContents("C1", testfma4);
            s.SetCellContents("D1", testfma5);
            s.SetCellContents("E1", testfma);
        }

        // Test whether backup is completed
        [TestMethod]
        [TestCategory("6")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Backup()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("B1");

            s.SetCellContents("A1", testfma);
            s.SetCellContents("B1", 1.0);
            s.SetCellContents("B1", testfma);

            Assert.AreEqual(1.0, s.GetCellContents("B1"));
        }

        [TestMethod]
        [TestCategory("6")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Backup2()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("A1");
            Formula testfma2 = new Formula("B1");

            s.SetCellContents("A1", testfma2);
            s.SetCellContents("B1", testfma);

            List<string> testlist = new List<string>();
            testlist.Add("A1");

            Assert.IsTrue(testlist.SequenceEqual(s.GetNamesOfAllNonemptyCells()));
        }

        // Test throw circular exception if it set itself
        [TestMethod]
        [TestCategory("6")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_FormulaCircular()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("A1+A1");

            s.SetCellContents("A1", testfma);
        }

        // Check not Circular one
        [TestMethod]
        [TestCategory("7")]
        public void TestDGContent_nonCircular()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("B1");
            List<string> testlist = new List<string>();
            testlist.Add("A1");

            s.SetCellContents("A1", testfma);
            Assert.AreEqual(testfma, s.GetCellContents("A1"));
            Assert.IsTrue(testlist.SequenceEqual(s.GetNamesOfAllNonemptyCells()));
        }

        [TestMethod]
        [TestCategory("7")]
        public void TestDGContent_nonCircular2()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("B1");
            Formula testfma2 = new Formula("C1");
            List<string> testlist = new List<string>();
            testlist.Add("A1"); testlist.Add("B1");

            s.SetCellContents("A1", testfma);
            s.SetCellContents("B1", testfma2);
            Assert.AreEqual(testfma, s.GetCellContents("A1"));
            Assert.AreEqual(testfma2, s.GetCellContents("B1"));
            Assert.IsTrue(testlist.SequenceEqual(s.GetNamesOfAllNonemptyCells()));
        }

        [TestMethod]
        [TestCategory("8")]
        public void TestDGContent_MultiCell()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("A2+A3");
            Formula testfma2 = new Formula("A4+A5");
            Formula testfma3 = new Formula("A2-A4");
            Formula testfma4 = new Formula("A5+A7");
            Formula testfma5 = new Formula("A6*A8");
            Formula testfma99 = new Formula("A99");
            s.SetCellContents("A1", testfma);
            s.SetCellContents("A2", testfma2);
            s.SetCellContents("A3", testfma3);
            s.SetCellContents("A4", testfma4);
            s.SetCellContents("A5", testfma5);
            s.SetCellContents("A6", testfma99);
            s.SetCellContents("A7", testfma99);
            s.SetCellContents("A8", testfma99);

            List<string> testlist = new List<string>() { "A99", "A8", "A7", "A6", "A5", "A4", "A2", "A3", "A1" };

            Assert.IsTrue(testlist.SequenceEqual(s.SetCellContents("A99", 1.0)));
        }

        [TestMethod]
        [TestCategory("8")]
        public void TestDGContent_MultiCell2()
        {
            Spreadsheet s = new Spreadsheet();
            Formula testfma = new Formula("A2+A3");
            Formula testfma2 = new Formula("A4+A5");
            Formula testfma3 = new Formula("A2-A4");
            Formula testfma4 = new Formula("A5+A7");
            Formula testfma5 = new Formula("A6*A8");
            Formula testfma99 = new Formula("A99");
            s.SetCellContents("A1", testfma);
            s.SetCellContents("A2", testfma2);
            s.SetCellContents("A3", testfma3);
            s.SetCellContents("A4", testfma4);
            s.SetCellContents("A5", testfma5);
            s.SetCellContents("A6", testfma99);
            s.SetCellContents("A7", testfma99);
            s.SetCellContents("A8", testfma99);

            List<string> testlist = new List<string>() { "A5", "A4", "A2", "A3", "A1" };
            Assert.IsTrue(testlist.SequenceEqual(s.SetCellContents("A5", 1.0)));

            List<string> testlist2 = new List<string>() { "A99", "A8", "A7", "A4", "A2", "A3", "A1", "A6" };
            Assert.IsTrue(testlist2.SequenceEqual(s.SetCellContents("A99", 1.0)));
        }

        // Stress Test
        [TestMethod]
        [TestCategory("9")]
        public void TestDGContent_MegaCell()
        {
            Spreadsheet s = new Spreadsheet();

            for(int i=0; i<100000; i++)
            {
                s.SetCellContents("A" + i, "A" + i + 1);
            }

            Assert.AreEqual(100000, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        [TestCategory("9")]
        public void TestDGContent_MegaCell2()
        {
            TestDGContent_MegaCell();
        }

        [TestMethod]
        [TestCategory("9")]
        public void TestDGContent_MegaCell3()
        {
            TestDGContent_MegaCell();
        }

        [TestMethod]
        [TestCategory("9")]
        public void TestDGContent_MegaCell4()
        {
            TestDGContent_MegaCell();
        }

        // Stress Test
        [TestMethod]
        [TestCategory("10")]
        public void TestDGContent2_MegaCell()
        {
            Spreadsheet s = new Spreadsheet();

            for (int i = 1; i <= 100000; i++)
            {
                s.SetCellContents("A" + i + 1, new Formula("A1"));
            }

            Assert.AreEqual(100000, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        [TestCategory("10")]
        public void TestDGContent2_MegaCell2()
        {
            TestDGContent_MegaCell();
        }

        [TestMethod]
        [TestCategory("10")]
        public void TestDGContent2_MegaCell3()
        {
            TestDGContent_MegaCell();
        }

        [TestMethod]
        [TestCategory("10")]
        public void TestDGContent2_MegaCell4()
        {
            TestDGContent_MegaCell();
        }
    }
}