
// Change log:
// Last updated: 9/30
/** @author     SangYoon Cho
 *  @date       2022/09/30 (Y/M/D)
 *  @version    1.1 ver
 *                  -> Fill in each methods with copying Spreadsheet.cs and AbstractSpreadsheet.cs 
 *                     and creates tests cs file.
 *              1.2 ver
 *                  -> Line 86: Change == to 'is not'
 *              2.0 ver
 *                  -> Modify Code refer to PS5
 *              
 */

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.Json;
using System.IO;

namespace SS
{
    // This class is going to be used as some kind of object vertical json
    public class Spreadsheet : AbstractSpreadsheet
    {
        // Get reference from DependencyGraph
        private DependencyGraph dg;
        // Store Spreadsheet Cell information into the Dictionary
        [JsonProperty(PropertyName = "cells")]
        private Dictionary<string, Cell> spreadSheetCell;

        private bool IsItChanged;
        /// <summary>
        /// This class is for Spreadsheet Cell to store input value.
        /// It takes double, string and Formula type value.
        /// If nothing input, show blank cell, which is actually "".
        /// </summary>
        /// 
        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class Cell
        {
            [JsonProperty(PropertyName = "stringForm")]
            public string contentString;    // store stringForm
            public object cellContent;      // store input content
            public object cellValue;        // store evaluated value
            public string getType;          // store type of the variable

            public Cell()
            {
                this.cellContent = "";
                this.cellValue = "";   // Would be used later
                this.getType = "string";
                this.contentString = "";
            }
            // Double constructor
            public Cell(double d)
            {
                this.cellContent = d;
                this.cellValue = d;    // Would be used later
                this.getType = d.GetType().ToString();
                this.contentString = d.ToString();
            }
            // String constructor
            public Cell(string s)
            {
                this.cellContent = s;
                this.cellValue = s;    // Would be used later
                this.getType = s.GetType().ToString();
                this.contentString = s.ToString();
            }
            // Formula constructor
            public Cell(Formula obj, Func<string, double> lookup)
            {
                this.cellContent = obj;
                this.cellValue = obj.Evaluate(lookup);
                this.getType = obj.GetType().ToString();
                this.contentString = "=" + obj.ToString();
            }

            // Getter, Setter of Cell class
            public object CellContent
            {
                get { return cellContent; }
                set { cellContent = value; }
            }

            public object CellValue
            {
                get { return cellValue; }
                set { cellValue = value; }
            }

            public object CellType
            {
                get { return getType; }
                set { getType = value.GetType().ToString(); }
            }

            public string ContentString
            {
                get { return contentString; }
                set { contentString = value; }
            }

            /// <summary>
            /// This is a helper method for recalculating with lookup delegate.
            /// </summary>
            /// <param name="lookup"> delegate function for converting string to double. </param>
            public void calculateFormula(Func<string, double> lookup)
            {
                if (getType.Equals("SpreadsheetUtilities.Formula"))
                {
                    Formula formula = (Formula)cellContent;
                    this.cellValue = formula.Evaluate(lookup);
                }
            }
        }

        /// <summary>
        /// Set a getter and setter for checking whether value is changed.
        /// </summary>
        public override bool Changed { get { return IsItChanged; } protected set { IsItChanged = value; } }

        // Spread default constructor
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            dg = new DependencyGraph();
            spreadSheetCell = new Dictionary<string, Cell>();

            Changed = false;
        }

        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            dg = new DependencyGraph();
            spreadSheetCell = new Dictionary<string, Cell>();

            Changed = false;
        }

        /// <summary>
        /// Check if file is invalid or not equal version. And then load a file, store values.
        /// </summary>
        /// <param name="file"> files needed to be read </param>
        /// <param name="isValid"> isValid delegate function. </param>
        /// <param name="normalize"> Normalize delegate function. </param>
        /// <param name="version"> Program version </param>
        /// <exception cref="InvalidNameException"> If file name is invalid. </exception>
        /// <exception cref="SpreadsheetReadWriteException"> If file and this spreadsheet have different version. </exception>
        public Spreadsheet(string file, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            if (!IsValid(Normalize(file)))
                throw new InvalidNameException();

            dg = new DependencyGraph();
            spreadSheetCell = new Dictionary<string, Cell>();

            // Load file
            readFile(file);

            Changed = false;
        }

        /// <summary>
        /// This is the actual load file method.
        /// </summary>
        /// <param name="filename"> file which you are gonna try to read </param>
        /// <exception cref="SpreadsheetReadWriteException"> If file and this program have different version. </exception>
        private void readFile(string filename)
        {
            try
            {
                // Load file text from JSON, and initialize it into the Spreadsheet object
                Spreadsheet? s = JsonConvert.DeserializeObject<Spreadsheet>(File.ReadAllText(filename));
                if (s != null)
                {
                    if (!this.Version.Equals(s.Version))
                    {
                        // catch different version
                        throw new SpreadsheetReadWriteException("Cannot load different version of the file.");
                    }
                    else
                    {
                        // you need to store value appropriately again for setting DG graph and Cell value.
                        foreach (string str in s.spreadSheetCell.Keys)
                        {
                            string contentFromFile = s.spreadSheetCell[str].ContentString;
                            // contentstring 형태변환
                            SetContentsOfCell(str, contentFromFile);
                        }
                    }
                }
            }
            catch (Exception)   // Catch file name exception
            {
                throw new SpreadsheetReadWriteException("Invalid filename.");
            }
        }

        /// <summary>
        /// This is the actual save method
        /// </summary>
        /// <param name="filename"> file which you are gonna try to read </param>
        /// <exception cref="SpreadsheetReadWriteException"> If filename is invalid </exception>
        public override void Save(string filename)
        {
            try
            {
                // Serialize spreadsheet and store into the JSON file.
                string json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filename, json);
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Invalid name.");
            }

            Changed = false;
        }

        /// <returns> if no any cells are created,  return null
        ///                                         otherwise, return all cells </returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            List<string> allNonEmptyCell = new List<string>();

            foreach (string name in spreadSheetCell.Keys)
            {
                // Define empty cell that has only ""
                if (spreadSheetCell[name].CellValue is not "")
                    allNonEmptyCell.Add(name);
            }

            return allNonEmptyCell;
        }

        /// <param name="name"> Cell name(index) </param>
        /// <returns> return contents of the cell
        ///           if there's no cell which has 'name', return empty </returns>
        /// <exception cref="InvalidNameException"></exception>
        public override object GetCellContents(string name)
        {
            if (!IsValid(name))
                throw new InvalidNameException();

            if (!spreadSheetCell.ContainsKey(Normalize(name)))
                return "";
            else
                return spreadSheetCell[Normalize(name)].CellContent;
        }

        /// <summary>
        /// Get the cell value.
        /// </summary>
        /// <param name="name"> Spreadsheet key </param>
        /// <returns> the value of the key in the spreadsheet </returns>
        /// <exception cref="InvalidNameException"> If name is invalid </exception>
        public override object GetCellValue(string name)
        {
            // Isvalid
            if (!IsValid(name))
                throw new InvalidNameException();
            // get the value of the cell
            if (!spreadSheetCell.ContainsKey(Normalize(name)))
                return "";
            else
                return spreadSheetCell[Normalize(name)].CellValue;
        }

        /// <summary>
        /// This is kind of tollgate. Every content should pass this method and need to be checked their type.
        /// </summary>
        /// <param name="name"> Key value of the spreadsheet(Cell) </param>
        /// <param name="content"> The value in the cell </param>
        /// <returns> list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell  </returns>
        /// <exception cref="InvalidNameException"> If name is invalid </exception>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (!IsValid(name))
                throw new InvalidNameException();

            IList<string> allDept = new List<string>();

            if (Double.TryParse(content, out double result)) // If content can be double type
            {
                allDept = SetCellContents(name, result);
            }
            else
            {
                // If content can be Formula
                if (!content.Equals("") && content.ElementAt(0).Equals('='))
                {
                    string formula = content.Substring(1);
                    Formula f = new Formula(formula, Normalize, IsValid);
                    allDept = SetCellContents(name, f);
                }
                else
                    allDept = SetCellContents(name, content);
            }

            Changed = true;

            // Recalculate process. When the program did organization successfully, 
            // program needs to do calculate according to the order of connection
            foreach (string str in allDept)
            {
                if (spreadSheetCell.TryGetValue(str, out Cell? cellV))   // until cellV is Formula
                    if (cellV.CellType.Equals("SpreadsheetUtilities.Formula"))  // Only formula need to be calculated
                        cellV.calculateFormula(myLookUp);
            }

            return allDept;
        }

        /// <summary>
        /// If trying to set the cell which has Formula before, 
        /// disconnect link from dependees.
        /// </summary>
        /// <param name="name"> cell name </param>
        /// <param name="number"> the value setting cell content </param>
        /// <returns> 
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell 
        /// </returns>
        /// <exception cref="InvalidNameException"></exception>        
        protected override IList<string> SetCellContents(string name, double number)
        {
            // If spreadsheet contains name key
            if (!spreadSheetCell.ContainsKey(name))
                spreadSheetCell.Add(name, new Cell(number));
            else
            {
                spreadSheetCell[name].CellContent = number;
                spreadSheetCell[name].CellValue = number;
                spreadSheetCell[name].CellType = number;
                spreadSheetCell[name].ContentString = number.ToString();
            }
            // Reorder Dependency graph
            dg.ReplaceDependees(name, new List<string>());

            List<string> list = new List<string>(GetCellsToRecalculate(name));

            return list;
        }

        /// <summary>
        /// If trying to set the cell which has Formula before, 
        /// disconnect link from dependees.
        /// </summary>
        /// <param name="name"> cell name </param>
        /// <param name="text"> the value setting cell content </param>
        /// <returns> 
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell 
        /// </returns>
        /// <exception cref="InvalidNameException"></exception>
        protected override IList<string> SetCellContents(string name, string text)
        {
            List<string> list = new List<string>();

            if (!spreadSheetCell.ContainsKey(name))
                spreadSheetCell.Add(name, new Cell(text));
            else
            {
                spreadSheetCell[name].CellContent = text;
                spreadSheetCell[name].CellValue = text;
                spreadSheetCell[name].CellType = text;
                spreadSheetCell[name].ContentString = text;
            }

            dg.ReplaceDependees(name, new List<string>());

            list = new List<string>(GetCellsToRecalculate(name));

            return list;
        }

        /// <summary>
        /// Find the variable in the format entered into the input formula and input it
        /// appropriately for the dependency graph.
        /// If It is set by different formula, also set the dependency graph.
        /// </summary>
        /// <param name="name"> cell name </param>
        /// <param name="fma"> input formula which set into the cell content </param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        /// <exception cref="CircularException"></exception>
        protected override IList<string> SetCellContents(string name, Formula fma)
        {
            if (fma.GetVariables().Contains(name))
                throw new CircularException();

            // These are for the backup against exception occursion
            List<string> list = new List<string>();
            HashSet<string> backupDG_dept = new HashSet<string>();  // the list for backup dependent list of the name
            HashSet<string> backupDG_dee = new HashSet<string>();   // the list for backup dependee list of the name
            bool backup_WasThereName = false;                       // check whether cell has previous content or not 
            object prevValue = "";                                  // store previous content of the cell
            object prevContent = "";                                // store previous content of the cell
            object prevType = "";                                   // store previous content of the cell
            string prevString = "";

            // If name cell already has content
            if (spreadSheetCell.ContainsKey(name))
            {
                prevContent = spreadSheetCell[name].CellContent;
                prevValue = spreadSheetCell[name].CellValue;        // save previous data(content)
                prevType = spreadSheetCell[name].CellType;
                prevString = spreadSheetCell[name].ContentString;

                foreach (string dept in dg.GetDependents(name))      // save dept list
                    backupDG_dept.Add(dept);
                foreach (string dee in dg.GetDependees(name))       // save dee list
                    backupDG_dept.Add(dee);
                backup_WasThereName = true;                         // it has previous content
            }

            try
            {
                // create a new cell
                if (!spreadSheetCell.ContainsKey(name))
                {
                    spreadSheetCell.Add(name, new Cell(fma, myLookUp));
                    // set the dependency graph according to the formula variables.
                    foreach (string str in fma.GetVariables())
                        dg.AddDependency(str, name);
                }
                // set an existing cell
                else
                {
                    spreadSheetCell[name].CellContent = fma;
                    spreadSheetCell[name].CellValue = fma;
                    spreadSheetCell[name].CellType = fma;
                    spreadSheetCell[name].ContentString = "=" + fma.ToString();
                    // set the dependency graph according to the formula variables.
                    dg.ReplaceDependees(name, fma.GetVariables());
                }

                list = GetCellsToRecalculate(name).ToList();
            }
            catch (CircularException e)
            {
                // Backup operation
                spreadSheetCell[name].CellValue = prevValue;
                foreach (string backup in fma.GetVariables())
                    dg.RemoveDependency(name, backup);
                if (backup_WasThereName)
                {
                    dg.ReplaceDependees(name, backupDG_dee);
                }
                throw e;
            }

            return list;
        }

        /// <summary>
        /// Get direct connection from name cell
        /// </summary>
        /// <param name="name"> cell name </param>
        /// <returns> return list of the directly connected cell </returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return dg.GetDependents(name);
        }

        /// <summary>
        /// This method is a helper method to set the lookup method.
        /// If cell doesn't allocate yet or empty or its value is not a double, throw exception.
        /// O.W. return double value.
        /// </summary>
        /// <param name="name"> Key value of the spreadsheet </param>
        /// <returns> double value of the cells' actual value </returns>
        /// <exception cref="ArgumentException"> If it is not the appropriate value in the cell, throw exception. </exception>
        private double myLookUp(string name)
        {
            if (!spreadSheetCell.ContainsKey(name) || spreadSheetCell[name].CellValue.Equals("") || spreadSheetCell[name].CellValue.GetType() != typeof(double))
                throw new ArgumentException();
            else
                return (double)spreadSheetCell[name].CellValue;
        }
    }
}