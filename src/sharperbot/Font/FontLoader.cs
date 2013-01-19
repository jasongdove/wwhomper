using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Emgu.CV;
using Emgu.CV.Structure;

namespace sharperbot.Font
{
    public class FontLoader
    {
        private readonly string _gameRoot;

        public FontLoader(string gameRoot)
        {
            _gameRoot = gameRoot;
        }

        public BitmapFont LoadFont(string fontName)
        {
            string bitmapFileName = Path.Combine(_gameRoot, String.Format(@"Fonts\_{0}.png", fontName));
            string definitionFileName = Path.Combine(_gameRoot, String.Format(@"Fonts\{0}.txt", fontName));

            if (!File.Exists(bitmapFileName))
            {
                throw new FileNotFoundException(bitmapFileName);
            }

            if (!File.Exists(definitionFileName))
            {
                throw new FileNotFoundException(definitionFileName);
            }

            var font = new BitmapFont { Bitmap = new Image<Bgr, byte>(bitmapFileName) };

            var definition = File.ReadAllText(definitionFileName, Encoding.GetEncoding(1252));

            var definesRegex = new Regex(@"Define.*?\);", RegexOptions.Singleline);
            var matches = definesRegex.Matches(definition).OfType<Match>().ToList();
            var charList = matches.First(x => x.Value.Contains("CharList")).Value;
            var widthList = matches.First(x => x.Value.Contains("WidthList")).Value;
            var rectList = matches.First(x => x.Value.Contains("RectList")).Value;
            var offsetList = matches.First(x => x.Value.Contains("OffsetList")).Value;

            AddCharList(font, charList);
            AddWidthList(font, widthList);
            AddRectList(font, rectList);
            AddOffsetList(font, offsetList);

            font.Ascent = Int32.Parse(new Regex(@"LayerSetAscent .*?(\d+?);").Match(definition).Groups[1].Value);

            int spaceWidth = Int32.Parse(new Regex(@"LayerSetCharWidths\s+Main\s\(' '\) \((\d+?)\);").Match(definition).Groups[1].Value);
            font.Characters.Add(new FontCharacter { Character = ' ', Width = spaceWidth, Rectangle = new Rectangle(0, 0, spaceWidth, 0) });

            return font;
        }

        private static void AddCharList(BitmapFont font, string charList)
        {
            charList = charList.Replace("Define CharList", String.Empty)
                               .Replace(" ", String.Empty)
                               .Replace(Environment.NewLine, String.Empty)
                               .Replace("\n", String.Empty)
                               .Replace(",\"", ",'")
                               .Replace("\",", "',")
                               .TrimStart('(', '\'')
                               .TrimEnd(';', ')', '\'');

            foreach (var c in charList.Split(new[] { "','" }, StringSplitOptions.None))
            {
                var character = new FontCharacter
                                    {
                                        Character = c[0]
                                    };

                font.Characters.Add(character);
            }
        }

        private static void AddWidthList(BitmapFont font, string widthList)
        {
            widthList = widthList.Replace("Define WidthList", String.Empty)
                                 .Replace(" ", String.Empty)
                                 .Replace(Environment.NewLine, String.Empty)
                                 .Replace("\n", String.Empty)
                                 .TrimStart('(')
                                 .TrimEnd(';', ')');

            var widths = widthList.Split(new[] { "," }, StringSplitOptions.None);
            for (int i = 0; i < widths.Length; i++)
            {
                font.Characters[i].Width = Int32.Parse(widths[i]);
            }
        }

        private static void AddRectList(BitmapFont font, string rectList)
        {
            rectList = rectList.Replace("Define RectList", String.Empty)
                               .Replace(" ", String.Empty)
                               .Replace(Environment.NewLine, String.Empty)
                               .Replace("\n", String.Empty)
                               .TrimStart('(')
                               .TrimEnd(';', ')');

            var rects = rectList.Split(new[] { "),(" }, StringSplitOptions.None);
            for (int i = 0; i < rects.Length; i++)
            {
                var r = rects[i].Replace(")", String.Empty)
                                .Replace("(", String.Empty)
                                .Split(new[] { "," }, StringSplitOptions.None);

                var rect = new Rectangle(
                    Int32.Parse(r[0]),
                    Int32.Parse(r[1]),
                    Int32.Parse(r[2]),
                    Int32.Parse(r[3]));

                font.Characters[i].Rectangle = rect;
            }
        }

        private void AddOffsetList(BitmapFont font, string offsetList)
        {
            offsetList = offsetList.Replace("Define OffsetList", String.Empty)
                               .Replace(" ", String.Empty)
                               .Replace(Environment.NewLine, String.Empty)
                               .Replace("\n", String.Empty)
                               .TrimStart('(')
                               .TrimEnd(';', ')');

            var offsets = offsetList.Split(new[] { "),(" }, StringSplitOptions.None);
            for (int i = 0; i < offsets.Length; i++)
            {
                var o = offsets[i].Replace(")", String.Empty)
                                  .Replace("(", String.Empty)
                                  .Split(new[] { "," }, StringSplitOptions.None);

                var point = new Point(
                    Int32.Parse(o[0]),
                    Int32.Parse(o[1]));

                font.Characters[i].Offset = point;
            }
        }
    }
}