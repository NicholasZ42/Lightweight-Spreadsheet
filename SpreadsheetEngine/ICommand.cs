using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Interface for a command.
    /// </summary>
    public interface ICommand
    {
        string Title
        {
            get;
        }

        /// <summary>
        /// Actions to take to execute method.
        /// </summary>
        void Execute();

        /// <summary>
        /// Actions to take to undo the actions of a command.
        /// </summary>
        void Unexecute();
    }
}
