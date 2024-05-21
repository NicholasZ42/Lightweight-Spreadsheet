// <copyright file="SpreadsheetCell.cs" company="Nicholas Zheng">
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
    /// Cell inside a spreadsheet.
    /// </summary>
    public class SpreadsheetCell : Cell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetCell"/> class.
        /// </summary>
        /// <param name="rowInd"> Row index in spreadsheet. </param>
        /// <param name="colInd"> Column index in spreadsheet. </param>
        public SpreadsheetCell(int rowInd, int colInd)
            : base(rowInd, colInd)
        {
        }
    }
}
