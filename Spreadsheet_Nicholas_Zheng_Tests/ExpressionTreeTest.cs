// <copyright file="ExpressionTreeTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Spreadsheet_Nicholas_Zheng_Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using SpreadsheetEngine;

    /// <summary>
    /// Tests the ExpressionTree class.
    /// </summary>
    [TestFixture]
    public class ExpressionTreeTest
    {
        /// <summary>
        /// Tests an expression tree with only the addition operator.
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluateAdditionTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("A+Hello+B+6");
            Assert.AreEqual(0.0, expressionTree.Evaluate());

            // Variable has not been set
        }

        /// <summary>
        /// Tests an expression tree with only the subtraction operator.
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluateSubtractionTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("A-Hello-B-6");
            Assert.AreEqual(0.0, expressionTree.Evaluate());

            // Variable has not been set
        }

        /// <summary>
        /// Tests an expression tree with only the multiplication operator.
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluateMultiplicationTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("4*6*2");
            Assert.AreEqual(48.0, expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests an expression tree with only the division operator.
        /// </summary>
        [Test]
        public void ExpressionTreeEvaluateDivisionTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("24/2/3/2");
            expressionTree.InOrderTraversal();
            Assert.AreEqual(2.0, expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests the SetVariable() function of ExpressionTree for a
        /// valid variable.
        /// </summary>
        [Test]
        public void ExpressionTreeSetVariableTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("A6/2");
            Assert.AreEqual(0.0, expressionTree.Evaluate());
            expressionTree.SetVariable("A6", 2);
            Assert.AreEqual(1.0, expressionTree.Evaluate());
            expressionTree.SetVariable("A6", 7);
            Assert.AreEqual(3.5, expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests the SetVariable() function of ExpressionTree for a
        /// valid variable.
        /// </summary>
        [Test]
        public void ExpressionTreeSetVariableMultiNegativeTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("Hello-12-World-yo");
            expressionTree.SetVariable("Hello", 42);
            expressionTree.SetVariable("World", 20);
            expressionTree.SetVariable("yo", 10);
            expressionTree.InOrderTraversal();
            Assert.AreEqual(0.0, expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests the SetVariable() function of ExpressionTree for a
        /// valid variable.
        /// </summary>
        [Test]
        public void ExpressionTreeSetVariableNegativeStartTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("12-World");
            expressionTree.SetVariable("World", -20);
            expressionTree.InOrderTraversal();
            Assert.AreEqual(32.0, expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests the SetVariable() function of ExpressionTree for an invalid
        /// variable.
        /// </summary>
        [Test]
        public void ExpressionTreeSetVariableInvalidTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("A6/2");
            Assert.AreEqual(0.0, expressionTree.Evaluate());
            expressionTree.SetVariable("A7", 2);
            Assert.AreEqual(0.0, expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests expression with valid parentheses.
        /// </summary>
        [Test]
        public void ExpressionTreeParenthesesLeftTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("((2+3)*(4+5)-4)");
            expressionTree.InOrderTraversal();
            Assert.AreEqual(41.0, expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests expression with valid parentheses.
        /// </summary>
        [Test]
        public void ExpressionTreeParenthesesOrderOfOperationsTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("(4+(2+3)*(4+5))");
            Assert.AreEqual(49.0, expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests expression with valid parentheses.
        /// </summary>
        [Test]
        public void ExpressionTreeParenthesesOrderOfOperations2Test()
        {
            ExpressionTree expressionTree = new ExpressionTree("(4+(2+3)*((4+5)+4))");
            Assert.AreEqual(69.0, expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests expression with valid parentheses.
        /// </summary>
        [Test]
        public void ExpressionTreeParenthesesTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("((2+3)*(4+5))");
            Assert.AreEqual(45.0, expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests expression with valid parentheses.
        /// </summary>
        [Test]
        public void ExpressionTreeParenthesesNotSurroundedTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("((2+3)*(4+5))+5");
            Assert.AreEqual(50.0, expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests expression with valid parentheses.
        /// </summary>
        [Test]
        public void ExpressionTreeParenthesesEndTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("10/(7-2)");
            Assert.AreEqual(10.0 / (7 - 2), expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests expression with valid parentheses.
        /// </summary>
        [Test]
        public void ExpressionTreeParenthesesEnd2Test()
        {
            ExpressionTree expressionTree = new ExpressionTree("10/(2*5)");
            Assert.AreEqual(1.0, expressionTree.Evaluate());
        }

        /// <summary>
        /// Tests expression with valid parentheses.
        /// </summary>
        [Test]
        public void ExpressionTreeSurroundedParenthesesTest()
        {
            ExpressionTree expressionTree = new ExpressionTree("(((((2+3)*(4+5)))))");
            Assert.AreEqual(45.0, expressionTree.Evaluate());
        }

        ///////////////////////////////// Testing private methods /////////////////////////

        /// <summary>
        /// Tests finding the first occurrence of two operators not in parentheses with a expression where there
        /// are none.
        /// </summary>
        [Test]
        public void FindFirstOccurrenceNotInParenthesesNoneTest()
        {
            string expression = "(((((2+3)*(4+5)))))";
            ExpressionTree expressionTree = new ExpressionTree(expression);
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "FindFirstOccurrenceNotInParentheses",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(-1, methodInfo.Invoke(expressionTree, new object[] { expression, '*', '/' }));
        }

        /// <summary>
        /// Tests finding the first occurrence of two operators not in parentheses with a expression where there
        /// are two.
        /// </summary>
        [Test]
        public void FindFirstOccurrenceNotInParenthesesTwoTest()
        {
            string expression = "(2+3)*(4+5)/3";
            ExpressionTree expressionTree = new ExpressionTree(expression);
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "FindFirstOccurrenceNotInParentheses",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(11, methodInfo.Invoke(expressionTree, new object[] { expression, '*', '/' }));
        }

        /// <summary>
        /// Tests finding the first occurrence of two operators not in parentheses with a expression where there
        /// are no parentheses.
        /// </summary>
        [Test]
        public void FindFirstOccurrenceNotInParenthesesNoParenthesesTest()
        {
            string expression = "2+3+4";
            ExpressionTree expressionTree = new ExpressionTree(expression);
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "FindFirstOccurrenceNotInParentheses",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(3, methodInfo.Invoke(expressionTree, new object[] { expression, '+', '-' }));
        }

        /// <summary>
        /// Tests seeing if an expression is enclosed in parentheses for an expression with no parentheses.
        /// </summary>
        [Test]
        public void IsEnclosedInParenthesesNoneTest()
        {
            string expression = "2+3+4";
            ExpressionTree expressionTree = new ExpressionTree(expression);
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "IsEnclosedInParentheses",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(false, methodInfo.Invoke(expressionTree, new object[] { expression }));
        }

        /// <summary>
        /// Tests seeing if an expression is enclosed in parentheses for an expression with
        /// one level of parentheses surrounding it.
        /// </summary>
        [Test]
        public void IsEnclosedInParenthesesOneTest()
        {
            string expression = "(2+3+4)";
            ExpressionTree expressionTree = new ExpressionTree(expression);
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "IsEnclosedInParentheses",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(true, methodInfo.Invoke(expressionTree, new object[] { expression }));
        }

        /// <summary>
        /// Tests seeing if an expression is enclosed in parentheses for an expression with
        /// two levels of parentheses surrounding it.
        /// </summary>
        [Test]
        public void IsEnclosedInParenthesesTwoTest()
        {
            string expression = "((2+3+4))";
            ExpressionTree expressionTree = new ExpressionTree(expression);
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "IsEnclosedInParentheses",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(true, methodInfo.Invoke(expressionTree, new object[] { expression }));
        }

        /// <summary>
        /// Tested using a variable with an invalid first character.
        /// </summary>
        [Test]
        public void IsValidVariableNameInvalid1Test()
        {
            string expression = "((2+3+4))";
            ExpressionTree expressionTree = new ExpressionTree(expression);
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "IsValidVariableName",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(false, methodInfo.Invoke(expressionTree, new object[] { "9A2" }));
        }

        /// <summary>
        /// Tested using an invalid variable with a non alphabetic character but valid first character.
        /// </summary>
        [Test]
        public void IsValidVariableNameInvalid2Test()
        {
            string expression = "((2+3+4))";
            ExpressionTree expressionTree = new ExpressionTree(expression);
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "IsValidVariableName",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(false, methodInfo.Invoke(expressionTree, new object[] { "A2/" }));
        }

        /// <summary>
        /// Tested using an invalid variable with length 0.
        /// </summary>
        [Test]
        public void IsValidVariableNameInvalid3Test()
        {
            string expression = "((2+3+4))";
            ExpressionTree expressionTree = new ExpressionTree(expression);
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "IsValidVariableName",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(false, methodInfo.Invoke(expressionTree, new object[] { string.Empty }));
        }

        /// <summary>
        /// Tested using a valid variable.
        /// </summary>
        [Test]
        public void IsValidVariableNameValid1Test()
        {
            string expression = "((2+3+4))";
            ExpressionTree expressionTree = new ExpressionTree(expression);
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "IsValidVariableName",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(true, methodInfo.Invoke(expressionTree, new object[] { "A2" }));
        }

        /// <summary>
        /// Tested using a valid variable.
        /// </summary>
        [Test]
        public void IsValidVariableNameValid2Test()
        {
            string expression = "((2+3+4))";
            ExpressionTree expressionTree = new ExpressionTree(expression);
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "IsValidVariableName",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(true, methodInfo.Invoke(expressionTree, new object[] { "A" }));
        }

        /// <summary>
        /// Tests parsing an infix expression to postfix.
        /// </summary>
        [Test]
        public void ParsePrecedenceTest1()
        {
            ExpressionTree expression = new ExpressionTree("A*B+C");
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "Parse",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("A B * C +", methodInfo.Invoke(expression, new object[] { "A*B+C" }));
        }

        /// <summary>
        /// Tests parsing an infix expression to postfix.
        /// </summary>
        [Test]
        public void ParsePrecedenceTest2()
        {
            ExpressionTree expression = new ExpressionTree("A+B*C");
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "Parse",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("A B C * +", methodInfo.Invoke(expression, new object[] { "A+B*C" }));
        }

        /// <summary>
        /// Tests parsing an infix expression to postfix.
        /// </summary>
        [Test]
        public void ParseParenthesesTest()
        {
            ExpressionTree expression = new ExpressionTree("A*(B+C)");
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "Parse",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("A B C + *", methodInfo.Invoke(expression, new object[] { "A*(B+C)" }));
        }

        /// <summary>
        /// Tests parsing an infix expression to postfix.
        /// </summary>
        [Test]
        public void ParseSamePrecedenceTest()
        {
            ExpressionTree expression = new ExpressionTree("A-B+C");
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "Parse",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("A B - C +", methodInfo.Invoke(expression, new object[] { "A-B+C" }));
        }

        /// <summary>
        /// Tests parsing an infix expression to postfix.
        /// </summary>
        [Test]
        public void ParseParenthesesAndPrecedenceTest()
        {
            ExpressionTree expression = new ExpressionTree("A*(B+C*D)+E");
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "Parse",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("A B C D * + * E +", methodInfo.Invoke(expression, new object[] { "A*(B+C*D)+E" }));
        }

        /// <summary>
        /// Tests parsing an empty infix expression to postfix.
        /// </summary>
        [Test]
        public void ParseEmptyTest()
        {
            ExpressionTree expression = new ExpressionTree(string.Empty);
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "Parse",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(string.Empty, methodInfo.Invoke(expression, new object[] { string.Empty }));
        }

        /// <summary>
        /// Tests parsing an infix expression to postfix.
        /// </summary>
        [Test]
        public void ParseLongVariableLengthTest()
        {
            ExpressionTree expression = new ExpressionTree("A21*B12+C9");
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "Parse",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("A21 B12 * C9 +", methodInfo.Invoke(expression, new object[] { "A21*B12+C9" }));
        }

        /// <summary>
        /// Tests parsing an infix expression to postfix.
        /// </summary>
        [Test]
        public void RemoveOuterParenthesesNoneTest()
        {
            ExpressionTree expression = new ExpressionTree("A21*B12+C9");
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "RemoveOuterParentheses",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("A21*B12+C9", methodInfo.Invoke(expression, new object[] { "A21*B12+C9" }));
        }

        /// <summary>
        /// Tests parsing an infix expression to postfix.
        /// </summary>
        [Test]
        public void RemoveOuterParenthesesMultipleTest()
        {
            ExpressionTree expression = new ExpressionTree("((A21*B12+C9))");
            MethodInfo methodInfo = typeof(ExpressionTree).GetMethod(
                "RemoveOuterParentheses",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual("A21*B12+C9", methodInfo.Invoke(expression, new object[] { "((A21*B12+C9))" }));
        }
    }
}
