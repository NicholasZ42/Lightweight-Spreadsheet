// <copyright file="ExpressionTreeConstantNode.cs" company="Nicholas Zheng">
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
    /// Constant in the ExpressionTree.
    /// </summary>
    public class ExpressionTreeConstantNode : ExpressionTreeNode
    {
        /// <summary>
        /// Value of the constant.
        /// </summary>
        private readonly double value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTreeConstantNode"/> class.
        /// </summary>
        /// <param name="value"> Value of the constant. </param>
        public ExpressionTreeConstantNode(double value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value of the constant.
        /// </summary>
        public double Value
        {
            get => this.value;
        }

        /// <summary>
        /// See Evaluate in ExpressionTreeNode summary.
        /// </summary>
        /// <returns> See return value in Evaluate in ExpressionTreeNode. </returns>
        public override double Evaluate()
        {
            return this.value;
        }
    }
}
