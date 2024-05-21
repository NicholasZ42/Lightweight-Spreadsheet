using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Class to do and undo a background change command for a cell.
    /// </summary>
    public class BackgroundChangeCommand : ICommand
    {
        /// <summary>
        /// Cell to perform commands on.
        /// </summary>
        private Cell cell;

        /// <summary>
        /// Cell's previous text.
        /// </summary>
        private uint oldBG;

        /// <summary>
        /// Cells text in the next command iteration.
        /// </summary>
        private uint currBG;

        /// <summary>
        /// Tells user what is going to be undone/redone.
        /// </summary>
        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundChangeCommand"/> class.
        /// </summary>
        /// <param name="cell"> Cell to change. </param>
        /// <param name="oldBG"> Old background color of cell. </param>
        public BackgroundChangeCommand(Cell cell, uint oldBG)
        {
            this.cell = cell;
            this.currBG = cell.BGColor;
            this.oldBG = oldBG;
            this.title = "Background change";
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
            this.cell.BGColor = this.currBG;
        }

        /// <summary>
        /// Changes the background of the cell to what it was before.
        /// </summary>
        public void Unexecute()
        {
            this.cell.BGColor = this.oldBG;
        }
    }
}
