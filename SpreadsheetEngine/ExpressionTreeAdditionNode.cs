using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Class representing an addition operator.
    /// </summary>
    public class ExpressionTreeAdditionNode : ExpressionTreeOperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTreeAdditionNode"/> class.
        /// </summary>
        public ExpressionTreeAdditionNode()
            : base('+', 7, Associativity.Left)
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
            return left + right;
        }
    }
}
