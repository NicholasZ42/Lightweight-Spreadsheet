// <copyright file="SpreadsheetTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Spreadsheet_Nicholas_Zheng_Tests
{
    using NUnit.Framework;
    using SpreadsheetEngine;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

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

        /// <summary>
        /// Tests saving a spreadsheet with valid data.
        /// </summary>
        [Test]
        public void SaveSpreadsheetTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);

            // Set up some test data
            spreadsheet.GetCell("A1").Text = "Hello";
            spreadsheet.GetCell("A1").BGColor = 123456;
            spreadsheet.GetCell("B1").Text = "World";
            spreadsheet.GetCell("B1").BGColor = 654321;

            using (FileStream fileStream = new FileStream("./Spreadsheet_Nicholas_Zheng/bin/Debug/spreadsheetTest.xml", FileMode.Create))
            {
                // Act
                spreadsheet.Save(fileStream);
            }

            using (FileStream fileStream = new FileStream("./Spreadsheet_Nicholas_Zheng/bin/Debug/spreadsheetTest.xml", FileMode.Open))
            {
                XmlDocument doc = new XmlDocument();
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

        private string SPREADSHEET_TEST_FILEPATH = "./Spreadsheet_Nicholas_Zheng/bin/Debug/spreadsheetTest.xml";

        /// <summary>
        /// Loading a spreadsheet with valid data.
        /// </summary>
        [Test]
        public void LoadSpreadsheetTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);

            // Set up some test data
            spreadsheet.GetCell("A1").Text = "Hello";
            spreadsheet.GetCell("A1").BGColor = 123456;
            spreadsheet.GetCell("B1").Text = "World";
            spreadsheet.GetCell("B1").BGColor = 654321;

            string c1Text = spreadsheet.GetCell("C1").Text;
            uint c1BG = spreadsheet.GetCell("C1").BGColor;

            // Save spreadsheet
            using (FileStream fileStream = new FileStream(this.SPREADSHEET_TEST_FILEPATH, FileMode.Create))
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
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);

            string emptyCellText = spreadsheet.GetCell("A1").Text;
            uint emptyCellBG = spreadsheet.GetCell("A1").BGColor;

            CellTextChangeCommand cellTextChangeCommand = new CellTextChangeCommand(
                spreadsheet.GetCell("A1"), emptyCellText);
            List<ICommand> commands = new List<ICommand>();
            commands.Add(cellTextChangeCommand);
            spreadsheet.AddUndo(commands);

            spreadsheet.GetCell("A1").Text = "Hello";

            cellTextChangeCommand = new CellTextChangeCommand(
                spreadsheet.GetCell("B1"), emptyCellText);
            commands = new List<ICommand>();
            commands.Add(cellTextChangeCommand);
            spreadsheet.AddUndo(commands);

            spreadsheet.GetCell("B1").Text = "Hello";

            spreadsheet.Undo();
            spreadsheet.Undo();

            Assert.AreEqual(emptyCellText, spreadsheet.GetCell("A1").Text);
            Assert.AreEqual(emptyCellBG, spreadsheet.GetCell("A1").BGColor);

            // Assert no other cells in spreadsheet were changed
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    string cellName = "" + (char)(i + (int)'A') + (j + 1).ToString();
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
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);

            string a1Text = spreadsheet.GetCell("A1").Text;
            uint a1BG = spreadsheet.GetCell("A1").BGColor;

            CellTextChangeCommand cellTextChangeCommand = new CellTextChangeCommand(
                spreadsheet.GetCell("A1"), a1Text);
            List<ICommand> commands = new List<ICommand>();
            commands.Add(cellTextChangeCommand);
            spreadsheet.AddUndo(commands);

            spreadsheet.GetCell("A1").Text = "Hello";

            spreadsheet.Undo();

            Assert.AreEqual(a1Text, spreadsheet.GetCell("A1").Text);
            Assert.AreEqual(a1BG, spreadsheet.GetCell("A1").BGColor);

            // Assert no other cells in spreadsheet were changed
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    string cellName = "" + (char)(i + (int)'A') + (j + 1).ToString();
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
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);

            // Set up some test data
            spreadsheet.GetCell("A1").Text = "Hello";
            spreadsheet.GetCell("A1").BGColor = 123456;
            spreadsheet.GetCell("B1").Text = "World";
            spreadsheet.GetCell("B1").BGColor = 654321;

            // Save spreadsheet
            using (FileStream fileStream = new FileStream(this.SPREADSHEET_TEST_FILEPATH, FileMode.Create))
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
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);

            string a1Text = spreadsheet.GetCell("A1").Text;
            uint a1BG = spreadsheet.GetCell("A1").BGColor;

            spreadsheet.GetCell("A1").Text = "Hello";

            CellTextChangeCommand cellTextChangeCommand = new CellTextChangeCommand(
               spreadsheet.GetCell("A1"), a1Text);
            List<ICommand> commands = new List<ICommand>();
            commands.Add(cellTextChangeCommand);
            spreadsheet.AddUndo(commands);

            spreadsheet.Undo();
            spreadsheet.Redo();

            Assert.AreEqual("Hello", spreadsheet.GetCell("A1").Text);

            // Assert no other cells in spreadsheet were changed
            for (int i = 1; i < 26; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    string cellName = "" + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(a1Text, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(a1BG, spreadsheet.GetCell(cellName).BGColor);
                }
            }

            // Check all cells in first row except A1
            for (int i = 0; i < 1; i++)
            {
                for (int j = 1; j < 50; j++)
                {
                    string cellName = "" + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(a1Text, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(a1BG, spreadsheet.GetCell(cellName).BGColor);
                }
            }
        }

        /// <summary>
        /// Change the BGColor of a spreadsheet and update the undo stacks.
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to change. </param>
        private void ChangeBG(ref Spreadsheet spreadsheet, string cellName, uint value, uint emptyCellBG)
        {
            spreadsheet.GetCell(cellName).BGColor = value;
            BackgroundChangeCommand backgroundChangeCommand = new BackgroundChangeCommand(
                spreadsheet.GetCell(cellName), emptyCellBG);
            List<ICommand> commands = new List<ICommand>();
            commands.Add(backgroundChangeCommand);
            spreadsheet.AddUndo(commands);
        }

        /// <summary>
        /// Test multipe undo operations done one after the other.
        /// </summary>
        [Test]
        public void UndoBackgroundMultipleTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);

            string emptyCellText = spreadsheet.GetCell("A1").Text;
            uint emptyCellBG = spreadsheet.GetCell("A1").BGColor;
            Assert.AreNotEqual(emptyCellBG, 123456);

            this.ChangeBG(ref spreadsheet, "A1", 123456, emptyCellBG);
            this.ChangeBG(ref spreadsheet, "B1", 123456, emptyCellBG);

            spreadsheet.Undo();
            spreadsheet.Undo();

            // Assert all cells in spreadsheet were reverted to original
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    string cellName = "" + (char)(i + (int)'A') + (j + 1).ToString();
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
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);

            string a1Text = spreadsheet.GetCell("A1").Text;
            uint a1BG = spreadsheet.GetCell("A1").BGColor;

            this.ChangeBG(ref spreadsheet, "A1", 123456, a1BG);

            spreadsheet.Undo();

            Assert.AreEqual(a1Text, spreadsheet.GetCell("A1").Text);
            Assert.AreEqual(a1BG, spreadsheet.GetCell("A1").BGColor);

            // Assert no other cells in spreadsheet were changed
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    string cellName = "" + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(a1Text, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(a1BG, spreadsheet.GetCell(cellName).BGColor);
                }
            }
        }

        /// <summary>
        /// Test single undo operation.
        /// </summary>
        [Test]
        public void RedoBackgroundSingleTest()
        {
            // Arrange
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);

            string a1Text = spreadsheet.GetCell("A1").Text;
            uint a1BG = spreadsheet.GetCell("A1").BGColor;

            this.ChangeBG(ref spreadsheet, "A1", 123456, a1BG);

            spreadsheet.Undo();
            spreadsheet.Redo();

            Assert.AreEqual(123456, spreadsheet.GetCell("A1").BGColor);

            // Assert no other cells in spreadsheet were changed
            for (int i = 1; i < 26; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    string cellName = "" + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(a1Text, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(a1BG, spreadsheet.GetCell(cellName).BGColor);
                }
            }

            // Check all cells in first row except A1
            for (int i = 0; i < 1; i++)
            {
                for (int j = 1; j < 50; j++)
                {
                    string cellName = "" + (char)(i + (int)'A') + (j + 1).ToString();
                    Assert.AreEqual(a1Text, spreadsheet.GetCell(cellName).Text);
                    Assert.AreEqual(a1BG, spreadsheet.GetCell(cellName).BGColor);
                }
            }
        }
    }
}
