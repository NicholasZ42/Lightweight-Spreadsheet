using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetEngine
{
    /// <summary>
    /// An arithmetic expression parser that builds a tree for the expression.
    /// </summary>
    public class ExpressionTree
    {
        /// <summary>
        /// String representation of the expression entered by the user.
        /// </summary>
        private readonly string expression;

        /// <summary>
        /// Pointer to head of ExpressionTree.
        /// </summary>
        private readonly ExpressionTreeNode head;

        /// <summary>
        /// Dictionary to hold values of variables.
        /// </summary>
        private Dictionary<string, double> variables;

        /// <summary>
        /// Class to generate operator nodes.
        /// </summary>
        private OperatorNodeFactory operatorNodeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// </summary>
        /// <param name="expression"> A string representing an expression. Ex: "A1+B2". </param>
        public ExpressionTree(string expression)
        {
            this.operatorNodeFactory = new OperatorNodeFactory();
            this.variables = new Dictionary<string, double>();
            this.expression = expression;
            string outerParenthesesRemoved = this.RemoveOuterParentheses(this.expression);
            string parsedExpression = this.Parse(outerParenthesesRemoved);
            this.head = this.CreateTree(parsedExpression);
        }

        /// <summary>
        /// Gets the string representation of Expression.
        /// </summary>
        public string Expression
        {
            get => this.expression;
        }

        /// <summary>
        /// Prints an in order traversal of the tree. Used for debugging.
        /// </summary>
        public void InOrderTraversal()
        {
            this.InOrderTraversal(this.head);
        }

        /// <summary>
        /// Sets the specified variable within the ExpressionTree variables dictionary.
        /// </summary>
        /// <param name="variableName"> Name of variable. </param>
        /// <param name="variableValue"> Value of variable. </param>
        public void SetVariable(string variableName, double variableValue)
        {
            this.variables[variableName] = variableValue;
        }

        /// <summary>
        /// Evaluates the expression tree to a double value.
        /// </summary>
        /// <returns> The value of the expression tree. </returns>
        public double Evaluate()
        {
            if (this.head != null)
            {
                try
                {
                    return this.head.Evaluate();
                }
                catch (VariableNotSetException e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            return 0;
        }

        /// <summary>
        /// Removes the outer parentheses for an expression.
        /// Ex: (2+3) becomes 2+3.
        /// </summary>
        /// <param name="expression"> String representation of expression. </param>
        /// <returns> Expression without any outer parentheses. </returns>
        private string RemoveOuterParentheses(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return string.Empty;
            }

            // Check for extra parentheses and get rid of them, e.g. (((((2+3)-(4+5)))))
            if (expression[0] == '(')
            {
                int parenthesisCounter = 1;
                for (int characterIndex = 1; characterIndex < expression.Length; characterIndex++)
                {
                    // if open parenthesis increment a counter
                    if (expression[characterIndex] == '(')
                    {
                        parenthesisCounter++;
                    }

                    // if closed parenthesis decrement the counter
                    else if (expression[characterIndex] == ')')
                    {
                        parenthesisCounter--;

                        // if the counter is 0 check where we are
                        if (parenthesisCounter == 0)
                        {
                            if (characterIndex != expression.Length - 1)
                            {
                                // if we are not at the end, then get out (there are no extra parentheses)
                                return expression;
                            }
                            else
                            {
                                // Else get rid of the outer most parentheses and start over
                                return this.RemoveOuterParentheses(expression.Substring(1, expression.Length - 2));
                            }
                        }
                    }
                }
            }

            return expression;
        }

        /// <summary>
        /// Parses the expression and turns it from infix to postfix.
        /// </summary>
        /// <param name="expression"> Expression without parentheses surrounding it. </param>
        /// <returns> The expression in postfix form. </returns>
        private string Parse(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return string.Empty;
            }

            string res = string.Empty;
            Stack<char> s = new Stack<char>();
            int i = 0;

            while (i < expression.Length)
            {
                char c = expression[i];
                if ((c >= '0' && c <= '9') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z'))
                {
                    while (i < expression.Length && ((c >= '0' && c <= '9') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z')))
                    {
                        res += c;
                        i++;

                        if (i < expression.Length)
                        {
                            c = expression[i];
                        }
                    }

                    i--;
                    res += " ";
                }
                else if (c == '(')
                {
                    s.Push(c);
                }
                else if (c == ')')
                {
                    char t = '\0';
                    while (s.Count > 0 && t != '(')
                    {
                        t = s.Pop();

                        if (t != '(')
                        {
                            res += t + " ";
                        }
                    }
                }
                else if (this.operatorNodeFactory.Operators.ContainsKey(c))
                {
                    if (s.Count == 0 || s.Peek() == '(')
                    {
                        s.Push(c);
                    }
                    else
                    {
                        ExpressionTreeOperatorNode incoming = this.operatorNodeFactory.CreateOperatorNode(c);
                        ExpressionTreeOperatorNode top = this.operatorNodeFactory.CreateOperatorNode(s.Peek());

                        if (incoming.Precedence < top.Precedence ||
                            (incoming.Precedence == top.Precedence && incoming.Associativity == Associativity.Right))
                        {
                            s.Push(c);
                        }
                        else if (incoming.Precedence > top.Precedence ||
                            (incoming.Precedence == top.Precedence && incoming.Associativity == Associativity.Left))
                        {
                            while (s.Count > 0 && (incoming.Precedence > top.Precedence ||
                            (incoming.Precedence == top.Precedence && incoming.Associativity == Associativity.Left)))
                            {
                                res += s.Pop() + " ";

                                if (s.Count > 0 && this.operatorNodeFactory.Operators.ContainsKey(s.Peek()))
                                {
                                    top = this.operatorNodeFactory.CreateOperatorNode(s.Peek());
                                }
                            }

                            s.Push(c);
                        }
                    }
                }

                i++;
            }

            while (s.Count > 0)
            {
                res += s.Pop();

                if (s.Count > 0)
                {
                    res += " ";
                }
            }

            return res;
        }

        /// <summary>
        /// Creates a ExpressionTreeNode from an expression in postfix form.
        /// </summary>
        /// <param name="expression"> The string representation of the expression in postfix form. </param>
        /// <returns> ExpressionTreeNode representing the expression. </returns>
        private ExpressionTreeNode CreateTree(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return null;
            }

            string[] tokenized = expression.Split(' ');
            Stack<ExpressionTreeNode> stk = new Stack<ExpressionTreeNode>();
            foreach (string s in tokenized)
            {
                if (s == string.Empty)
                {
                    continue;
                }

                // Operator
                if (this.operatorNodeFactory.Operators.ContainsKey(s.ToCharArray()[0]))
                {
                    ExpressionTreeNode right = stk.Pop();
                    ExpressionTreeNode left = stk.Pop();
                    char operand = s.ToCharArray()[0];
                    ExpressionTreeOperatorNode expressionTreeOperatorNode = this.operatorNodeFactory.CreateOperatorNode(operand);
                    expressionTreeOperatorNode.Left = left;
                    expressionTreeOperatorNode.Right = right;
                    stk.Push(expressionTreeOperatorNode);
                }
                else if (this.IsValidVariableName(s))
                {
                    stk.Push(new ExpressionTreeVariableNode(s, ref this.variables));
                }
                else
                {
                    stk.Push(new ExpressionTreeConstantNode(double.Parse(s)));
                }
            }

            return stk.Pop();
        }

        /// <summary>
        /// Determines if expression is a valid variable name.
        /// Variables will start with an alphabet character, upper or lower-case, and be followed by any
        /// number of alphabet characters and numerical digits(0-9).
        /// </summary>
        /// <param name="expression"> String. </param>
        /// <returns> Whether expression is a valid variable name. </returns>
        private bool IsValidVariableName(string expression)
        {
            if (expression.Length == 0)
            {
                return false;
            }

            // If the first character is not an alphabetical character, it is invalid.
            if (!((expression[0] >= 'A' && expression[0] <= 'Z') ||
                    (expression[0] >= 'a' && expression[0] <= 'z')))
            {
                return false;
            }

            if (expression.Length == 1)
            {
                return true;
            }

            // See if remaining characters are alphabetical or numerical
            for (int i = 1; i < expression.Length; i++)
            {
                // If any are not alphabetical or numerical, return false
                if (!((expression[i] >= 'A' && expression[i] <= 'Z') ||
                   (expression[i] >= 'a' && expression[i] <= 'z') ||
                     (expression[i] >= '0' && expression[i] <= '9')))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds the first occurrence of either operand1 or operand2 not in parentheses, starting from the end
        /// of the string.
        /// </summary>
        /// <param name="expression"> String representing expression. </param>
        /// <param name="operand1"> Character representing an operand. </param>
        /// <param name="operand2"> Character representing a different operand. </param>
        /// <returns> Index of the first character in expression that matches either
        /// operand and is not inside parentheses. -1 if none were found. </returns>
        private int FindFirstOccurrenceNotInParentheses(string expression, char operand1, char operand2)
        {
            int openCount = 0;
            int closeCount = 0;
            for (int j = expression.Length - 1; j >= 0; j--)
            {
                if (expression[j] == '(')
                {
                    openCount++;
                }
                else if (expression[j] == ')')
                {
                    closeCount++;
                }
                else if (closeCount == openCount && (expression[j] == operand1 || expression[j] == operand2))
                {
                    return j;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns if the expression is enclosed in parentheses.
        /// </summary>
        /// <param name="expression"> String representation of expression. </param>
        /// <returns> Boolean representing whether expression is enclosed in parentheses. </returns>
        private bool IsEnclosedInParentheses(string expression)
        {
            if (expression == string.Empty)
            {
                return false;
            }

            int closePairIndex = -1;
            if (expression[0] == '(')
            {
                Stack<char> s = new Stack<char>();

                for (int j = 0; j < expression.Length; ++j)
                {
                    char c = expression[j];

                    if (c == '(')
                    {
                        s.Push(c);
                    }
                    else if (c == ')')
                    {
                        if (s.Count == 0)
                        {
                            throw new ArgumentException("Unbalanced parentheses.");
                        }

                        s.Pop();

                        if (s.Count == 0)
                        {
                            closePairIndex = j;
                            break;
                        }
                    }
                }

                if (closePairIndex == expression.Length - 1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Prints an in order traversal of the tree.
        /// </summary>
        /// <param name="n"> Node in the tree.</param>
        private void InOrderTraversal(ExpressionTreeNode n)
        {
            if (n is ExpressionTreeConstantNode)
            {
                Debug.WriteLine((n as ExpressionTreeConstantNode).Value);
            }
            else if (n is ExpressionTreeVariableNode)
            {
                ExpressionTreeVariableNode temp = n as ExpressionTreeVariableNode;
                Debug.WriteLine("(" + temp.VariableName + "," + this.variables[temp.VariableName] + ")");
            }
            else
            {
                ExpressionTreeOperatorNode temp = n as ExpressionTreeOperatorNode;
                this.InOrderTraversal(temp.Left);
                Debug.WriteLine(temp.Operator);
                this.InOrderTraversal(temp.Right);
            }
        }
    }
}
