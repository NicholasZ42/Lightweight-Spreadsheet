// <copyright file="ExpressionTreeOperatorNode.cs" company="Nicholas Zheng">
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
    /// Represents the associativity of a operator.
    /// </summary>
    public enum Associativity
    {
        /// <summary>
        /// Left associative.
        /// </summary>
        Left,

        /// <summary>
        /// Non associative.
        /// </summary>
        Non,

        /// <summary>
        /// Right associative.
        /// </summary>
        Right,
    }

    /// <summary>
    /// Node for an operator in the ExpressionTree.
    /// </summary>
    public abstract class ExpressionTreeOperatorNode : ExpressionTreeNode
    {
        /// <summary>
        /// The precedence of the operator.
        /// </summary>
        protected readonly int precedence;

        /// <summary>
        /// False means left associative, otherwise right associative.
        /// </summary>
        protected readonly Associativity associativity;

        /// <summary>
        /// Operand in the ExpressionTree.
        /// </summary>
        private char @operator;

        /// <summary>
        /// ExpressionTreeNode to the left of the current.
        /// </summary>
        private ExpressionTreeNode left;

        /// <summary>
        /// ExpressionTreeNode to the right of the current.
        /// </summary>
        private ExpressionTreeNode right;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTreeOperatorNode"/> class.
        /// </summary>
        /// <param name="operand"> What operator the node represents. </param>
        /// <param name="precedence"> See precedence field. </param>
        /// <param name="associativity"> See associativity field. </param>
        public ExpressionTreeOperatorNode(char operand, int precedence, Associativity associativity)
        {
            this.@operator = operand;
            this.left = null;
            this.right = null;
            this.precedence = precedence;
            this.associativity = associativity;
        }

        /// <summary>
        /// Gets or sets left.
        /// </summary>
        public ExpressionTreeNode Left
        {
            get => this.left;
            set => this.left = value;
        }

        /// <summary>
        /// Gets or sets right.
        /// </summary>
        public ExpressionTreeNode Right
        {
            get => this.right;
            set => this.right = value;
        }

        /// <summary>
        /// Gets operand.
        /// </summary>
        public char Operator
        {
            get => this.@operator;
        }

        /// <summary>
        /// Gets the precedence.
        /// </summary>
        public int Precedence
        {
            get => this.precedence;
        }

        /// <summary>
        /// Gets the associativity.
        /// </summary>
        public Associativity Associativity
        {
            get => this.associativity;
        }

        /// <summary>
        /// Tries to evaluate the left and right sides.
        /// </summary>
        /// <param name="left"> Variable to hold left side value. </param>
        /// <param name="right"> Variable to hold right side value. </param>
        protected void TryEvaluate(ref double left, ref double right)
        {
            try
            {
                left = this.Left.Evaluate();
                right = this.Right.Evaluate();
            }
            catch (VariableNotSetException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
