using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Factory class for operator nodes.
    /// </summary>
    public class OperatorNodeFactory
    {
        /// <summary>
        /// Dictionary containing all operators.
        /// </summary>
        private Dictionary<char, Type> operators = new Dictionary<char, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNodeFactory"/> class.
        /// </summary>
        public OperatorNodeFactory()
        {
            this.TraverseAvailableOperators((op, type) => this.operators.Add(op, type));
        }

        /// <summary>
        /// Delegate for what action to do on a certain operator.
        /// </summary>
        /// <param name="op"> Character representing operator. </param>
        /// <param name="type"> Type of operator. </param>
        private delegate void OnOperator(char op, Type type);

        /// <summary>
        /// Gets operands.
        /// </summary>
        public Dictionary<char, Type> Operators
        {
            get => this.operators;
        }

        /// <summary>
        /// Creates an operator node based on the operator.
        /// </summary>
        /// <param name="op"> Character representing operator. </param>
        /// <returns> An ExpressionTreeOperatorNode representing op. </returns>
        public ExpressionTreeOperatorNode CreateOperatorNode(char op)
        {
            // Create new operator node depending on operator.
            if (this.operators.ContainsKey(op))
            {
                object operatorNodeObject = System.Activator.CreateInstance(this.operators[op]);
                if (operatorNodeObject is ExpressionTreeOperatorNode)
                {
                    return (ExpressionTreeOperatorNode)operatorNodeObject;
                }
            }

            throw new Exception("Unhandled operator");
        }

        /// <summary>
        /// Traverses assembly and populates operator dictionary.
        /// </summary>
        /// <param name="onOperator"> Delegate which adds operator to dictionary. </param>
        private void TraverseAvailableOperators(OnOperator onOperator)
        {
            // get the type declaration of OperatorNode
            Type operatorNodeType = typeof(ExpressionTreeOperatorNode);

            // Iterate over all loaded assemblies:
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Get all types that inherit from our OperatorNode class using LINQ
                IEnumerable<Type> operatorTypes =
                assembly.GetTypes().Where(type => type.IsSubclassOf(operatorNodeType));

            // Iterate over those subclasses of OperatorNode
                foreach (var type in operatorTypes)
                    {
                        // for each subclass, retrieve the Operator property
                        PropertyInfo operatorField = type.GetProperty("Operator");
                        if (operatorField != null)
                        {
                        // Get the character of the Operator
                        // object value = operatorField.GetValue(type);
                        // If “Operator” property is not static, you will need to create
                        // an instance first and use the following code instead (or similar):
                        object value = operatorField.GetValue(Activator.CreateInstance(type));
                        if (value is char)
                            {
                                char operatorSymbol = (char)value;

                                // And invoke the function passed as parameter
                                // with the operator symbol and the operator class
                                onOperator(operatorSymbol, type);
                            }
                        }
                    }
            }
        }
    }
}
