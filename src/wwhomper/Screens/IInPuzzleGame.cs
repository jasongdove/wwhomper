using System.Collections.Generic;
using sharperbot.Screens;
using sharperbot.Screens.Controls;
using wwhomper.Data;

namespace wwhomper.Screens
{
    public interface IInPuzzleGame : IGameScreen
    {
        Button Submit { get; }
        Button Back { get; }

        void ClearAllGears();
        List<PuzzleGearSpot> GetGearSpots();
        List<PuzzleTool> GetTools();
        List<PuzzleGear> GetGears();
        void SubmitAnswer(List<PuzzleStep> steps);
        void ApplyTool(PuzzleTool tool, PuzzleGear gear);
        void Trash(PuzzleGear gear);
    }
}