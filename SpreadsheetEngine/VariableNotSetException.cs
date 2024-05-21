﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Class to indicate a variable has not been set.
    /// </summary>
    public class VariableNotSetException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNotSetException"/> class.
        /// </summary>
        public VariableNotSetException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNotSetException"/> class.
        /// </summary>
        /// <param name="message"> String representation of the message. </param>
        public VariableNotSetException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNotSetException"/> class.
        /// </summary>
        /// <param name="message"> String representation of the expression. </param>
        /// <param name="innerException"> Previous exception. </param>
        public VariableNotSetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
