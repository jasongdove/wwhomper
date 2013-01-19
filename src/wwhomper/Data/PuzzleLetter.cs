namespace wwhomper.Data
{
    public class PuzzleLetter
    {
        private readonly PuzzleGearSize _size;

        public PuzzleLetter(string letter, PuzzleGearSize size)
        {
            Letter = letter;
            _size = size;
        }

        public string Letter { get; set; }

        public PuzzleGearSize Size
        {
            get { return _size; }
        }
    }
}