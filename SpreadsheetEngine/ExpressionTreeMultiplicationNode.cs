using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Class representing a multiplication operator.
    /// </summary>
    public class ExpressionTreeMultiplicationNode : ExpressionTreeOperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTreeMultiplicationNode"/> class.
        /// </summary>
        public ExpressionTreeMultiplicationNode()
            : base('*', 6, Associativity.Left)
        {
        }

        /// <summary>
        /// See Evaluate in ExpressionTreeNode summary.
        /// </summary>
        /// <returns> See return value in Evaluate in ExpressionTreeNode. </returns>
        public override double Evaluate()
        {
            double left = this.Left.Evaluate();
            double right = this.Right.Evaluate();
            return left * right;
        }
    }
}
