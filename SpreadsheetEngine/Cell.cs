// <copyright file="Cell.cs" company="Nicholas Zheng">
// Copyright (c) Nicholas Zheng. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Represents one cell in a spreadsheet class.
    /// </summary>
    public abstract class Cell : INotifyPropertyChanged
    {
        /// <summary>
        /// Evaluated value of the cell. If the text doesn't start with '=',
        /// it will be the text value. Otherwise, it will be the formula the user
        /// entered.
        /// </summary>
        protected string value;

        /// <summary>
        /// Text that user enters.
        /// </summary>
        protected string text;

        /// <summary>
        /// Row index in the spreadsheet.
        /// </summary>
        private int rowIndex;

        /// <summary>
        /// Column index in spreadsheet.
        /// </summary>
        private int columnIndex;

        /// <summary>
        /// Background color.
        /// </summary>
        private uint backgroundColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="rowIndex"> See documentation for rowIndex. </param>
        /// <param name="columnIndex"> See documentation for columnIndex. </param>
        public Cell(int rowIndex, int columnIndex)
        {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.backgroundColor = 0xFFFFFFFF;
            this.text = string.Empty;
    }

        /// <summary>
        /// List of subscribers to the current class.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// List of subscribers to the current class.
        /// </summary>
        public event PropertyChangedEventHandler FormulaChanged = (sender, e) => { };

        /// <summary>
        /// Gets row index. Read-only.
        /// </summary>
        public int RowIndex
        {
            get => this.rowIndex;
        }

        /// <summary>
        /// Gets column index. Read-only.
        /// </summary>
        public int ColumnIndex
        {
            get => this.columnIndex;
        }

        /// <summary>
        /// Gets or sets the text value. Notifies subscribers if the text value is changed.
        /// </summary>
        public string Text
        {
            get => this.text;

            set
            {
                if (value == this.text)
                {
                    return;
                }

                this.text = value;

                // Call all subscribed delegates in list
                this.PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                this.FormulaChanged(this, new PropertyChangedEventArgs("Formula"));
            }
        }

        /// <summary>
        /// Gets the value. Notifies subscribers if the text value is changed.
        /// Only subscribers internal to the project can set it. This is intended
        /// to be only the Spreadsheet class and the children of this class.
        /// </summary>
        public string Value
        {
            get => this.value;

            internal set
            {
                if (value == this.value)
                {
                    return;
                }

                this.value = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        /// <summary>
        /// Gets or sets the background color.
        /// Notifies subscribers if its value is changed.
        /// </summary>
        public uint BGColor
        {
            get => this.backgroundColor;
            set
            {
                if (value == this.backgroundColor)
                {
                    return;
                }

                this.backgroundColor = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("Background"));
            }
        }

        /// <summary>
        /// Event handler for when the current cell's value has changed and it is being used in the formula of a different cell.
        /// Ex: The formula for a different cell is A1 + 1 and A1's value has changed. A1 is the current cell.
        /// </summary>
        /// <param name="sender"> The cell whose value has changed. </param>
        /// <param name="e"> What properties were changed. </param>
        public void Formula_Changed(object sender, PropertyChangedEventArgs e)
        {
            string prevVal = this.value;

            // Update cell
            this.PropertyChanged(this, new PropertyChangedEventArgs("Formula"));

            if (this.value != prevVal)
            {
                // Notify subscribers to update value of parent cell.
                this.FormulaChanged(this, new PropertyChangedEventArgs("Formula"));
            }
        }
    }
}
