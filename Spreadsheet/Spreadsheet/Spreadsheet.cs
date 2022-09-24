
// Change log:
// Last updated: 9/23
/** @author     SangYoon Cho
 *  @date       2022/09/23 (Y/M/D)
 *  @version    1.1 ver
 *                  -> Fill in each methods with copying Spreadsheet.cs and AbstractSpreadsheet.cs 
 *                     and creates tests cs file.
 *              1.2 ver
 *                  -> Line 86: Change == to 'is not'
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

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        // Get reference from DependencyGraph
        private DependencyGraph dg = new DependencyGraph();

        /// <summary>
        /// This class is for Spreadsheet Cell to store input value.
        /// It takes double, string and Formula type value.
        /// If nothing input, show blank cell, which is actually "".
        /// </summary>
        private class Cell
        {
            object cellContent;
            //object cellValue; // This would be used later

            public Cell()
            {
                cellContent = "";
                //cellValue = "";   // Would be used later
            }
            // Double constructor
            public Cell(double d)
            {
                cellContent = d;
                //cellValue = d;    // Would be used later
            }
            // String constructor
            public Cell(string s)
            {
                cellContent = s;
                //cellValue = s;    // Would be used later
            }
            // Formula constructor
            public Cell(Formula obj)
            {
                cellContent = obj;
                //cellValue = obj;  // Would be used later
            }

            // Getter, Setter of Cell class
            public object CellValue
            {
                get { return cellContent; }
                set { cellContent = value; }
            }
        }
        // Store Spreadsheet Cell information into the Dictionary
        private Dictionary<string, Cell> spreadSheetCell = new Dictionary<string, Cell>();


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
            if (!IsValidVariable(name))
                throw new InvalidNameException();

            if (!spreadSheetCell.ContainsKey(name))
                return new Cell().CellValue;
            else
                return spreadSheetCell[name].CellValue;
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
        public override IList<string> SetCellContents(string name, double number)
        {
            if (!IsValidVariable(name))
                throw new InvalidNameException();
            if (!spreadSheetCell.ContainsKey(name))
                spreadSheetCell.Add(name, new Cell(number));
            else
            {
                spreadSheetCell[name].CellValue = number;
            }

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
        public override IList<string> SetCellContents(string name, string text)
        {
            if (!IsValidVariable(name))
                throw new InvalidNameException();

            List<string> list = new List<string>();

            if (!spreadSheetCell.ContainsKey(name))
                spreadSheetCell.Add(name, new Cell(text));
            else
            {
                spreadSheetCell[name].CellValue = text;
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
        public override IList<string> SetCellContents(string name, Formula fma)
        {
            if (!IsValidVariable(name))
                throw new InvalidNameException();

            if (fma.GetVariables().Contains(name))
                throw new CircularException();

            // These are for the backup against exception occursion
            List<string> list = new List<string>();
            HashSet<string> backupDG_dept = new HashSet<string>();  // the list for backup dependent list of the name
            HashSet<string> backupDG_dee = new HashSet<string>();   // the list for backup dependee list of the name
            bool backup_WasThereName = false;                       // check whether cell has previous content or not 
            object prevValue = "";                                  // store previous content of the cell

            // If name cell already has content
            if (spreadSheetCell.ContainsKey(name))
            {
                prevValue = spreadSheetCell[name].CellValue;        // save previous data(content)
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
                    spreadSheetCell.Add(name, new Cell(fma));
                    // set the dependency graph according to the formula variables.
                    foreach (string str in fma.GetVariables())
                        dg.AddDependency(str, name);
                }
                // set an existing cell
                else
                {
                    spreadSheetCell[name].CellValue = fma;
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
    }
}