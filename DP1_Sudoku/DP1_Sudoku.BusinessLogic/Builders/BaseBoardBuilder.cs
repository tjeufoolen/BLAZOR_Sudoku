﻿using DP1_Sudoku.BusinessLogic.Extensions;
using DP1_Sudoku.BusinessLogic.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DP1_Sudoku.BusinessLogic.Builders
{
    public abstract class BaseBoardBuilder : IBoardBuilder
    {
        public IBoard Board { get; protected set; } = new Board();

        public virtual void Reset()
        {
            Board = new Board();
        }

        public virtual void BuildCells(IList<string> lines)
        {
            var cells = CreateCells(lines);
            SetCellNeighbours(cells);

            Board.Cells = cells;
        }

        protected abstract Cell[,] CreateCells(IList<string> lines);

        protected virtual void SetCellNeighbours(Cell[,] cellRows)
        {
            int amountOfRows = cellRows.GetLength(0);
            int maxRowLength = cellRows.GetLength(1);

            for (int rowIdx = 0; rowIdx < amountOfRows; rowIdx++)
            {
                for (int charIdx = 0; charIdx < maxRowLength; charIdx++)
                {
                    Cell current = cellRows[rowIdx, charIdx];
                    if (current == null) break;

                    // Check for cell above current cell
                    if (rowIdx - 1 >= 0)
                        current.Top = cellRows[rowIdx - 1, charIdx];

                    // Check for cell below current cell
                    if (rowIdx + 1 <= amountOfRows - 1)
                        current.Bottom = cellRows[rowIdx + 1, charIdx];

                    // Check for cell left of current cell
                    if (charIdx - 1 >= 0)
                        current.Left = cellRows[rowIdx, charIdx - 1];

                    // Check for cell right of current cell
                    if (charIdx + 1 <= maxRowLength - 1)
                        current.Right = cellRows[rowIdx, charIdx + 1];
                }
            }
        }

        public void BuildGroups(IList<string> _)
        {
            BuildSubgroups();
            BuildRowGroups();
            BuildColumnGroups();
        }

        protected virtual void BuildRowGroups()
        {
            for (int row = 0; row < Board.Cells?.GetLength(0); row++)
            {
                var group = new GroupComposite();
                var rowCells = GetRow(Board.Cells, row);

                group.Children.AddRange(rowCells);

                Board.HorizontalGroups.Add(group);
            }
        }

        protected virtual void BuildColumnGroups()
        {
            for (int column = 0; column < Board.Cells?.GetLength(1); column++)
            {
                var group = new GroupComposite();
                var columnCells = GetColumn(Board.Cells, column);

                group.Children.AddRange(columnCells);

                Board.VerticalGroups.Add(group);
            }
        }
        protected abstract void BuildSubgroups();

        #region Helpers
        protected Cell[] GetRow(Cell[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }

        protected Cell[] GetColumn(Cell[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }
        #endregion Helpers
    }
}
