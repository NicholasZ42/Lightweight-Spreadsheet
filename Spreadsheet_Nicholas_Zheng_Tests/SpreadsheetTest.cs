// <copyright file="SpreadsheetTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Spreadsheet_Nicholas_Zheng_Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using NUnit.Framework;
    using SpreadsheetEngine;

    /// <summary>
    /// Tests the Spreadsheet class.
    /// </summary>
    [TestFixture]
    public class SpreadsheetTest
    {
        /// <summary>
        /// Tests getting an invalid cell.
        /// </summary>
        /// <param name="cellName">String representation of a cell name. Ex: "A1". </param>
        [Test]
        [TestCase("A1")]
        [TestCase("J10")]
        [TestCase("B1")]
        public void GetCellNameParameterNonNullTest(string cellName)
        {
            Spreadsheet spreadsheet = new (10, 10);

            Assert.NotNull(spreadsheet.GetCell(cellName));
        }

        /// <summary>
        /// Tests getting an invalid cell.
        /// </summary>
        /// <param name="cellName">String representation of a cell name. Ex: "A1". </param>
        [Test]
        [TestCase("a1")]
        [TestCase("z10")]
        [TestCase("aa1")]
        public void GetCellNameParameterNullTest(string cellName)
        {
            Spreadsheet spreadsheet = new (10, 10);

            Assert.Null(spreadsheet.GetCell(cellName));
        }


        /// <summary>
        /// Tests getting valid cells.
        /// </summary>
        /// <param name="row">Row in spreadsheet.</param>
        /// <param name="col">Column in spreadsheet. </param>
        [Test]
        [TestCase(9, 9)]
        [TestCase(0, 0)]
        [TestCase(5, 5)]
        public void GetCellIndexParameterNonNullTest(int row, int col)
        {
            Spreadsheet spreadsheet = new (10, 10);

            Assert.NotNull(spreadsheet.GetCell(row, col));
        }

        /// <summary>
        /// Tests getting invalid cells.
        /// </summary>
        /// <param name="row">Row in spreadsheet.</param>
        /// <param name="col">Column in spreadsheet. </param>
        [Test]
        [TestCase(-1, 2)]
        [TestCase(11, 2)]
        [TestCase(1, 12)]
        [TestCase(1, -1)]
        [TestCase(1, 10)]
        public void GetCellIndexParameterNullTest(int row, int col)
        {
            Spreadsheet spreadsheet = new (10, 10);

            Assert.Null(spreadsheet.GetCell(row, col));
        }

        /// <summary>
        /// Test ComputeFormula indirectly by having cells reference each other.
        /// Test case 1: C1 references multiple cells.
        /// Test case 2: C1 references multiple cells with bad or self references.
        /// </summary>
        /// <param name="cellTextPairs">In the format (cell, text) for every 2 elements. </param>
        [Test]
        [Timeout(1000)]
        [TestCase(["D1", "=10", "D2", "=10", "C1", "=D1+D2", "20"])]
        [TestCase(["D1", "=D1", "D2", "=DD1", "C1", "=D1+D2", null])]
        public void ComputeFormulaTest(params string[] cellTextPairs)
        {
            Spreadsheet spreadsheet = new (50, 26);

            for (int i = 0; i < cellTextPairs.Length - 1; i += 2)
            {
                string cell = cellTextPairs[i];
                string text = cellTextPairs[i + 1];
                spreadsheet.GetCell(cell).Text = text;
            }

            string c = cellTextPairs[cellTextPairs.Length - 3];
            string expected = cellTextPairs[cellTextPairs.Length - 1];
            Assert.AreEqual(expected, spreadsheet.GetCell(c).Value);
        }

        /// <summary>
        /// Test circular references.
        /// Test case 1: Circular reference between A1 and B2 and C1 references one of them.
        /// Test case 2: Circular reference involving 4 cells.
        /// </summary>
        /// <param name="cellTextPairs">In the format (cell, text) for every 2 elements. </param>
        [Test]
        [Timeout(1000)]
        [TestCase(["A1", "=B1", "B1", "=A1", "C1", "=A1"])]
        [TestCase(["A1", "=B1", "B1", "=A2", "A2", "=B2", "B2", "=B1"])]
        public void CircularReferenceTest(params string[] cellTextPairs)
        {
            Spreadsheet spreadsheet = new (50, 26);
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
        /// Test for cases that should not cause circular references.
        /// </summary>
        /// <param name="cellTextPairs">In the format (cell, text) for every 2 elements. </param>
        [Test]
        [Timeout(1000)]
        [TestCase(["A1", "=B1", "C1", "=A1"])]
        [TestCase(["A1", "=B1", "B1", "=A2", "A2", "=B2"])]
        public void CircularReferenceNonPositiveTest(params string[] cellTextPairs)
        {
            Spreadsheet spreadsheet = new (50, 26);
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
        /// <param name="cellName">cell name.</param>
        /// <param name="value">value.</param>
        [Test]
        [Timeout(1000)]
        [TestCase("A1", "=B51")]
        [TestCase("A1", "=B0")]
        [TestCase("A1", "=B-51")]
        public void BadReferenceTest(string cellName, string value)
        {
            Spreadsheet spreadsheet = new (50, 26);
            spreadsheet.GetCell(cellName).Text = value;
            Assert.AreEqual(spreadsheet.GetCell(cellName).Value, "!(bad reference)");
        }

        /// <summary>
        /// Test for cases that should NOT cause a bad reference.
        /// </summary>
        /// <param name="cellName">cell name.</param>
        /// <param name="value">value.</param>
        [Test]
        [Timeout(1000)]
        [TestCase("A1", "=B50")]
        [TestCase("A1", "=B1")]
        [TestCase("A1", "=Z50")]
        [TestCase("A1", "=Z1")]
        [TestCase("A1", "=E25")]
        public void BadReferenceNonPositiveTest(string cellName, string value)
        {
            Spreadsheet spreadsheet = new (50, 26);
            spreadsheet.GetCell(cellName).Text = value;
            Assert.AreNotEqual(spreadsheet.GetCell(cellName).Value, "!(bad reference)");
        }

        /// <summary>
        /// Test for cases that should cause a self reference.
        /// </summary>
        /// <param name="cellName">cell name.</param>
        /// <param name="value">value.</param>
        [Test]
        [Timeout(1000)]
        [TestCase("A1", "=A1")]
        [TestCase("A50", "=A50")]
        [TestCase("Z50", "=Z50")]
        [TestCase("Z1", "=Z1")]
        [TestCase("E25", "=E25")]
        public void SelfReferenceTest(string cellName, string value)
        {
            Spreadsheet spreadsheet = new (50, 26);
            spreadsheet.GetCell(cellName).Text = value;
            Assert.AreEqual(spreadsheet.GetCell(cellName).Value, "!(self reference)");
        }

        /// <summary>
        /// Test for cases that should NOT cause a self reference.
        /// </summary>
        /// <param name="cellName">cell name.</param>
        /// <param name="value">value.</param>
        [Test]
        [Timeout(1000)]
        [TestCase("A1", "=A2")]
        [TestCase("A50", "=A49")]
        [TestCase("Z50", "=Z51")]
        [TestCase("Z1", "=Z11")]
        [TestCase("E25", "=E2")]
        public void SelfReferenceNonPositiveTest(string cellName, string value)
        {
            Spreadsheet spreadsheet = new (50, 26);
            spreadsheet.GetCell(cellName).Text = value;
            Assert.AreNotEqual(spreadsheet.GetCell(cellName).Value, "!(self reference)");
        }

        /// <summary>
        /// Tests saving a spreadsheet with valid data.
        /// </summary>
        [Test]
        public void SaveSpreadsheetTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new (50, 26);

            // Set up some test data
            spreadsheet.GetCell("A1").Text = "Hello";
            spreadsheet.GetCell("A1").BGColor = 123456;
            spreadsheet.GetCell("B1").Text = "World";
            spreadsheet.GetCell("B1").BGColor = 654321;

            using (FileStream fileStream = new (this.SPREADSHEET_TEST_FILEPATH, FileMode.Create))
            {
                // Act
                spreadsheet.Save(fileStream);
            }

            using (FileStream fileStream = new (this.SPREADSHEET_TEST_FILEPATH, FileMode.Open))
            {
                XmlDocument doc = new ();
                doc.Load(fileStream);

                XmlElement root = doc.DocumentElement;
                Assert.IsNotNull(root);
                Assert.AreEqual("root", root.Name);

                XmlNodeList cells = root.SelectNodes("cell");
                Assert.AreEqual(2, cells.Count);

                // Validate first cell (A1)
                XmlNode cellA1 = cells[0];
                Assert.AreEqual("cell", cellA1.Name);
                Assert.AreEqual("0", cellA1.SelectSingleNode("rowIndex").InnerText); // Row index for A1
                Assert.AreEqual("0", cellA1.SelectSingleNode("colIndex").InnerText); // Column index for A1
                Assert.AreEqual("123456", cellA1.SelectSingleNode("bgColor").InnerText);
                Assert.AreEqual("Hello", cellA1.SelectSingleNode("text").InnerText);

                // Validate second cell (B1)
                XmlNode cellB1 = cells[1];
                Assert.AreEqual("cell", cellB1.Name);
                Assert.AreEqual("0", cellB1.SelectSingleNode("rowIndex").InnerText); // Row index for B1
                Assert.AreEqual("1", cellB1.SelectSingleNode("colIndex").InnerText); // Column index for B1
                Assert.AreEqual("654321", cellB1.SelectSingleNode("bgColor").InnerText);
                Assert.AreEqual("World", cellB1.SelectSingleNode("text").InnerText);
            }
        }

        private readonly string SPREADSHEET_TEST_FILEPATH = "../spreadsheetTest.xml";

        /// <summary>
        /// Loading a spreadsheet with valid data.
        /// </summary>
        [Test]
        public void LoadSpreadsheetTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new (50, 26);

            // Set up some test data
            spreadsheet.GetCell("A1").Text = "Hello";
            spreadsheet.GetCell("A1").BGColor = 123456;
            spreadsheet.GetCell("B1").Text = "World";
            spreadsheet.GetCell("B1").BGColor = 654321;

            string c1Text = spreadsheet.GetCell("C1").Text;
            uint c1BG = spreadsheet.GetCell("C1").BGColor;

            // Save spreadsheet
            using (FileStream fileStream = new (this.SPREADSHEET_TEST_FILEPATH, FileMode.Create))
            {
                // Act
                spreadsheet.Save(fileStream);
            }

            // Change spreadsheet
            spreadsheet.GetCell("A1").Text = "HelloT";
            spreadsheet.GetCell("A1").BGColor = 12356;
            spreadsheet.GetCell("B1").Text = "Worldd";
            spreadsheet.GetCell("B1").BGColor = 65421;
            spreadsheet.GetCell("C1").Text = "Worldd";
            spreadsheet.GetCell("C1").BGColor = 65421;

            spreadsheet.Load(this.SPREADSHEET_TEST_FILEPATH);

            // Assert that spreadsheet is the same as original
            Assert.AreEqual("Hello", spreadsheet.GetCell("A1").Text);
            Assert.AreEqual(123456, spreadsheet.GetCell("A1").BGColor);
            Assert.AreEqual("World", spreadsheet.GetCell("B1").Text);
            Assert.AreEqual(654321, spreadsheet.GetCell("B1").BGColor);
            Assert.AreEqual(c1Text, spreadsheet.GetCell("C1").Text);
            Assert.AreEqual(c1BG, spreadsheet.GetCell("C1").BGColor);
        }

        /// <summary>
        /// Test multipe undo operations done one after the other.
        /// </summary>
        [Test]
        public void UndoCellTextMultipleTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new (50, 26);

            string emptyCellText = spreadsheet.GetCell("A1").Text;
            uint emptyCellBG = spreadsheet.GetCell("A1").BGColor;

            ChangeText(ref spreadsheet, "A1", "Hello", emptyCellText);
            ChangeText(ref spreadsheet, "B1", "Hello", emptyCellText);

            spreadsheet.Undo();
            spreadsheet.Undo();

            Assert.AreEqual(emptyCellText, spreadsheet.GetCell("A1").Text);
            Assert.AreEqual(emptyCellBG, spreadsheet.GetCell("A1").BGColor);

            // Assert no other cells in spreadsheet were changed
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    string cellName = string.Empty + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(emptyCellText, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(emptyCellBG, spreadsheet.GetCell(cellName).BGColor);
                }
            }
        }

        /// <summary>
        /// Test single undo operation.
        /// </summary>
        [Test]
        public void UndoCellTextSingleTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new (50, 26);

            string a1Text = spreadsheet.GetCell("A1").Text;
            uint a1BG = spreadsheet.GetCell("A1").BGColor;

            ChangeText(ref spreadsheet, "A1", "Hello", a1Text);

            spreadsheet.Undo();

            Assert.AreEqual(a1Text, spreadsheet.GetCell("A1").Text);
            Assert.AreEqual(a1BG, spreadsheet.GetCell("A1").BGColor);

            // Assert no other cells in spreadsheet were changed
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    string cellName = string.Empty + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(a1Text, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(a1BG, spreadsheet.GetCell(cellName).BGColor);
                }
            }
        }

        /// <summary>
        /// Test undo operation after spreadsheet load.
        /// </summary>
        [Test]
        public void UndoCellTextAfterLoadSpreadsheetTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new (50, 26);

            // Set up some test data
            spreadsheet.GetCell("A1").Text = "Hello";
            spreadsheet.GetCell("A1").BGColor = 123456;
            spreadsheet.GetCell("B1").Text = "World";
            spreadsheet.GetCell("B1").BGColor = 654321;

            // Save spreadsheet
            using (FileStream fileStream = new (this.SPREADSHEET_TEST_FILEPATH, FileMode.Create))
            {
                // Act
                spreadsheet.Save(fileStream);
            }

            spreadsheet.Load(this.SPREADSHEET_TEST_FILEPATH);
            spreadsheet.Undo();

            Assert.AreEqual("Hello", spreadsheet.GetCell("A1").Text);
            Assert.AreEqual(123456, spreadsheet.GetCell("A1").BGColor);
            Assert.AreEqual("World", spreadsheet.GetCell("B1").Text);
            Assert.AreEqual(654321, spreadsheet.GetCell("B1").BGColor);
            Assert.AreEqual(true, spreadsheet.UndosEmpty());
            Assert.AreEqual(true, spreadsheet.RedosEmpty());
        }

        /// <summary>
        /// Test single undo operation.
        /// </summary>
        [Test]
        public void RedoCellTextSingleTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new (50, 26);

            string a1Text = spreadsheet.GetCell("A1").Text;
            uint a1BG = spreadsheet.GetCell("A1").BGColor;

            ChangeText(ref spreadsheet, "A1", "Hello", a1Text);

            spreadsheet.Undo();
            spreadsheet.Redo();

            Assert.AreEqual("Hello", spreadsheet.GetCell("A1").Text);

            // Assert no other cells in spreadsheet were changed
            for (int i = 1; i < 26; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    string cellName = string.Empty + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(a1Text, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(a1BG, spreadsheet.GetCell(cellName).BGColor);
                }
            }

            // Check all cells in first row except A1
            for (int i = 0; i < 1; i++)
            {
                for (int j = 1; j < 50; j++)
                {
                    string cellName = string.Empty + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(a1Text, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(a1BG, spreadsheet.GetCell(cellName).BGColor);
                }
            }
        }

        /// <summary>
        /// Change the text of a spreadsheet and update the undo stacks.
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to change. </param>
        private static void ChangeText(ref Spreadsheet spreadsheet, string cellName, string newText, string emptyCellText)
        {
            spreadsheet.GetCell(cellName).Text = newText;
            CellTextChangeCommand cellTextChangeCommand = new (
                spreadsheet.GetCell(cellName), emptyCellText);
            List<ICommand> commands = [cellTextChangeCommand];
            spreadsheet.AddUndo(commands);
        }

        /// <summary>
        /// Test multipe undo operations done one after the other.
        /// </summary>
        [Test]
        public void UndoBackgroundMultipleTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new (50, 26);

            string emptyCellText = spreadsheet.GetCell("A1").Text;
            uint emptyCellBG = spreadsheet.GetCell("A1").BGColor;
            Assert.AreNotEqual(emptyCellBG, 123456);

            ChangeBG(ref spreadsheet, "A1", 123456, emptyCellBG);
            ChangeBG(ref spreadsheet, "B1", 123456, emptyCellBG);

            spreadsheet.Undo();
            spreadsheet.Undo();

            // Assert all cells in spreadsheet were reverted to original
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    string cellName = string.Empty + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(emptyCellText, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(emptyCellBG, spreadsheet.GetCell(cellName).BGColor);
                }
            }
        }

        /// <summary>
        /// Test single undo operation.
        /// </summary>
        [Test]
        public void UndoBackgroundSingleTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new (50, 26);

            string a1Text = spreadsheet.GetCell("A1").Text;
            uint a1BG = spreadsheet.GetCell("A1").BGColor;

            ChangeBG(ref spreadsheet, "A1", 123456, a1BG);

            spreadsheet.Undo();

            Assert.AreEqual(a1Text, spreadsheet.GetCell("A1").Text);
            Assert.AreEqual(a1BG, spreadsheet.GetCell("A1").BGColor);

            // Assert no other cells in spreadsheet were changed
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    string cellName = string.Empty + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(a1Text, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(a1BG, spreadsheet.GetCell(cellName).BGColor);
                }
            }
        }

        /// <summary>
        /// Test single redo operation.
        /// </summary>
        [Test]
        public void RedoBackgroundSingleTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new (50, 26);

            string a1Text = spreadsheet.GetCell("A1").Text;
            uint a1BG = spreadsheet.GetCell("A1").BGColor;

            ChangeBG(ref spreadsheet, "A1", 123456, a1BG);

            spreadsheet.Undo();
            spreadsheet.Redo();

            Assert.AreEqual(123456, spreadsheet.GetCell("A1").BGColor);

            // Assert no other cells in spreadsheet were changed
            for (int i = 1; i < 26; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    string cellName = string.Empty + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(a1Text, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(a1BG, spreadsheet.GetCell(cellName).BGColor);
                }
            }

            // Check all cells in first row except A1
            for (int i = 0; i < 1; i++)
            {
                for (int j = 1; j < 50; j++)
                {
                    string cellName = string.Empty + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(a1Text, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(a1BG, spreadsheet.GetCell(cellName).BGColor);
                }
            }
        }

        /// <summary>
        /// Tests to cover the GetTopUndoCommandTitle method.
        /// </summary>
        /// <param name="expectedTitle">For when the method is called.</param>
        /// <param name="commandTitles"> Which type of commands to add. </param>
        [TestCase(null, new string[] { })]
        [TestCase("Background change", new string[] { "Background change" })]
        public void GetTopUndoCommandTitleTest(string expectedTitle, string[] commandTitles)
        {
            Spreadsheet spreadsheet = new (50, 26);

            var commands = commandTitles.Select(title =>
                title == "Background change"
                    ? (ICommand)new BackgroundChangeCommand(null, 123456)
                    : new CellTextChangeCommand(null, "Test"))
                .ToList();

            if (commands.Count > 0)
            {
                spreadsheet.AddUndo(commands);
            }

            // Act
            var result = spreadsheet.GetTopUndoCommandTitle();

            // Assert
            Assert.AreEqual(expectedTitle, result);
        }

        /// <summary>
        /// Tests to cover the GetTopRedoCommandTitle method.
        /// </summary>
        /// <param name="expectedTitle"> For calling the method. </param>
        /// <param name="commandTitles"> Which commands to add. </param>
        [TestCase(null, new string[] { })]
        [TestCase("Background change", new string[] { "Background change" })]
        public void GetTopRedoCommandTitleTest(string expectedTitle, string[] commandTitles)
        {
            Spreadsheet spreadsheet = new(50, 26);

            var commands = commandTitles.Select(title =>
                title == "Background change"
                    ? (ICommand)new BackgroundChangeCommand(null, 123456)
                    : new CellTextChangeCommand(null, "Test"))
                .ToList();

            if (commands.Count > 0)
            {
                spreadsheet.AddUndo(commands);
                spreadsheet.Undo();
            }

            // Act
            var result = spreadsheet.GetTopRedoCommandTitle();

            // Assert
            Assert.AreEqual(expectedTitle, result);
        }

        /// <summary>
        /// Change the BGColor of a spreadsheet and update the undo stacks.
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to change. </param>
        private static void ChangeBG(ref Spreadsheet spreadsheet, string cellName, uint value, uint emptyCellBG)
        {
            spreadsheet.GetCell(cellName).BGColor = value;
            BackgroundChangeCommand backgroundChangeCommand = new (
                spreadsheet.GetCell(cellName), emptyCellBG);
            List<ICommand> commands = [backgroundChangeCommand];
            spreadsheet.AddUndo(commands);
        }
    }
}
