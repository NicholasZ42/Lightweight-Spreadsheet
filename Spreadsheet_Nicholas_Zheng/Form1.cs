// <copyright file="Form1.cs" company="Nicholas Zheng">
// Copyright (c) Nicholas Zheng. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SpreadsheetEngine;

namespace Spreadsheet_Nicholas_Zheng
{
    /// <summary>
    /// Form to display a spreadsheet.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Spreadsheet class representing information to be displayed.
        /// </summary>
        private Spreadsheet spreadsheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();
            this.InitializeDataGrid();

            this.spreadsheet = new Spreadsheet(50, 26);
            this.spreadsheet.CellPropertyChanged += this.Spreadsheet_CellPropertyChanged;

            // Subscribe cell begin and end event handlers.
            this.dataGridView1.CellBeginEdit += this.dataGridView1_CellBeginEdit;
            this.dataGridView1.CellEndEdit += this.dataGridView1_CellEndEdit;

            // Disable undo and redo buttons as no commands have been executed yet
            this.undoCellBackgroundChangeToolStripMenuItem.Enabled = false;
            this.redoCellTextChangeToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Initializes the spreadsheet with a given number of rows and columns.
        /// </summary>
        public void InitializeDataGrid()
        {
            // Initialize columns
            this.dataGridView1.Columns.Clear();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                this.dataGridView1.Columns.Add(string.Empty + c, string.Empty + c);
            }

            // Initialize rows 1 - 50
            this.dataGridView1.Rows.Clear();
            this.dataGridView1.Rows.Add(50);

            // Update row headers to display row numbers
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                this.dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
        }

        /// <summary>
        /// Updates cell in the UI with the new value in the SpreadsheetCell.
        /// </summary>
        /// <param name="sender"> Object who broad casted the property change. </param>
        /// <param name="e"> What properties were changed. </param>
        private void Spreadsheet_CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                SpreadsheetCell cell = sender as SpreadsheetCell;

                this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Value;
            }

            if (e.PropertyName == "Background")
            {
                SpreadsheetCell cell = sender as SpreadsheetCell;

                this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Style.BackColor = System.Drawing.Color.FromArgb((int)cell.BGColor);
            }
        }

        /// <summary>
        /// Unused method.
        /// </summary>
        /// <param name="sender"> The broadcaster class. </param>
        /// <param name="e"> What parameters were changed. </param>
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Demo of the spreadsheet.
        /// </summary>
        /// <param name="sender"> N/A. </param>
        /// <param name="e"> Unknown. </param>
        private void button1_Click(object sender, EventArgs e)
        {
            // Set 50 cells with string "Hello World"
            string s = "Hello World";
            Random random = new Random();
            for (int i = 0; i < 50; i++)
            {
                int randomRow = random.Next(0, 50);
                int randomCol = random.Next(2, 26);
                this.spreadsheet.GetCell(randomRow, randomCol).Text = s;
            }

            // Set the text in every cell in column B to “This is cell B#”, where #
            // number is the row number for the cell.
            for (int i = 1; i <= 50; ++i)
            {
                this.spreadsheet.GetCell(i - 1, 1).Text = $"This is cell B{i}";
            }

            // Set the text in every cell in column A to “=B#”, where ‘#’ is the row number of the
            // cell.
            for (int i = 1; i <= 50; ++i)
            {
                this.spreadsheet.GetCell(i - 1, 0).Text = $"=B{i}";
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        /// <summary>
        /// Event handler for when cell is beginning editing.
        /// </summary>
        /// <param name="sender"> Object which is being edited. </param>
        /// <param name="e"> Data grid view cell which is being edited. </param>
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            SpreadsheetCell spreadsheetCell = (SpreadsheetCell)this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);
            this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = spreadsheetCell.Text;
        }

        /// <summary>
        /// Event handler for when cell is finished editing.
        /// </summary>
        /// <param name="sender"> Object which is being edited. </param>
        /// <param name="e">  Data grid view cell which is being edited. </param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SpreadsheetCell spreadsheetCell = (SpreadsheetCell)this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);

            // Change spreadsheet cell text and add it to list of commands to undo
            string oldText = spreadsheetCell.Text;
            spreadsheetCell.Text = (string)this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            List<ICommand> commands = new List<ICommand>();
            commands.Add(new CellTextChangeCommand(spreadsheetCell, oldText));
            this.spreadsheet.AddUndo(commands);

            this.UpdateUndoButton();

            this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = spreadsheetCell.Value;
        }

        /// <summary>
        /// Event handling for when the "Change Background Color" button is clicked.
        /// </summary>
        /// <param name="sender"> N/A. </param>
        /// <param name="e"> Event properties. </param>
        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog myDialog = new ColorDialog();

            // Keeps the user from selecting a custom color.
            myDialog.AllowFullOpen = false;

            // Allows the user to get help. (The default is false.)
            myDialog.ShowHelp = true;

            // Sets the initial color select to the current text color.
            myDialog.Color = this.dataGridView1.Rows[0].Cells[0].Style.BackColor;

            // Update all selected cells with new color.
            if (myDialog.ShowDialog() == DialogResult.OK)
            {
                List<ICommand> commands = new List<ICommand>();

                foreach (DataGridViewCell c in this.dataGridView1.SelectedCells)
                {
                    Cell spreadsheetCell = this.spreadsheet.GetCell(c.RowIndex, c.ColumnIndex);
                    uint oldBG = spreadsheetCell.BGColor;
                    spreadsheetCell.BGColor = (uint)myDialog.Color.ToArgb();
                    commands.Add(new BackgroundChangeCommand(spreadsheetCell, oldBG));
                }

                this.spreadsheet.AddUndo(commands);
                this.UpdateUndoButton();
            }
        }

        /// <summary>
        /// Undoes the most recent command.
        /// </summary>
        /// <param name="sender"> Not used. </param>
        /// <param name="e"> N/A. </param>
        private void undoCellBackgroundChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadsheet.Undo();

            if (this.spreadsheet.UndosEmpty())
            {
                this.undoCellBackgroundChangeToolStripMenuItem.Enabled = false;
            }
            else
            {
                this.UpdateUndoButton();
            }

            if (!this.spreadsheet.RedosEmpty())
            {
                this.UpdateRedoButton();
            }
        }

        /// <summary>
        /// Updates the undo button with a new title.
        /// </summary>
        private void UpdateUndoButton()
        {
            this.undoCellBackgroundChangeToolStripMenuItem.Text = "Undo " + this.spreadsheet.GetTopUndoCommandTitle();
            this.undoCellBackgroundChangeToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// Redoes a command.
        /// </summary>
        /// <param name="sender"> Not used. </param>
        /// <param name="e"> N/A.</param>
        private void redoCellTextChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadsheet.Redo();

            if (this.spreadsheet.RedosEmpty())
            {
                this.redoCellTextChangeToolStripMenuItem.Enabled = false;
            }
            else
            {
                this.UpdateRedoButton();
            }

            if (!this.spreadsheet.UndosEmpty())
            {
                this.UpdateUndoButton();
            }
        }

        /// <summary>
        /// Updates the redo button with the action of the top command on the redo stack.
        /// </summary>
        private void UpdateRedoButton()
        {
            this.redoCellTextChangeToolStripMenuItem.Enabled = true;
            this.redoCellTextChangeToolStripMenuItem.Text = "Redo " + this.spreadsheet.GetTopRedoCommandTitle();
        }

        /// <summary>
        /// Loads xml data into the spreadsheet.
        /// </summary>
        /// <param name="sender"> Object that raised event. </param>
        /// <param name="e"> Event parameters. </param>
        private void loadFromXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.spreadsheet.Load(openFileDialog.FileName);
                }
            }
        }

        /// <summary>
        /// Saves the current spreadsheet to a file.
        /// </summary>
        /// <param name="sender"> Object that raised event. </param>
        /// <param name="e"> Event arguments. </param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    this.spreadsheet.Save(myStream);
                }
            }
        }
    }
}
