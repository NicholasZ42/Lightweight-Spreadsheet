// <copyright file="SpreadsheetTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Spreadsheet_Nicholas_Zheng_Tests
{
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;
    using SpreadsheetEngine;

    /// <summary>
    /// Tests the Spreadsheet class.
    /// </summary>
    [TestFixture]
    public class SpreadsheetTest
    {
        /// <summary>
        /// Tests getting a valid cell in the spreadsheet.
        /// </summary>
        [Test]
        public void GetCellNameParameterNonNullTest()
        {
            Spreadsheet spreadsheet = new Spreadsheet(10, 10);

            Assert.NotNull(spreadsheet.GetCell("A1"));
        }

        /// <summary>
        /// Tests getting an invalid cell in the spreadsheet.
        /// </summary>
        [Test]
        public void GetCellNameParameterNullTest()
        {
            Spreadsheet spreadsheet = new Spreadsheet(10, 10);

            Assert.Null(spreadsheet.GetCell("a1"));
        }

        /// <summary>
        /// Tests getting a valid cell in the spreadsheet.
        /// </summary>
        [Test]
        public void GetCellIndexParameterNonNullTest()
        {
            Spreadsheet spreadsheet = new Spreadsheet(10, 10);

            Assert.NotNull(spreadsheet.GetCell(0, 2));
        }

        /// <summary>
        /// Tests getting an invalid cell in the spreadsheet.
        /// </summary>
        [Test]
        public void GetCellIndexParameterNullTest()
        {
            Spreadsheet spreadsheet = new Spreadsheet(10, 10);

            Assert.Null(spreadsheet.GetCell(-1, 2));
        }

        /// <summary>
        /// Test circular references.
        /// Test case 1: Circular reference between A1 and B2 and C1 references one of them.
        /// Test case 2: Circular reference involving 4 cells.
        /// </summary>
        /// <param name="cellTextPairs">In the format (cell, text) for every 2 elements. </param>
        [Test]
        [Timeout(1000)]
        [TestCase(new object[] { "A1", "=B1", "B1", "=A1", "C1", "=A1" })]
        [TestCase(new object[] { "A1", "=B1", "B1", "=A2", "A2", "=B2", "B2", "=B1" })]
        public void CircularReferenceTest(params string[] cellTextPairs)
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            for (int i = 0; i < cellTextPairs.Length; i += 2)
            {
                string cell = cellTextPairs[i];
                string text = cellTextPairs[i + 1];
                spreadsheet.GetCell(cell).Text = text;
            }

            for (int i = 0; i < cellTextPairs.Length; i += 2)
            {
                string cell = cellTextPairs[i];
                Assert.AreEqual(spreadsheet.GetCell(cell).Value, "!(circular reference)");
            }
        }

        /// <summary>
        /// Test for cases that should not cause circular references
        /// </summary>
        /// <param name="cellTextPairs">In the format (cell, text) for every 2 elements. </param>
        [Test]
        [Timeout(1000)]
        [TestCase(new object[] { "A1", "=B1", "C1", "=A1" })]
        [TestCase(new object[] { "A1", "=B1", "B1", "=A2", "A2", "=B2" })]
        public void CircularReferenceNonPositiveTest(params string[] cellTextPairs)
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            for (int i = 0; i < cellTextPairs.Length; i += 2)
            {
                string cell = cellTextPairs[i];
                string text = cellTextPairs[i + 1];
                spreadsheet.GetCell(cell).Text = text;
            }

            for (int i = 0; i < cellTextPairs.Length; i += 2)
            {
                string cell = cellTextPairs[i];
                Assert.AreNotEqual(spreadsheet.GetCell(cell).Value, "!(circular reference)");
            }
        }

        /// <summary>
        /// Test for cases that should cause a bad reference.
        /// </summary>
        /// <param name="cellName">cell name</param>
        /// <param name="value">value</param>
        [Test]
        [Timeout(1000)]
        [TestCase("A1", "=B51")]
        [TestCase("A1", "=B0")]
        [TestCase("A1", "=B-51")]
        public void BadReferenceTest(string cellName, string value)
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            spreadsheet.GetCell(cellName).Text = value;
            Assert.AreEqual(spreadsheet.GetCell(cellName).Value, "!(bad reference)");
        }

        /// <summary>
        /// Test for cases that should NOT cause a bad reference.
        /// </summary>
        /// <param name="cellName">cell name</param>
        /// <param name="value">value</param>
        [Test]
        [Timeout(1000)]
        [TestCase("A1", "=B50")]
        [TestCase("A1", "=B1")]
        [TestCase("A1", "=Z50")]
        [TestCase("A1", "=Z1")]
        [TestCase("A1", "=E25")]
        public void BadReferenceNonPositiveTest(string cellName, string value)
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            spreadsheet.GetCell(cellName).Text = value;
            Assert.AreNotEqual(spreadsheet.GetCell(cellName).Value, "!(bad reference)");
        }

        /// <summary>
        /// Test for cases that should cause a self reference.
        /// </summary>
        /// <param name="cellName">cell name</param>
        /// <param name="value">value</param>
        [Test]
        [Timeout(1000)]
        [TestCase("A1", "=A1")]
        [TestCase("A50", "=A50")]
        [TestCase("Z50", "=Z50")]
        [TestCase("Z1", "=Z1")]
        [TestCase("E25", "=E25")]
        public void SelfReferenceTest(string cellName, string value)
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            spreadsheet.GetCell(cellName).Text = value;
            Assert.AreEqual(spreadsheet.GetCell(cellName).Value, "!(self reference)");
        }

        /// <summary>
        /// Test for cases that should NOT cause a self reference.
        /// </summary>
        /// <param name="cellName">cell name</param>
        /// <param name="value">value</param>
        [Test]
        [Timeout(1000)]
        [TestCase("A1", "=A2")]
        [TestCase("A50", "=A49")]
        [TestCase("Z50", "=Z51")]
        [TestCase("Z1", "=Z11")]
        [TestCase("E25", "=E2")]
        public void SelfReferenceNonPositiveTest(string cellName, string value)
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            spreadsheet.GetCell(cellName).Text = value;
            Assert.AreNotEqual(spreadsheet.GetCell(cellName).Value, "!(self reference)");
        }
    }
}
