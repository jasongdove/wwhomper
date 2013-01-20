namespace wwhomper.Data
{
    public class PuzzleStep
    {
        private readonly PuzzleGear _gear;
        private readonly PuzzleTool _tool;

        public PuzzleStep(PuzzleGear gear)
            : this(gear, null)
        {
        }

        public PuzzleStep(PuzzleGear gear, PuzzleTool tool)
        {
            _gear = gear;
            _tool = tool;
        }

        public PuzzleGear Gear
        {
            get { return _gear; }
        }

        public PuzzleTool Tool
        {
            get { return _tool; }
        }
    }
}
