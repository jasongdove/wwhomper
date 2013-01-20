using System.Collections.Generic;

namespace wwhomper.Dictionary
{
    public interface IWordList
    {
        bool ContainsWord(string word);
        List<string> OfLength(int length);
        char BestLetterForIndex(char[] letters, int index);
        char WorstLetterOverall(char[] letters);
    }
}