// <copyright file="ExpressionTreeNode.cs" company="Nicholas Zheng">
// Copyright (c) Nicholas Zheng. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Node inside the ExpressionTree class.
    /// </summary>
    public abstract class ExpressionTreeNode
    {
        /// <summary>
        /// Evaluates the Node to a value.
        /// </summary>
        /// <returns> A double representing the value of the evaluated node. </returns>
        public abstract double Evaluate();
    }
}
