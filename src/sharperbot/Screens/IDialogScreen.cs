using sharperbot.Screens.Controls;

namespace sharperbot.Screens
{
    public interface IDialogScreen : IGameScreen
    {
        Button Accept { get; }
    }
}