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
    }
}
