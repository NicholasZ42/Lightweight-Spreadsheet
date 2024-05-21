// <copyright file="ExpressionTreeVariableNode.cs" company="Nicholas Zheng">
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
    /// Variable in an ExpressionTree.
    /// </summary>
    public class ExpressionTreeVariableNode : ExpressionTreeNode
    {
        /// <summary>
        /// Name of the variable.
        /// </summary>
        private string variableName;

        /// <summary>
        /// Dictionary holding values of all variables.
        /// </summary>
        private Dictionary<string, double> variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTreeVariableNode"/> class.
        /// </summary>
        /// <param name="variableName"> Name of the variable. </param>
        /// /// <param name="variables"> Reference to dictionary containing variable values. </param>
        public ExpressionTreeVariableNode(string variableName, ref Dictionary<string, double> variables)
        {
            this.variableName = variableName;
            this.variables = variables;
        }

        /// <summary>
        /// Gets or sets the variableName.
        /// </summary>
        public string VariableName
        {
            get => this.variableName;
            set => this.variableName = value;
        }

        /// <summary>
        /// See Evaluate in ExpressionTreeNode summary.
        /// </summary>
        /// <returns> See return value in Evaluate in ExpressionTreeNode. </returns>
        public override double Evaluate()
        {
            if (this.variables.ContainsKey(this.variableName))
            {
                return this.variables[this.variableName];
            }
            else
            {
                throw new VariableNotSetException("Variable " + this.variableName + " has not been set!");
            }
        }
    }
}
