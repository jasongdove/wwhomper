using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using Ninject.Extensions.Logging;
using sharperbot.Assets;
using sharperbot.AutoIt;
using sharperbot.Font;
using sharperbot.Screens;
using sharperbot.Screens.Controls;
using wwhomper.Data;

namespace wwhomper.Screens
{
    public class InPuzzleGame : TemplateScreen
    {
        private readonly Button _submit;
        private readonly Button _back;

        private readonly Image<Bgra, byte> _inactiveGearSpot;
        private readonly Image<Bgra, byte> _largeCopperGearSpot;
        private readonly Image<Bgra, byte> _smallCopperGearSpot;
        private readonly Image<Bgra, byte> _largeSilverGearSpot;
        private readonly Image<Bgra, byte> _smallSilverGearSpot;
        private readonly Image<Bgra, byte> _largeGoldGearSpot;
        private readonly Image<Bgra, byte> _smallGoldGearSpot;

        private readonly Image<Bgra, byte> _largeWildcardGear;
        private readonly Image<Bgra, byte> _smallWildcardGear;
        private readonly Image<Bgra, byte> _largeCopperGear;
        private readonly Image<Bgra, byte> _smallCopperGear;
        private readonly Image<Bgra, byte> _largeSilverGear;
        private readonly Image<Bgra, byte> _smallSilverGear;
        private readonly Image<Bgra, byte> _largeGoldGear;
        private readonly Image<Bgra, byte> _smallGoldGear;

        private readonly Image<Bgra, byte> _torch;
        private readonly Image<Bgra, byte> _copperPaint;
        private readonly Image<Bgra, byte> _silverPaint;
        private readonly Image<Bgra, byte> _goldPaint;
        
        private readonly Dictionary<Rectangle, Rectangle> _letterSearchAreaTesseractArea;

        private readonly Button _gearOne;
        private readonly Button _gearTwo;
        private readonly Button _gearThree;
        private readonly Button _gearFour;
        private readonly Button _gearFive;
        
        private readonly Rectangle _gearSpotOne;
        private readonly Rectangle _gearSpotTwo;
        private readonly Rectangle _gearSpotThree;
        private readonly Rectangle _gearSpotFour;
        private readonly Rectangle _gearSpotFive;

        private readonly Rectangle _torchSpot;
        private readonly Rectangle _paintSpot;

        private readonly Rectangle _trash;
        private readonly Button _trashConfirm;

        private readonly BitmapFont _gearFont;

        public InPuzzleGame(IAutoIt autoIt, IAssetCatalog assetCatalog, ILogger logger, FontLoader fontLoader)
            : base(
                autoIt,
                assetCatalog,
                logger,
                @"Images\ALL\Game\puzzle_game\PuzzleGame_Background.jpg",
                11, 556, 277, 35,
                0, 569, 304, 60)
        {
            _submit = CreateCoordinateButton(271, 330, 93, 41);
            _back = CreateCoordinateButton(78, 66, 72, 22);

            // Gear spots
            _inactiveGearSpot = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_NonInteractive.jpg")
                .Copy(new Rectangle(25, 26, 38, 38));

            _largeCopperGearSpot = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\GearSpot_Large_Copper.jpg")
                .Copy(new Rectangle(30, 32, 29, 29));

            _smallCopperGearSpot = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\GearSpot_Small_Copper.jpg")
                .Copy(new Rectangle(34, 37, 20, 19));

            _largeSilverGearSpot = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\GearSpot_Large_Silver.jpg")
                .Copy(new Rectangle(33, 34, 24, 25));

            _smallSilverGearSpot = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\GearSpot_Small_Silver.jpg")
                .Copy(new Rectangle(35, 37, 18, 18));

            _largeGoldGearSpot = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\GearSpot_Large_Gold.jpg")
                .Copy(new Rectangle(34, 37, 23, 22));

            _smallGoldGearSpot = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\GearSpot_Small_Gold.jpg")
                .Copy(new Rectangle(36, 38, 17, 17));

            // Gears
            _largeWildcardGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Large_Wildcard.jpg")
                .Copy(new Rectangle(30, 33, 28, 25));

            _smallWildcardGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Small_Wildcard.jpg")
                .Copy(new Rectangle(28, 29, 32, 32));

            _largeCopperGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Large_Copper.jpg")
                .Copy(new Rectangle(25, 65, 38, 3));

            _smallCopperGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Small_Copper.jpg")
                .Copy(new Rectangle(27, 32, 5, 32));

            _largeSilverGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Large_Silver.jpg")
                .Copy(new Rectangle(25, 65, 38, 3));

            _smallSilverGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Small_Silver.jpg")
                .Copy(new Rectangle(27, 32, 5, 32));

            _largeGoldGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Large_Gold.jpg")
                .Copy(new Rectangle(25, 65, 38, 3));

            _smallGoldGear = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\Gear_Small_Gold.jpg")
                .Copy(new Rectangle(27, 32, 5, 32));

            // Tools
            _torch = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\PowerUp_Torch_Idle.jpg")
                .Copy(new Rectangle(14, 59, 24, 31));

            _copperPaint = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\PowerUp_Brush_Copper_Idle.jpg")
                .Copy(new Rectangle(13, 51, 19, 32));

            _silverPaint = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\PowerUp_Brush_Silver_Idle.jpg")
                .Copy(new Rectangle(9, 59, 22, 32));

            _goldPaint = assetCatalog
                .GetCompositeImage(@"Images\ALL\Game\puzzle_game\PowerUp_Brush_Gold_Idle.jpg")
                .Copy(new Rectangle(6, 48, 28, 25));

            _letterSearchAreaTesseractArea = new Dictionary<Rectangle, Rectangle>
            {
                // top row
                { new Rectangle(32, 395, 76, 82), new Rectangle(58, 423, 25, 27) },
                { new Rectangle(122, 393, 79, 86), new Rectangle(149, 423, 25, 27) },
                { new Rectangle(217, 398, 73, 78), new Rectangle(241, 423, 25, 27) },
                { new Rectangle(304, 398, 75, 77), new Rectangle(329, 423, 25, 27) },
                { new Rectangle(395, 397, 76, 79), new Rectangle(420, 423, 25, 27) },
                { new Rectangle(486, 396, 76, 83), new Rectangle(512, 423, 25, 27) },
                { new Rectangle(580, 400, 73, 76), new Rectangle(603, 423, 25, 27) },
                { new Rectangle(673, 400, 72, 74), new Rectangle(696, 423, 25, 27) },

                // bottom row
                { new Rectangle(36, 490, 72, 73), new Rectangle(59, 511, 25, 27) },
                { new Rectangle(126, 486, 71, 78), new Rectangle(149, 511, 25, 27) },
                { new Rectangle(218, 488, 71, 74), new Rectangle(240, 511, 25, 27) },
                { new Rectangle(303, 488, 74, 73), new Rectangle(328, 511, 25, 27) },
                { new Rectangle(397, 490, 70, 72), new Rectangle(419, 511, 25, 27) },
                { new Rectangle(488, 486, 73, 77), new Rectangle(512, 511, 25, 27) },
                { new Rectangle(579, 487, 73, 75), new Rectangle(603, 511, 25, 27) },
            };

            _gearOne = CreateCoordinateButton(237, 150, 9, 9);
            _gearTwo = CreateCoordinateButton(295, 180, 8, 7);
            _gearThree = CreateCoordinateButton(357, 172, 11, 8);
            _gearFour = CreateCoordinateButton(415, 198, 9, 10);
            _gearFive = CreateCoordinateButton(479, 190, 7, 6);

            _gearSpotOne = new Rectangle(206, 119, 71, 73);
            _gearSpotTwo = new Rectangle(265, 150, 69, 69);
            _gearSpotThree = new Rectangle(327, 138, 71, 73);
            _gearSpotFour = new Rectangle(380, 164, 74, 74);
            _gearSpotFive = new Rectangle(448, 156, 70, 75);

            _torchSpot = new Rectangle(188, 288, 65, 98);
            _paintSpot = new Rectangle(10, 282, 192, 113);

            _trash = new Rectangle(690, 516, 32, 29);
            _trashConfirm = CreateCoordinateButton(527, 291, 98, 23);

            _gearFont = fontLoader.LoadFont(@"PG_GearLtr_Large23");
        }

        public Button Submit
        {
            get { return _submit; }
        }

        public Button Back
        {
            get { return _back; }
        }

        public void ClearAllGears()
        {
            Logger.Debug("Clearing all gear spots");

            _gearOne.Click();
            AutoIt.Type("{BACKSPACE}");
            AutoIt.WaitAfterInput();

            _gearTwo.Click();
            AutoIt.Type("{BACKSPACE}");
            AutoIt.WaitAfterInput();

            _gearThree.Click();
            AutoIt.Type("{BACKSPACE}");
            AutoIt.WaitAfterInput();

            _gearFour.Click();
            AutoIt.Type("{BACKSPACE}");
            AutoIt.WaitAfterInput();

            _gearFive.Click();
            AutoIt.Type("{BACKSPACE}");
            AutoIt.WaitAfterInput();
        }

        public List<PuzzleGearSpot> GetGearSpots()
        {
            var gearSpots = new List<PuzzleGearSpot>();

            do
            {
                var windowContents = AutoIt.GetWindowImage();
                gearSpots.Clear();

                var allGearSpots = new List<Rectangle> { _gearSpotOne, _gearSpotTwo, _gearSpotThree, _gearSpotFour, _gearSpotFive };
                for (int i = 0; i < allGearSpots.Count; i++)
                {
                    windowContents.ROI = allGearSpots[i];
                    if (AutoIt.IsTemplateInWindow(windowContents, _inactiveGearSpot).Success)
                    {
                        ////Logger.Debug("Found inactive gear in spot {0}", i);
                    }
                    else if (AutoIt.IsTemplateInWindow(windowContents, _largeCopperGearSpot).Success)
                    {
                        ////Logger.Debug("Found large copper gear in spot {0}", i);
                        gearSpots.Add(new PuzzleGearSpot(PuzzleGearSize.Large, PuzzleGearColor.Copper, i));
                    }
                    else if (AutoIt.IsTemplateInWindow(windowContents, _smallCopperGearSpot).Success)
                    {
                        ////Logger.Debug("Found small copper gear in spot {0}", i);
                        gearSpots.Add(new PuzzleGearSpot(PuzzleGearSize.Small, PuzzleGearColor.Copper, i));
                    }
                    else if (AutoIt.IsTemplateInWindow(windowContents, _largeSilverGearSpot).Success)
                    {
                        ////Logger.Debug("Found large silver gear in spot {0}", i);
                        gearSpots.Add(new PuzzleGearSpot(PuzzleGearSize.Large, PuzzleGearColor.Silver, i));
                    }
                    else if (AutoIt.IsTemplateInWindow(windowContents, _smallSilverGearSpot).Success)
                    {
                        ////Logger.Debug("Found small silver gear in spot {0}", i);
                        gearSpots.Add(new PuzzleGearSpot(PuzzleGearSize.Small, PuzzleGearColor.Silver, i));
                    }
                    else if (AutoIt.IsTemplateInWindow(windowContents, _largeGoldGearSpot).Success)
                    {
                        ////Logger.Debug("Found large gold gear in spot {0}", i);
                        gearSpots.Add(new PuzzleGearSpot(PuzzleGearSize.Large, PuzzleGearColor.Gold, i));
                    }
                    else if (AutoIt.IsTemplateInWindow(windowContents, _smallGoldGearSpot).Success)
                    {
                        ////Logger.Debug("Found small gold gear in spot {0}", i);
                        gearSpots.Add(new PuzzleGearSpot(PuzzleGearSize.Small, PuzzleGearColor.Gold, i));
                    }
                    else
                    {
                        Logger.Warn("Unable to detect gear spot - spot={0}", i);
                        SaveDebugImage(windowContents, "problemGearArea", String.Format("{0}.png", i));
                    }
                }
            } while (gearSpots.Count < 4); // this helps us retry if the "hint" highlight interferes

            Func<int, string> gearSpotMessage = x => gearSpots.Any(y => y.Index == x)
                ? gearSpots.First(y => y.Index == x).ToString()
                : "inactive";

            Logger.Debug(
                "Detected gear spots - one={0}, two={1}, three={2}, four={3}, five={4}",
                gearSpotMessage(1),
                gearSpotMessage(2),
                gearSpotMessage(3),
                gearSpotMessage(4),
                gearSpotMessage(5));

            return gearSpots;
        }

        public List<PuzzleTool> GetTools()
        {
            var windowContents = AutoIt.GetWindowImage();

            var tools = new List<PuzzleTool>();

            var torchSearchArea = windowContents.Copy(_torchSpot);
            var torchSearch = AutoIt.IsTemplateInWindow(torchSearchArea, _torch);
            if (torchSearch.Success)
            {
                var pickupArea = new Rectangle(
                    _torchSpot.X + torchSearch.Point.X,
                    _torchSpot.Y + torchSearch.Point.Y,
                    _torch.Size.Width,
                    _torch.Size.Height);

                tools.Add(new PuzzleTorch(pickupArea));
            }

            var paintSearchArea = windowContents.Copy(_paintSpot);
            var copperPaintSearch = AutoIt.IsTemplateInWindow(paintSearchArea, _copperPaint);
            if (copperPaintSearch.Success)
            {
                var pickupArea = new Rectangle(
                    _paintSpot.X + copperPaintSearch.Point.X,
                    _paintSpot.Y + copperPaintSearch.Point.Y,
                    _copperPaint.Size.Width,
                    _copperPaint.Size.Height);

                tools.Add(new PuzzlePaint(PuzzleGearColor.Copper, pickupArea));
            }

            var silverPaintSearch = AutoIt.IsTemplateInWindow(paintSearchArea, _silverPaint);
            if (silverPaintSearch.Success)
            {
                var pickupArea = new Rectangle(
                    _paintSpot.X + silverPaintSearch.Point.X,
                    _paintSpot.Y + silverPaintSearch.Point.Y,
                    _silverPaint.Size.Width,
                    _silverPaint.Size.Height);

                tools.Add(new PuzzlePaint(PuzzleGearColor.Silver, pickupArea));
            }

            var goldPaintSearch = AutoIt.IsTemplateInWindow(paintSearchArea, _goldPaint);
            if (goldPaintSearch.Success)
            {
                var pickupArea = new Rectangle(
                    _paintSpot.X + goldPaintSearch.Point.X,
                    _paintSpot.Y + goldPaintSearch.Point.Y,
                    _goldPaint.Size.Width,
                    _goldPaint.Size.Height);

                tools.Add(new PuzzlePaint(PuzzleGearColor.Gold, pickupArea));
            }

            Logger.Debug(
                "Detected tools - copperPaint={0}, silverPaint={1}, goldPaint={2}, torch={3}",
                tools.OfType<PuzzlePaint>().Any(x => x.Color.HasFlag(PuzzleGearColor.Copper)),
                tools.OfType<PuzzlePaint>().Any(x => x.Color.HasFlag(PuzzleGearColor.Silver)),
                tools.OfType<PuzzlePaint>().Any(x => x.Color.HasFlag(PuzzleGearColor.Gold)),
                tools.OfType<PuzzleTorch>().Any());

            return tools;
        }

        public List<PuzzleGear> GetGears()
        {
            var gears = new List<PuzzleGear>();

            var windowContents = AutoIt.GetWindowImage();

            foreach (var entry in _letterSearchAreaTesseractArea)
            {
                PuzzleGear gear = null;

                var searchArea = windowContents.Copy(entry.Key);
                if (AutoIt.IsTemplateInWindow(searchArea, _largeWildcardGear).Success)
                {
                    gear = new PuzzleGear(
                        '*',
                        PuzzleGearSize.Large | PuzzleGearSize.Small,
                        PuzzleGearColor.Copper | PuzzleGearColor.Silver | PuzzleGearColor.Gold,
                        entry.Value);
                }
                else if (AutoIt.IsTemplateInWindow(searchArea, _smallWildcardGear).Success)
                {
                    gear = new PuzzleGear(
                        '*',
                        PuzzleGearSize.Large | PuzzleGearSize.Small,
                        PuzzleGearColor.Copper | PuzzleGearColor.Silver | PuzzleGearColor.Gold,
                        entry.Value);
                }
                else if (AutoIt.IsTemplateInWindow(searchArea, _largeCopperGear).Success)
                {
                    gear = new PuzzleGear(
                        GetLetterForGear(searchArea),
                        PuzzleGearSize.Large,
                        PuzzleGearColor.Copper,
                        entry.Value);
                }
                else if (AutoIt.IsTemplateInWindow(searchArea, _smallCopperGear).Success)
                {
                    gear = new PuzzleGear(
                        GetLetterForGear(searchArea),
                        PuzzleGearSize.Small,
                        PuzzleGearColor.Copper,
                        entry.Value);
                }
                else if (AutoIt.IsTemplateInWindow(searchArea, _largeSilverGear).Success)
                {
                    gear = new PuzzleGear(
                        GetLetterForGear(searchArea),
                        PuzzleGearSize.Large,
                        PuzzleGearColor.Silver,
                        entry.Value);
                }
                else if (AutoIt.IsTemplateInWindow(searchArea, _smallSilverGear).Success)
                {
                    gear = new PuzzleGear(
                        GetLetterForGear(searchArea),
                        PuzzleGearSize.Small,
                        PuzzleGearColor.Silver,
                        entry.Value);
                }
                else if (AutoIt.IsTemplateInWindow(searchArea, _largeGoldGear).Success)
                {
                    gear = new PuzzleGear(
                        GetLetterForGear(searchArea),
                        PuzzleGearSize.Large,
                        PuzzleGearColor.Gold,
                        entry.Value);
                }
                else if (AutoIt.IsTemplateInWindow(searchArea, _smallGoldGear).Success)
                {
                    gear = new PuzzleGear(
                        GetLetterForGear(searchArea),
                        PuzzleGearSize.Small,
                        PuzzleGearColor.Gold,
                        entry.Value);
                }

                if (gear != null)
                {
                    gears.Add(gear);
                }
            }

            Logger.Debug("Detected gears - {0}", String.Join(" ", gears));

            return gears;
        }

        public void SubmitAnswer(List<PuzzleStep> steps)
        {
            Func<int, string> puzzleString = i => i < steps.Count
                ? steps[i].Gear.ToString() + (steps[i].Tool != null ? "/" + steps[i].Tool.ToString() : String.Empty)
                : "nothing";

            Logger.Debug(
                "Solved puzzle - one={0}, two={1}, three={2}, four={3}, five={4}",
                puzzleString(0),
                puzzleString(1),
                puzzleString(2),
                puzzleString(3),
                puzzleString(4));

            var allGearButtons = new[] { _gearOne, _gearTwo, _gearThree, _gearFour, _gearFive };
            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                if (step.Tool != null)
                {
                    ApplyTool(step.Tool, step.Gear);
                }

                AutoIt.Click(step.Gear.PickupArea);
                allGearButtons[i].Click();

                AutoIt.WaitAfterInput();
            }

            _submit.Click();
        }

        public void ApplyTool(PuzzleTool tool, PuzzleGear gear)
        {
            Logger.Debug(
                "Applying tool - tool={0}, gear={1}",
                tool.ToString(),
                gear.ToString());

            AutoIt.Click(tool.PickupArea);
            AutoIt.Click(gear.PickupArea);

            Thread.Sleep(2500);
        }

        public void Trash(PuzzleGear gear)
        {
            Logger.Debug("Trashing gear - gear={1}", gear.ToString());

            AutoIt.Click(gear.PickupArea);
            AutoIt.Click(_trash);
            AutoIt.WaitAfterInput();
            _trashConfirm.Click();
        }

        private char GetLetterForGear(Image<Bgra, byte> gearImage)
        {
            double max = 0;
            char bestCharacter = '?';

            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            foreach (var character in _gearFont.Characters.Where(x => alphabet.Contains(x.Character)))
            {
                var match = gearImage.MatchTemplate(character.MatchImage, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCOEFF_NORMED);
                double[] currentMins, currentMaxes;
                Point[] minPoints, maxPoints;
                match.MinMax(out currentMins, out currentMaxes, out minPoints, out maxPoints);

                if (currentMaxes[0] > max)
                {
                    max = currentMaxes[0];
                    bestCharacter = character.Character;
                }
            }

            return bestCharacter;
        }
    }
}