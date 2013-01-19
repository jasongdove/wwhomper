using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using sharperbot.Assets;
using sharperbot.AutoIt;

namespace sharperbot.Screens
{
    public abstract class TextScreen : GameScreen
    {
        private readonly Rectangle _rectangle;
        private readonly string _text;

        protected TextScreen(IAutoIt autoIt, IAssetCatalog assetCatalog, int x, int y, int width, int height, string text)
            : base(autoIt, assetCatalog)
        {
            AdditionalCharacters = String.Empty;
            RequiresZoom = false;

            _rectangle = new Rectangle(x, y, width, height);
            _text = text;
        }

        protected string AdditionalCharacters { get; set; }
        protected bool RequiresZoom { get; set; }

        public override ScreenSearchResult IsActive(Image<Bgra, byte> windowContents)
        {
            var image = windowContents.GetSubRect(_rectangle);
            var text = (RequiresZoom ? GetZoomedOutText(image, 8, AdditionalCharacters) : GetText(image, AdditionalCharacters)).Trim();
            
            var result = new ScreenSearchResult { Success = text == _text };
            if (result.Success)
            {
                result.Screen = this;
            }

            return result;
        }
    }
}