using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace wwhomper.Font
{
    public class BitmapFont
    {
        private readonly List<FontCharacter> _characters = new List<FontCharacter>();

        public Image<Bgr, byte> Bitmap { get; set; }
        public int Ascent { get; set; }

        public List<FontCharacter> Characters
        {
            get { return _characters; }
        }

        public Image<Bgra, byte> GetImage(string text)
        {
            var textCharacters = text.Select(l => _characters.First(c => c.Character == l)).ToList();

            var image = new Image<Bgra, byte>(
                1 + textCharacters.Sum(c => Math.Max(c.Width, c.Rectangle.Width)) + textCharacters.Sum(c => c.Offset.X),
                textCharacters.Max(c => c.Rectangle.Height) + textCharacters.Max(c => c.Offset.Y),
                new Bgra(228, 227, 201, 255));

            int x = 1;
            foreach (var character in textCharacters)
            {
                if (character.Character == ' ')
                {
                    x += character.Width;
                    continue;
                }

                var targetRect = new Rectangle(
                    x - character.Offset.X,
                    character.Offset.Y,
                    character.Rectangle.Width,
                    character.Rectangle.Height);

                var mask = Bitmap.GetSubRect(character.Rectangle).Convert<Gray, byte>();
                var characterImage = new Image<Bgra, byte>(mask.Width, mask.Height);
                for (int cy = 0; cy < characterImage.Height; cy++)
                {
                    for (int cx = 0; cx < characterImage.Width; cx++)
                    {
                        var alpha = mask[cy, cx].Intensity / 253;
                        characterImage[cy, cx] = new Bgra(
                            101 * alpha + (228 * (1 - alpha)),
                            32 * alpha + (227 * (1 - alpha)),
                            21 * alpha + (201 * (1 - alpha)),
                            255);
                    }
                }

                characterImage.CopyTo(image.GetSubRect(targetRect));

                x += character.Width;
            }

            return image;
        }
    }
}