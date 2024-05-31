// <copyright file="Spreadsheet.cs" company="Nicholas Zheng">
// Copyright (c) Nicholas Zheng. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Represents a spreadsheet.
    /// </summary>
    public class Spreadsheet
    {
        private const string SELF_REFERENCE = "!(self reference)";

        private const string BAD_REFERENCE = "!(bad reference)";

        private const string CIRCULAR_REFERENCE = "!(circular reference)";

        /// <summary>
        /// 2-D array containing all cells in the spreadsheet.
        /// </summary>
        private readonly SpreadsheetCell[,] spreadsheet;

        /// <summary>
        /// Column count of spreadsheet.
        /// </summary>
        private readonly int columnCount;

        /// <summary>
        /// Row count of spreadsheet.
        /// </summary>
        private readonly int rowCount;

        /// <summary>
        /// Stack of commands to undo.
        /// </summary>
        private Stack<List<ICommand>> undos;

        /// <summary>
        /// Stack of commands to redo.
        /// </summary>
        private Stack<List<ICommand>> redos;

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        /// <param name="rows"> Number of rows. </param>
        /// <param name="cols">Number of cols. </param>
        public Spreadsheet(int rows, int cols)
        {
            this.spreadsheet = new SpreadsheetCell[rows, cols];
            this.columnCount = cols;
            this.rowCount = rows;

            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    this.spreadsheet[row, col] = new SpreadsheetCell(row, col);
                    this.spreadsheet[row, col].PropertyChanged += this.Cell_PropertyChanged;
                }
            }

            this.undos = new Stack<List<ICommand>>();
            this.redos = new Stack<List<ICommand>>();
        }

        /// <summary>
        /// Event for when a cell property is changed.
        /// </summary>
        public event PropertyChangedEventHandler CellPropertyChanged = delegate { };

        /// <summary>
        /// Gets the rowCount.
        /// </summary>
        public int RowCount
        {
            get => this.rowCount;
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        public int ColumnCount
        {
            get => this.columnCount;
        }

        /// <summary>
        /// Gets the title of the top command on the undo stack.
        /// </summary>
        /// <returns>  Title of the top command on the undo stack. </returns>
        public string GetTopUndoCommandTitle()
        {
            if (this.undos.Count == 0)
            {
                return null;
            }

            return this.undos.Peek()[0].Title;
        }

        /// <summary>
        /// See return.
        /// </summary>
        /// <returns> Whether undo stack is empty. </returns>
        public bool UndosEmpty()
        {
            return this.undos.Count == 0;
        }

        /// <summary>
        /// Gets the title of the top command on the redo stack.
        /// </summary>
        /// <returns>  Title of the top command on the redo stack. </returns>
        public string GetTopRedoCommandTitle()
        {
            if (this.redos.Count == 0)
            {
                return null;
            }

            return this.redos.Peek()[0].Title;
        }

        /// <summary>
        /// See return.
        /// </summary>
        /// <returns> Whether redo stack is empty. </returns>
        public bool RedosEmpty()
        {
            return this.redos.Count == 0;
        }

        /// <summary>
        /// Returns cell at location or null if there is no such cell.
        /// </summary>
        /// <param name="rowInd"> Row index in spreadsheet. </param>
        /// <param name="colInd"> Column index in spreadsheet. </param>
        /// <returns> Cell at location if it exists. </returns>
        public Cell GetCell(int rowInd, int colInd)
        {
            int rows = this.spreadsheet.GetLength(0);
            int cols = this.spreadsheet.GetLength(1);

            if (rowInd < 0 || rowInd > rows - 1)
            {
                return null;
            }

            if (colInd < 0 || colInd > cols - 1)
            {
                return null;
            }

            return this.spreadsheet[rowInd, colInd];
        }

        /// <summary>
        /// Returns cell at location or null if there is no such cell.
        /// </summary>
        /// <param name="cellName"> Name of the cell. Ex: A1. </param>
        /// <returns> Cell at location if it exists. </returns>
        public Cell GetCell(string cellName)
        {
            if (cellName.Length < 2)
            {
                return null;
            }

            char col = cellName[0];

            if (col < 'A' || col > 'Z')
            {
                return null;
            }

            int colInd = col - 'A';

            if (int.TryParse(cellName.Substring(1), out int parsedValue))
            {
                int row = parsedValue;
                return this.GetCell(row - 1, colInd);
            }

            return null;
        }

        /// <summary>
        /// Adds a command to the undo stack.
        /// </summary>
        /// <param name="commands"> The commands to add to the undo stack. </param>
        public void AddUndo(List<ICommand> commands)
        {
            this.undos.Push(commands);
        }

        /// <summary>
        /// Undoes the command at the top of the undo stack if the undo
        /// stack is nonempty.
        /// </summary>
        public void Undo()
        {
            if (this.undos.Count != 0)
            {
                List<ICommand> commands = this.undos.Pop();

                foreach (ICommand c in commands)
                {
                    c.Unexecute();
                }

                this.redos.Push(commands);
            }
        }

        /// <summary>
        /// Redoes the command at the top of the redo stack if the redo
        /// stack is nonempty.
        /// </summary>
        public void Redo()
        {
            if (this.redos.Peek()[0] != null)
            {
                List<ICommand> commands = this.redos.Pop();

                foreach (ICommand c in commands)
                {
                    c.Execute();
                }

                this.undos.Push(commands);
            }
        }

        /// <summary>
        /// Saves the spreadsheet to a XML file.
        /// </summary>
        /// <param name="stream"> Stream to save to. </param>
        public void Save(Stream stream)
        {
            // Create the XmlDocument.
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("root");

            foreach (SpreadsheetCell spreadsheetCell in this.spreadsheet)
            {
                if (this.ContainsNonDefaultProperties(spreadsheetCell))
                {
                    XmlElement cell = doc.CreateElement("cell");

                    XmlElement rowIndex = doc.CreateElement("rowIndex");
                    rowIndex.InnerText = spreadsheetCell.RowIndex.ToString();

                    XmlElement colIndex = doc.CreateElement("colIndex");
                    colIndex.InnerText = spreadsheetCell.ColumnIndex.ToString();

                    XmlElement bgColor = doc.CreateElement("bgColor");
                    bgColor.InnerText = spreadsheetCell.BGColor.ToString();

                    XmlElement text = doc.CreateElement("text");
                    text.InnerText = spreadsheetCell.Text;

                    cell.AppendChild(rowIndex);
                    cell.AppendChild(colIndex);
                    cell.AppendChild(bgColor);
                    cell.AppendChild(text);
                    root.AppendChild(cell);
                }
            }

            // Save the document to a file. White space is
            // preserved (no white space).
            doc.PreserveWhitespace = true;
            doc.AppendChild(root);
            doc.Save(stream);
            stream.Close();
        }

        /// <summary>
        /// Resets the spreadsheet and loads content into it from a XML file.
        /// Then, it resets the undo and redo stacks.
        /// </summary>
        /// <param name="fileName"> file to load from. </param>
        public void Load(string fileName)
        {
            this.ClearSpreadsheetData();
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            foreach (XmlElement n in doc.ChildNodes[0].ChildNodes)
            {
                XmlNodeList rowIndexList = n.GetElementsByTagName("rowIndex");
                int rowIndex = -1;
                int.TryParse(rowIndexList[0].InnerText, out rowIndex);

                XmlNodeList colIndexList = n.GetElementsByTagName("colIndex");
                int colIndex = -1;
                int.TryParse(colIndexList[0].InnerText, out colIndex);

                if (rowIndex != -1 && colIndex != -1)
                {
                    XmlNodeList bgColorList = n.GetElementsByTagName("bgColor");
                    uint bgColor = 0xFFFFFFFF;
                    uint.TryParse(bgColorList[0].InnerText, out bgColor);
                    this.spreadsheet[rowIndex, colIndex].BGColor = bgColor;

                    XmlNodeList textList = n.GetElementsByTagName("text");
                    this.spreadsheet[rowIndex, colIndex].Text = textList[0].InnerText;
                }
                else
                {
                    continue;
                }
            }

            this.ClearStacks();
        }

        /// <summary>
        /// Clears the background and text fields for each spreadsheet cell
        /// within the spreadsheet.
        /// </summary>
        private void ClearSpreadsheetData()
        {
            foreach (SpreadsheetCell spreadsheetCell in this.spreadsheet)
            {
                spreadsheetCell.Text = string.Empty;
                spreadsheetCell.BGColor = 0xFFFFFFFF;
            }
        }

        /// <summary>
        /// Clears the undo and redo stacks.
        /// </summary>
        private void ClearStacks()
        {
            this.undos.Clear();
            this.redos.Clear();
        }

        /// <summary>
        /// Determines if a spreadsheet cell contains non-default properties.
        /// </summary>
        /// <returns> True if spreadsheet cell contains non-default properties, false otherwise. </returns>
        /// /// <param name="spreadsheetCell"> Spreadsheet cell to investigate. /param>.
        private bool ContainsNonDefaultProperties(SpreadsheetCell spreadsheetCell)
        {
            return spreadsheetCell.Text != string.Empty || spreadsheetCell.BGColor != 0xFFFFFFFF;
        }

        /// <summary>
        /// Event handling behavior for when a cell property is changed.
        /// </summary>
        /// <param name="sender"> Spreadsheet cell. </param>
        /// <param name="e"> Event info. </param>
        private void Cell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Background")
            {
                this.CellPropertyChanged(sender, new PropertyChangedEventArgs("Background"));
                return;
            }

            if (e.PropertyName == "Text")
            {
                SpreadsheetCell spreadsheetCell = sender as SpreadsheetCell;

                if (!this.ReferenceCheck(spreadsheetCell))
                {
                    this.UpdateValue(ref sender);
                    this.SubscribeToCellsInFormula(ref spreadsheetCell);
                }
            }

            if (e.PropertyName == "Formula")
            {
                SpreadsheetCell spreadsheetCell = sender as SpreadsheetCell;
                if (!this.ReferenceCheck(spreadsheetCell))
                {
                    this.UpdateValue(ref sender);
                }
            }

            this.CellPropertyChanged(sender, new PropertyChangedEventArgs("Value"));
        }

        /// <summary>
        /// Updates the value of the cell.
        /// </summary>
        /// <param name="sender"> Cell to be updated. </param>
        private void UpdateValue(ref object sender)
        {
            SpreadsheetCell cell = sender as SpreadsheetCell;

            if (cell.Text != null && cell.Text != string.Empty && cell.Text.ToString()[0] == '=')
            {
                cell.Value = this.ComputeFormula(cell);
            }
            else
            {
                cell.Value = cell.Text;
            }
        }

        /// <summary>
        /// Determines whether a cell contains a reference error.
        /// </summary>
        /// <param name="cell"> The cell in question. </param>
        /// <returns> True if it does, false otherwise. </returns>
        private bool ReferenceCheck(SpreadsheetCell cell)
        {
            if (this.SelfReference(cell))
            {
                cell.Value = SELF_REFERENCE;
                return true;
            }
            else if (this.DNEReference(cell))
            {
                cell.Value = BAD_REFERENCE;
                return true;
            }
            else if (this.CircularReference(cell))
            {
                cell.Value = CIRCULAR_REFERENCE;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Extracts all the cell references in the formula of a cell.
        /// </summary>
        /// <param name="formula"> The formula of the cell. </param>
        /// <returns> A list of all the cell references. </returns>
        private List<string> ExtractCellReferences(string formula)
        {
            string pattern = @"\b[A-Za-z]+\d*\b";
            MatchCollection matches = Regex.Matches(formula, pattern);
            List<string> cellReferences = new List<string>();

            if (formula != null && formula != string.Empty && formula[0] != '=')
            {
                return cellReferences;
            }

            for (int i = 0; i < matches.Count; i++)
            {
                cellReferences.Add(matches[i].Value);
            }

            return cellReferences;
        }

        /// <summary>
        /// Determines whether a cell's formula references a cell that does not exist
        /// in the spreadsheet.
        /// </summary>
        /// <param name="cell"> The cell in question. </param>
        /// <returns> True if the cell references a cell that DNE, otherwise false. </returns>
        private bool DNEReference(SpreadsheetCell cell)
        {
            List<string> cellNames = this.ExtractCellReferences(cell.Text);

            foreach (string formulaCell in cellNames)
            {
                Cell spreadsheetCell = this.GetCell(formulaCell);
                if (spreadsheetCell == null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether a cell contains a self reference in its formula.
        /// </summary>
        /// <param name="cell"> The cell in question. </param>
        /// <returns> True if the cell contains a self reference, false otherwise. </returns>
        private bool SelfReference(SpreadsheetCell cell)
        {
            List<string> cellNames = this.FindCellNames(cell.Text);

            foreach (string formulaCell in cellNames)
            {
                Cell spreadsheetCell = this.GetCell(formulaCell);
                if (spreadsheetCell as SpreadsheetCell == cell)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Detects a circular reference in the formula of a spreadsheet cell.
        /// </summary>
        /// <param name="cell"> The cell in question. </param>
        /// <returns> True if a circular reference was detected, false otherwise. </returns>
        private bool CircularReference(SpreadsheetCell cell)
        {
            bool res = false;
            this.CircularReferenceHelper(cell, cell, ref res);
            return res;
        }

        /// <summary>
        /// Goes through each of the cells in the formula of the current cell. Traces the references in each cell
        /// to see if they point back to the original cell.
        /// </summary>
        /// <param name="cell"> The current cell in question. </param>
        /// <param name="originalCell"> The original cell in the formula. </param>
        /// <param name="res"> Reference to the result. </param>
        private void CircularReferenceHelper(SpreadsheetCell cell, SpreadsheetCell originalCell, ref bool res)
        {
            List<string> cellNames = this.FindCellNames(cell.Text);

            foreach (string formulaCell in cellNames)
            {
                Cell fCell = this.GetCell(formulaCell);
                SpreadsheetCell spreadsheetCell = fCell as SpreadsheetCell;
                if (spreadsheetCell == originalCell)
                {
                    if (cell == originalCell)
                    {
                        return;
                    }
                    else
                    {
                        res = true;
                        return;
                    }
                }
                else if (spreadsheetCell.Value == SELF_REFERENCE || spreadsheetCell.Value == BAD_REFERENCE)
                {
                    return;
                }
                else if (spreadsheetCell.Value == CIRCULAR_REFERENCE)
                {
                    // If the formula references a circular reference cycle, it is considered a circular reference
                    res = true;
                    return;
                }

                bool formulaCellRes = false;
                this.CircularReferenceHelper(spreadsheetCell as SpreadsheetCell, originalCell, ref formulaCellRes);

                if (formulaCellRes == true)
                {
                    res = true;
                    return;
                }
            }

            res = false;
        }

        /// <summary>
        /// Finds the cell names in the formula. See if they match the pattern
        /// [uppercase letter][digit].
        /// </summary>
        /// <param name="formula"> String representation of the formula. </param>
        /// <returns> List of cell names in the formula. </returns>
        private List<string> FindCellNames(string formula)
        {
            string pattern = @"[A-Z]+\d+";

            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(formula);

            List<string> validCellNames = new List<string>();

            if (formula != null && formula != string.Empty && formula[0] != '=')
            {
                return validCellNames;
            }

            foreach (Match match in matches)
            {
                validCellNames.Add(match.Value);
            }

            return validCellNames;
        }

        /// <summary>
        /// Subscribes the cell to cells in its formula. This way, if any changes occur in the dependent cells,
        /// the original cell will update.
        /// </summary>
        /// <param name="cell"> Cell containing formula. </param>
        private void SubscribeToCellsInFormula(ref SpreadsheetCell cell)
        {
            List<string> cellNames = this.FindCellNames(cell.Text);

            foreach (string c in cellNames)
            {
                Cell spreadsheetCellInFormula = this.GetCell(c);
                spreadsheetCellInFormula.FormulaChanged += cell.Formula_Changed; // Subscribe to spreadsheetCellInFormula
            }
        }

        /// <summary>
        /// Computes the value of a cell if it is a formula.
        /// </summary>
        /// <param name="cell"> The cell containing the formula to be computed. </param>
        /// <returns> A string representing the value of the formula. </returns>
        private string ComputeFormula(SpreadsheetCell cell)
        {
            string formula = cell.Text.ToString().Substring(1);
            ExpressionTree expressionTree = new ExpressionTree(formula);

            List<string> cellNames = this.FindCellNames(cell.Text);

            if (cellNames.Count == 1 && formula == cellNames[0])
            {
                string cellValue = this.GetCell(cellNames[0]).Value;

                if (cellValue == null)
                {
                    return "0";
                }
                else
                {
                    return cellValue;
                }
            }

            // Set each of the variables inside the formula.
            foreach (string c in cellNames)
            {
                Cell spreadsheetCell = this.GetCell(c);
                double value = 0;

                if (spreadsheetCell.Value == SELF_REFERENCE || spreadsheetCell.Value == BAD_REFERENCE)
                {
                    return cell.Value;
                }

                bool isDouble = double.TryParse(spreadsheetCell.Value, out value);

                if (isDouble)
                {
                    expressionTree.SetVariable(c, value);
                }
            }

            return expressionTree.Evaluate().ToString();
        }
    }
}
