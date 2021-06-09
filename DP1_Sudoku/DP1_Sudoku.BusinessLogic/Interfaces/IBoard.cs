﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace DP1_Sudoku.BusinessLogic.Interfaces
{
    public interface IBoard : IGridComponent
    {
        public ISolveStrategy? SolveStrategy { get; set; }
        Cell[,]? Cells { get; set; }
        IList<GroupComposite> SubGroups { get; set; }
        IList<GroupComposite> HorizontalGroups { get; set; }
        IList<GroupComposite> VerticalGroups { get; set; }
        public int MaxValidCellValue { get; }
        bool IsBoardCorrect();
        public bool ValidateGroups();
        void VerifyBoard();
        Task Solve(Task onCellValueUpdate);
    }
}
