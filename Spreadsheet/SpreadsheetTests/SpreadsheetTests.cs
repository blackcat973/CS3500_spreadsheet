using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        /// <summary>
        /// Check if variable is valid or not
        /// </summary>
        /// <param name="variable"> variable which needs to be checked </param>
        /// <returns>  Return true if variable is a valid variable, other false.
        /// </returns>
        private static bool IsValidVariable(string variable)
        {
            string varPattern = @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*";

            if (Regex.IsMatch(variable, varPattern, RegexOptions.Singleline))
                return true;

            return false;
        }
        // ################################### PS5 ###################################
        [TestMethod]
        [TestCategory("PS5_1")]
        public void ver2_0_Test_Valid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "");
        }

        [TestMethod]
        [TestCategory("PS5_1")]
        public void ver2_0_Test_Valid2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "a");
            Assert.AreEqual(s.GetCellContents("A1").GetType(), typeof(string));
        }

        [TestMethod]
        [TestCategory("PS5_1")]
        public void ver2_0_Test_Valid3()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "");
            s.SetContentsOfCell("A1", "a");
            Assert.AreEqual(s.GetCellValue("A1"), "a");
        }

        [TestMethod]
        [TestCategory("PS5_1")]
        public void ver2_0_Test_Valid4()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "");
            s.SetContentsOfCell("A1", "=B1");
        }

        [TestMethod]
        [TestCategory("PS5_1")]
        public void ver2_0_Test_Valid5()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            Assert.AreEqual(s.GetCellContents("A1").GetType(), typeof(double));
        }

        // Test Equal
        [TestMethod]
        [TestCategory("PS5_2")]
        public void ver2_0_Test_Equal()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "");
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("a1", "1");
            Assert.AreEqual((double) s.GetCellValue("A1"), (double) s.GetCellValue("a1"), 1e-9);
        }

        [TestMethod]
        [TestCategory("PS5_2")]
        public void ver2_0_Test_Equal2()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "");
            s.SetContentsOfCell("A1", "1");
            Assert.AreEqual(1.0, (double)s.GetCellValue("A1"), 1e-9);
            Assert.AreEqual(1.0, (double)s.GetCellValue("a1"), 1e-9);
        }
        // Change value
        [TestMethod]
        [TestCategory("PS5_3")]
        public void ver2_0_Test_Change()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "");
            s.SetContentsOfCell("A1", "=B1");
            Assert.AreEqual(s.GetCellContents("A1").GetType(), typeof(Formula));

            s.SetContentsOfCell("A1", "1");
            Assert.AreEqual(1.0, (double)s.GetCellValue("A1"), 1e-9);
        }

        [TestMethod]
        [TestCategory("PS5_3")]
        public void ver2_0_Test_Change2()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "");
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("A2", "3");
            s.SetContentsOfCell("A3", "= A1 + A2");
            Assert.AreEqual(4.0, (double)s.GetCellValue("A3"), 1e-9);

            s.SetContentsOfCell("A2", "30");
            Assert.AreEqual(31.0, (double)s.GetCellValue("A3"), 1e-9);
        }
        // Save test
        [TestMethod]
        [TestCategory("PS5_4")]
        public void ver2_0_Test_Save()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "1.0");
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("A2", "3");
            s.SetContentsOfCell("A3", "= A1 + A2");
            s.SetContentsOfCell("B1", "test");
            s.SetContentsOfCell("B2", "TEST");
            s.SetContentsOfCell("B3", "a1");
            s.SetContentsOfCell("B3", "=a1");

            s.Save("savetest.json");
        }

        [TestMethod]
        [TestCategory("PS5_4")]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ver2_0_Test_Invalid_Save()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "1.0");
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("A2", "3");
            s.SetContentsOfCell("A3", "= A1 + A2");
            s.SetContentsOfCell("B1", "test");
            s.SetContentsOfCell("B1", "TEST");

            s.Save("<");
        }        
        // Load test
        [TestMethod]
        [TestCategory("PS5_5")]
        public void ver2_0_Test_Load()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "1.0");
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("A2", "4");
            s.SetContentsOfCell("A3", "= A1 + A2");
            s.SetContentsOfCell("B1", "3");
            s.SetContentsOfCell("B2", "5");
            s.SetContentsOfCell("B3", "= B1 * B2");
            s.SetContentsOfCell("B4", "= B3 / A3");

            s.Save("loadtest.json");

            Spreadsheet s2 = new Spreadsheet("loadtest.json", s => true, s => s, "1.0");
            Assert.AreEqual(5.0, (double)s.GetCellValue("A3"), 1e-9);
            Assert.AreEqual(15.0, (double)s.GetCellValue("B3"), 1e-9);
            Assert.AreEqual(3.0, (double)s.GetCellValue("B4"), 1e-9);
        }

        [TestMethod]
        [TestCategory("PS5_5")]
        public void ver2_0_Test_Load2()
        {
            ver2_0_Test_Load();
        }

        [TestMethod]
        [TestCategory("PS5_6")]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ver2_0_Test_Invalid_Load()
        {
             Spreadsheet s2 = new Spreadsheet("Nowhere.json", s => true, s => s, "1.0");
        }

        [TestMethod]
        [TestCategory("PS5_6")]
        [ExpectedException(typeof(InvalidNameException))]
        public void ver2_0_Test_Invalid_Load2()
        {
            Spreadsheet s = new Spreadsheet(s => false, s => s.ToUpper(), "1.0");
            s.SetContentsOfCell("A1", "1");

            s.Save("loadtest.json");

            Spreadsheet s2 = new Spreadsheet("loadtest.json", s => true, s => s, "1.0");
        }

        // Check diff version
        [TestMethod]
        [TestCategory("PS5_6")]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ver2_0_Test_Invalid_Load3()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "1.0");
            s.SetContentsOfCell("A1", "1");

            s.Save("loadtest2.json");

            Spreadsheet s2 = new Spreadsheet("loadtest2.json", s => true, s => s, "2.0");
        }
        // GetCellValue test
        [TestMethod]
        [TestCategory("PS5_7")]
        [ExpectedException(typeof(InvalidNameException))]
        public void ver2_0_Test_GetCellValue()
        {
            Spreadsheet s = new Spreadsheet(s => false, s => s.ToUpper(), "");

            s.GetCellValue("^%");
        }

        [TestMethod]
        [TestCategory("PS5_7")]
        public void ver2_0_Test_GetCellValue2()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "");

            Assert.AreEqual("", s.GetCellValue("B1"));
        }

        [TestMethod]
        [TestCategory("PS5_8")]
        [ExpectedException(typeof(InvalidNameException))]
        public void ver2_0_Test_4Para_Constructor()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "1.0");
            s.SetContentsOfCell("A1", "1");

            s.Save("loadtest3.json");

            Spreadsheet s2 = new Spreadsheet("loadtest3.json", s => false, s => s, "2.0");
        }

        // Stress Test
        [TestMethod]
        [TestCategory("PS5_9")]
        public void TestDGContent_MegaCell()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");

            for (int i = 0; i < 100000; i++)
            {
                s.SetContentsOfCell("A" + i, "A" + i + 1);
            }

            Assert.AreEqual(100000, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        [TestCategory("PS5_9")]
        public void TestDGContent_MegaCell2()
        {
            TestDGContent_MegaCell();
        }

        [TestMethod]
        [TestCategory("PS5_9")]
        public void TestDGContent_MegaCell3()
        {
            TestDGContent_MegaCell();
        }

        [TestMethod]
        [TestCategory("PS5_9")]
        public void TestDGContent_MegaCell4()
        {
            TestDGContent_MegaCell();
        }

        // Stress Test
        [TestMethod]
        [TestCategory("PS5_10")]
        public void TestDGContent2_MegaCell()
        {

            try
            {
                for (int i = 1; i <= 100; i++)
                {
                    Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");

                    for (int j = i + 1; j <= i + 10000; j++)
                    {
                        s.SetContentsOfCell("A" + j, "A" + j + 1);
                        s.SetContentsOfCell("B" + j, "B" + j + 1);
                        s.SetContentsOfCell("C" + j, "C" + j + 1);
                    }
                    s.Save("multi1.json");

                    Assert.AreEqual("A" + i + 101, s.GetCellValue("A" + i + 10));
                    Assert.AreEqual("B" + i + 101, s.GetCellValue("B" + i + 10));
                    Assert.AreEqual("C" + i + 101, s.GetCellValue("C" + i + 10));

                    Spreadsheet s2 = new Spreadsheet("multi1.json", s => true, s => s, "");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [TestMethod]
        [TestCategory("PS5_10")]
        public void TestDGContent2_MegaCell2()
        {
            TestDGContent_MegaCell();
        }

        [TestMethod]
        [TestCategory("PS5_10")]
        public void TestDGContent2_MegaCell3()
        {
            TestDGContent_MegaCell();
        }

        [TestMethod]
        [TestCategory("PS5_10")]
        public void TestDGContent2_MegaCell4()
        {
            TestDGContent_MegaCell();
        }

        // ################################### PS4 ###################################  
        // ########################### TEST SINGLE CONTENT ###########################  
        [TestMethod]
        [TestCategory("PS4_1")]
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
        [TestCategory("PS4_1")]
        public void TestSingleContent_Valid2()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("A1", "");

            Assert.AreEqual("", s.GetCellContents("A1"));
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.GetCellContents("25");
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid2()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.GetCellContents("2x");
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid3()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.GetCellContents("&");
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid4()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("&", "1.0");
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid5()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("&", "cant");
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid6()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("&", "=A3+A1");
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid7()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("+", "=A3+A1");
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSingleContent_inValid8()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("+X", "=A3+A1");
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleContent_inValid10()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("A1", "=+");
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        public void TestSingleContent_inValid10_2()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("A1", "+");
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleContent_inValid11()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("A1", "=X+");
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleContent_inValid12()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("A1", "=X+");
        }

        [TestMethod]
        [TestCategory("PS4_2")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleContent_inValid13()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("A1", "=X(");
        }

        // Test cell changing
        [TestMethod]
        [TestCategory("PS4_3")]
        public void TestSingleContent_SetContent()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");

            s.SetContentsOfCell("A1", "2.0");
            Assert.AreEqual(2.0, s.GetCellContents("A1"));

            s.SetContentsOfCell("A1", "1e+2");
            Assert.AreEqual(100.0, s.GetCellContents("A1"));

            s.SetContentsOfCell("A1", "test");
            Assert.AreEqual("test", s.GetCellContents("A1"));

            s.SetContentsOfCell("A1", "=B1+C1");
            Assert.AreEqual("B1+C1", s.GetCellContents("A1").ToString());
        }

        // Return GetNamesOfAll
        [TestMethod]
        [TestCategory("PS4_3")]
        public void TestSingleContent_GetAll()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");

            s.SetContentsOfCell("A1", "B1+C1");

            List<string> testlist = new List<string>();
            testlist.Add("A1");

            Assert.IsTrue(testlist.SequenceEqual(s.GetNamesOfAllNonemptyCells()));
        }

        [TestMethod]
        [TestCategory("PS4_3")]
        public void TestSingleContent_GetAll2()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");

            s.SetContentsOfCell("A1", "");

            s.SetContentsOfCell("A2", "1e+2");

            s.SetContentsOfCell("A3", "");

            s.SetContentsOfCell("A4", "=B1+C1");

            List<string> testlist = new List<string>();
            testlist.Add("A2"); testlist.Add("A4");

            Assert.IsTrue(testlist.SequenceEqual(s.GetNamesOfAllNonemptyCells()));
        }

        [TestMethod]
        [TestCategory("PS4_3")]
        public void TestSingleContent_GetAll3()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            Formula testfma = new Formula("B1+C1");

            s.SetContentsOfCell("A1", "2.0");

            s.SetContentsOfCell("A2", "1e+2");

            s.SetContentsOfCell("A3", "test");

            s.SetContentsOfCell("A4", "=B1+C1");

            List<string> testlist = new List<string>();
            testlist.Add("A1"); testlist.Add("A2"); testlist.Add("A3"); testlist.Add("A4");

            Assert.IsTrue(testlist.SequenceEqual(s.GetNamesOfAllNonemptyCells()));
        }

        // Return SetCellContents
        [TestMethod]
        [TestCategory("PS4_4")]
        public void TestSingleContent_ReturnSetCellList()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");

            List<string> testlist = new List<string>();
            testlist.Add("A1");
            Assert.IsTrue(testlist.SequenceEqual(s.SetContentsOfCell("A1", "2.0")));
        }

        [TestMethod]
        [TestCategory("PS4_4")]
        public void TestSingleContent_ReturnSetCellList2()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            Formula testfma = new Formula("B1");
            Formula testfma2 = new Formula("C1");

            List<string> testlist = new List<string>();
            testlist.Add("A1"); 
            Assert.IsTrue(testlist.SequenceEqual(s.SetContentsOfCell("A1", "=B1")));
            
            List<string> testlist2 = new List<string>() { "B1", "A1" };
            Assert.IsTrue(testlist2.SequenceEqual(s.SetContentsOfCell("B1", "=C1")));

            List<string> testlist3 = new List<string>() { "C1", "B1", "A1" };
            Assert.IsTrue(testlist3.SequenceEqual(s.SetContentsOfCell("C1", "1.0")));
        }

        // ########################### TEST DEPENDENCY CONTENT ###########################  

        // Check CircularException
        [TestMethod]
        [TestCategory("PS4_5")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Circular()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            Formula testfma = new Formula("A1");

            s.SetContentsOfCell("A1", "=A1");
        }

        [TestMethod]
        [TestCategory("PS4_5")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Circular2()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            Formula testfma = new Formula("A1");
            Formula testfma2 = new Formula("B1");

            s.SetContentsOfCell("A1", "=A1");
            s.SetContentsOfCell("B1", "=B1");
        }

        [TestMethod]
        [TestCategory("PS4_5")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Circular4()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            Formula testfma = new Formula("A1");
            Formula testfma2 = new Formula("B1");
            Formula testfma3 = new Formula("D1");

            s.SetContentsOfCell("A1", "=B1");
            s.SetContentsOfCell("B1", "=D1");
            s.SetContentsOfCell("C1", "=A1");
            s.SetContentsOfCell("B1", "=A1");
        }

        [TestMethod]
        [TestCategory("PS4_5")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Circular5()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            Formula testfma = new Formula("A1");
            Formula testfma2 = new Formula("B1");
            Formula testfma3 = new Formula("C1");
            Formula testfma4 = new Formula("D1");
            Formula testfma5 = new Formula("E1");

            s.SetContentsOfCell("A1", "=B1");
            s.SetContentsOfCell("B1", "=C1");
            s.SetContentsOfCell("C1", "=D1");
            s.SetContentsOfCell("D1", "=E1");
            s.SetContentsOfCell("E1", "=A1");
        }

        [TestMethod]
        [TestCategory("PS4_5")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Circular6()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            Formula testfma = new Formula("A1+A2");
            Formula testfma2 = new Formula("B1+C1");
            Formula testfma3 = new Formula("C1+D1");
            Formula testfma4 = new Formula("E1");
            Formula testfma5 = new Formula("D1+B1");

            s.SetContentsOfCell("A1", "=B1+C1");
            s.SetContentsOfCell("B1", "=B1+C1");
            s.SetContentsOfCell("C1", "=E1");
            s.SetContentsOfCell("D1", "=E1");
            s.SetContentsOfCell("E1", "=A1+A2");
        }

        [TestMethod]
        [TestCategory("PS4_6")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_Backup2()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            Formula testfma = new Formula("A1");
            Formula testfma2 = new Formula("B1");

            s.SetContentsOfCell("A1", "=B1");
            s.SetContentsOfCell("B1", "=A1");

            List<string> testlist = new List<string>();
            testlist.Add("A1");

            Assert.IsTrue(testlist.SequenceEqual(s.GetNamesOfAllNonemptyCells()));
        }

        // Test throw circular exception if it set itself
        [TestMethod]
        [TestCategory("PS4_6")]
        [ExpectedException(typeof(CircularException))]
        public void TestDGContent_FormulaCircular()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            Formula testfma = new Formula("A1+A1");

            s.SetContentsOfCell("A1", "=A1+A1");
        }

        // Check not Circular one
        [TestMethod]
        [TestCategory("PS4_7")]
        public void TestDGContent_nonCircular()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            Formula testfma = new Formula("B1");
            List<string> testlist = new List<string>();
            testlist.Add("A1");

            s.SetContentsOfCell("A1", "=B1");
            Assert.AreEqual(testfma, s.GetCellContents("A1"));
            Assert.IsTrue(testlist.SequenceEqual(s.GetNamesOfAllNonemptyCells()));
        }

        [TestMethod]
        [TestCategory("PS4_7")]
        public void TestDGContent_nonCircular2()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            Formula testfma = new Formula("B1");
            Formula testfma2 = new Formula("C1");
            List<string> testlist = new List<string>();
            testlist.Add("A1"); testlist.Add("B1");

            s.SetContentsOfCell("A1", "=B1");
            s.SetContentsOfCell("B1", "=C1");
            Assert.AreEqual(testfma, s.GetCellContents("A1"));
            Assert.AreEqual(testfma2, s.GetCellContents("B1"));
            Assert.IsTrue(testlist.SequenceEqual(s.GetNamesOfAllNonemptyCells()));
        }

        [TestMethod]
        [TestCategory("PS4_8")]
        public void TestDGContent_MultiCell()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A2", "=A4+A5");
            s.SetContentsOfCell("A3", "=A2-A4");
            s.SetContentsOfCell("A4", "=A5+A7");
            s.SetContentsOfCell("A5", "=A6*A8");
            s.SetContentsOfCell("A6", "=A99");
            s.SetContentsOfCell("A7", "=A99");
            s.SetContentsOfCell("A8", "=A99");

            List<string> testlist = new List<string>() { "A99", "A8", "A7", "A6", "A5", "A4", "A2", "A3", "A1" };

            Assert.IsTrue(testlist.SequenceEqual(s.SetContentsOfCell("A99", "1.0")));
        }

        [TestMethod]
        [TestCategory("PS4_8")]
        public void TestDGContent_MultiCell2()
        {
            Spreadsheet s = new Spreadsheet(s => IsValidVariable(s), s => s, "");
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A2", "=A4+A5");
            s.SetContentsOfCell("A3", "=A2-A4");
            s.SetContentsOfCell("A4", "=A5+A7");
            s.SetContentsOfCell("A5", "=A6*A8");
            s.SetContentsOfCell("A6", "=A99");
            s.SetContentsOfCell("A7", "=A99");
            s.SetContentsOfCell("A8", "=A99");

            List<string> testlist = new List<string>() { "A5", "A4", "A2", "A3", "A1" };
            Assert.IsTrue(testlist.SequenceEqual(s.SetContentsOfCell("A5", "1.0")));

            List<string> testlist2 = new List<string>() { "A99", "A8", "A7", "A4", "A2", "A3", "A1", "A6" };
            Assert.IsTrue(testlist2.SequenceEqual(s.SetContentsOfCell("A99", "1.0")));
        }

        
    }
}