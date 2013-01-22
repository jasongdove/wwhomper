using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ninject.Extensions.Logging;

namespace wwhomper.Dictionary
{
    public class WordList : IWordList
    {
        private readonly ILogger _logger;

        private readonly HashSet<string> _words;
        private readonly Dictionary<char, int> _totalFrequency;
        private readonly Dictionary<int, Dictionary<char, int>> _indexedFrequency;

        public WordList(ILogger logger)
        {
            _logger = logger;

            _words = new HashSet<string>();
            _totalFrequency = new Dictionary<char, int>();
            _indexedFrequency = new Dictionary<int, Dictionary<char, int>>
            {
                { 0, new Dictionary<char, int>() },
                { 1, new Dictionary<char, int>() },
                { 2, new Dictionary<char, int>() },
                { 3, new Dictionary<char, int>() },
                { 4, new Dictionary<char, int>() },
                { 5, new Dictionary<char, int>() },
            };
        }

        public void Load(string fileName)
        {
            _logger.Debug("Loading word list - filename={0}", fileName);

            using (var file = new StreamReader(fileName))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    line = line.Trim().ToUpperInvariant();
                    if (line.Length >= 3 && line.Length <= 6)
                    {
                        AddWord(line);
                    }
                }
            }
        }

        protected void LoadFromDictionary(string dictionary)
        {
            _logger.Debug("Loading word list from dictionary string");

            using (var reader = new StringReader(dictionary))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(","))
                    {
                        line = line.Substring(0, line.IndexOf(",", StringComparison.OrdinalIgnoreCase));
                    }

                    line = line.Trim().ToUpperInvariant();
                    if (line.Length >= 3 && line.Length <= 6)
                    {
                        AddWord(line);
                    }
                }
            }
        }

        private void AddWord(string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                char c = word[i];

                if (!_totalFrequency.ContainsKey(c))
                {
                    _totalFrequency.Add(c, 0);
                }

                _totalFrequency[c]++;

                if (!_indexedFrequency[i].ContainsKey(c))
                {
                    _indexedFrequency[i].Add(c, 0);
                }

                _indexedFrequency[i][c]++;
            }

            _words.Add(word);
        }

        public bool ContainsWord(string word)
        {
            return _words.Contains(word);
        }

        public List<string> OfLength(int length)
        {
            return _words.Where(x => x.Length == length).ToList();
        }

        public char BestLetterForIndex(char[] letters, int index)
        {
            var sortedLetters = _indexedFrequency[index].OrderByDescending(x => x.Value).Select(x => x.Key);
            return sortedLetters.First(letters.Contains);
        }

        public char WorstLetterOverall(char[] letters)
        {
            var sortedLetters = _totalFrequency.OrderBy(x => x.Value).Select(x => x.Key);
            return sortedLetters.First(letters.Contains);
        }

        public int GetLetterRankingForIndex(char letter, int index)
        {
            var sortedLetters = _indexedFrequency[index].OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
            return sortedLetters.IndexOf(letter);
        }
    }
}