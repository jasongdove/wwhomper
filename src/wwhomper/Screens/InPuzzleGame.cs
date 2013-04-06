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
    public class InPuzzleGame : TemplateScreen, IInPuzzleGame
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
                0, 550, 320, 50)
        {
            _submit = CreateCoordinateButton(269, 303, 92, 41);
            _back = CreateCoordinateButton(73, 40, 76, 22);

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
                { new Rectangle(29, 370, 76, 82), new Rectangle(55, 398, 25, 27) },
                { new Rectangle(119, 368, 79, 86), new Rectangle(146, 398, 25, 27) },
                { new Rectangle(214, 373, 73, 78), new Rectangle(238, 398, 25, 27) },
                { new Rectangle(301, 373, 75, 77), new Rectangle(326, 398, 25, 27) },
                { new Rectangle(392, 372, 76, 79), new Rectangle(417, 398, 25, 27) },
                { new Rectangle(483, 371, 76, 83), new Rectangle(509, 398, 25, 27) },
                { new Rectangle(577, 375, 73, 76), new Rectangle(600, 398, 25, 27) },
                { new Rectangle(670, 375, 72, 74), new Rectangle(693, 398, 25, 27) },

                // bottom row
                { new Rectangle(33, 465, 72, 73), new Rectangle(56, 486, 25, 27) },
                { new Rectangle(123, 461, 71, 78), new Rectangle(146, 486, 25, 27) },
                { new Rectangle(215, 463, 71, 74), new Rectangle(237, 486, 25, 27) },
                { new Rectangle(300, 463, 74, 73), new Rectangle(325, 486, 25, 27) },
                { new Rectangle(394, 465, 70, 72), new Rectangle(416, 486, 25, 27) },
                { new Rectangle(485, 461, 73, 77), new Rectangle(509, 486, 25, 27) },
                { new Rectangle(576, 462, 73, 75), new Rectangle(600, 486, 25, 27) },
            };

            _gearOne = CreateCoordinateButton(234, 125, 9, 9);
            _gearTwo = CreateCoordinateButton(292, 155, 8, 7);
            _gearThree = CreateCoordinateButton(354, 147, 11, 8);
            _gearFour = CreateCoordinateButton(412, 173, 9, 10);
            _gearFive = CreateCoordinateButton(476, 165, 7, 6);

            _gearSpotOne = new Rectangle(203, 92, 72, 74);
            _gearSpotTwo = new Rectangle(260, 123, 72, 72);
            _gearSpotThree = new Rectangle(322, 114, 73, 72);
            _gearSpotFour = new Rectangle(380, 139, 70, 73);
            _gearSpotFive = new Rectangle(443, 130, 72, 75);

            _torchSpot = new Rectangle(179, 259, 76, 117);
            _paintSpot = new Rectangle(0, 241, 208, 155);

            _trash = new Rectangle(676, 474, 51, 59);
            _trashConfirm = CreateCoordinateButton(524, 266, 98, 23);

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

            AutoIt.MoveMouseOffscreen();
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
                gearSpotMessage(0),
                gearSpotMessage(1),
                gearSpotMessage(2),
                gearSpotMessage(3),
                gearSpotMessage(4));

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

            Logger.Debug(
                "Detected gears - count={0}, gears={1}",
                gears.Count,
                String.Join(" ", gears));

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