using DP1_Sudoku.BusinessLogic;
using DP1_Sudoku.BusinessLogic.Factories;
using DP1_Sudoku.BusinessLogic.Interfaces;
using DP1_Sudoku.BusinessLogic.Strategies.CellValueStrategies;
using DP1_Sudoku.BusinessLogic.Strategies.SolveStrategies;
using DP1_Sudoku.Shared;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace DP1_Sudoku.Pages.Game
{
    public partial class GamePage
    {
        [Parameter] public string? Name { get; set; }
        [Parameter] public string? Extension { get; set; }
        [Inject] public NavigationManager? NavManager { get; set; }
        [Inject] public IPuzzleObjectFactory? PuzzleObjectFactory { get; set; }

        private IBoard? _board;
        private PuzzleComponent? _puzzle;
        private PuzzleDisplaySettings _puzzleDisplaySettings = new();

        private ICellValueStrategy _valueStrategy = new CellValueStrategy();

        private bool _boardCompleted = false;

        protected override async Task OnParametersSetAsync()
        {
            // Check if parameters have been set
            if (Name == null || Extension == null)
            {
                NavManager?.NavigateTo("/gameNotFound");
                await base.OnParametersSetAsync();
                return;
            }

            // Check if dependency injection found all needed objects
            if (PuzzleObjectFactory == null)
            {
                await base.OnParametersSetAsync();
                return;
            }

            // Fetch puzzle from parameters
            PuzzleObject? _puzzle = await PuzzleObjectFactory.LoadPuzzle(Name, Extension);

            // Check if puzzle exists
            if (_puzzle == null)
            {
                NavManager?.NavigateTo("/gameNotFound");
                await base.OnParametersSetAsync();
                return;
            }

            // Init puzzle
            await InitPuzzle(_puzzle);

            _boardCompleted = false;

            // Call base
            await base.OnParametersSetAsync();
        }

        private async Task InitPuzzle(PuzzleObject puzzleObject)
        {
            string extension = puzzleObject.FileExtension;
            string[] lines = await puzzleObject.GetPuzzleString();

            _board = BoardFactory.GetInstance().CreateBoard(extension, lines);

            if (_board != null)
                _board.SolveStrategy = new BackTrackSolveStrategy();
        }

        private void ToggleShowAuxiliaryNumbers()
        {
            _puzzleDisplaySettings.ShowAuxiliaryNumbers = !_puzzleDisplaySettings.ShowAuxiliaryNumbers;
        }

        private void ToggleColorInvalidNumbers()
        {
            _puzzleDisplaySettings.ColorInvalidNumbers = !_puzzleDisplaySettings.ColorInvalidNumbers;
        }

        private void SetCellValue(int value)
        {
            Cell? selectedCell = _puzzle?.SelectedCellComponent?.Cell;
            if (selectedCell != null)
            {
                _valueStrategy?.SetValue(selectedCell, value);
                _board?.VerifyBoard();
            }
        }

        private void CheckBoard()
        {
            if (_board == null) return;
            _boardCompleted = _board.Validate();
        }

        private void AutoSolve()
        {
            if (_board == null) return;
            _board.Solve();
        }
    }

    public class PuzzleDisplaySettings
    {
        public bool ShowAuxiliaryNumbers { get; set; } = false;
        public bool ColorInvalidNumbers { get; set; } = false;
    }
}