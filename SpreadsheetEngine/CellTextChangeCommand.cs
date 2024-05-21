using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Class to do and undo a cell text change for a cell.
    /// </summary>
    public class CellTextChangeCommand : ICommand
    {
        /// <summary>
        /// Cell to perform commands on.
        /// </summary>
        private Cell cell;

        /// <summary>
        /// Cell's previous text.
        /// </summary>
        private string oldText;

        /// <summary>
        /// Cells text in the next command iteration.
        /// </summary>
        private string currText;

        /// <summary>
        /// Tells user what is going to be undone/redone.
        /// </summary>
        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellTextChangeCommand"/> class.
        /// </summary>
        /// <param name="cell"> Cell to change. </param>
        /// <param name="oldText"> old text of the cell. </param>
        public CellTextChangeCommand(Cell cell, string oldText)
        {
            this.cell = cell;
            this.currText = cell.Text;
            this.oldText = oldText;
            this.title = "Cell text change";
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title
        {
            get => this.title;
        }

        /// <summary>
        /// Changes the text of the cell to the new text.
        /// </summary>
        public void Execute()
        {
            this.cell.Text = this.currText;
        }

        /// <summary>
        /// Changes the text of the cell to what it was before.
        /// </summary>
        public void Unexecute()
        {
            this.cell.Text = this.oldText;
        }
    }
}
