using System;
using System.Collections.Generic;
using System.IO;

namespace wwhomper
{
    public class WordList
    {
        private readonly HashSet<string> _words;

        public WordList()
        {
            _words = new HashSet<string>();
        }

        public void Load(string fileName)
        {
            using (var file = new StreamReader(fileName))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    line = line.Trim().ToUpperInvariant();
                    if (line.Length >= 3 && line.Length <= 6)
                    {
                        _words.Add(line);
                    }
                }
            }
        }

        public void LoadFromDictionary(string dictionary)
        {
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
                        _words.Add(line);
                    }
                }
            }
        }

        public bool ContainsWord(string word)
        {
            return _words.Contains(word);
        }
    }
}