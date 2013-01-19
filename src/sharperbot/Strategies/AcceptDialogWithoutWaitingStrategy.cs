using sharperbot.Screens;

namespace sharperbot.Strategies
{
    public class AcceptDialogWithoutWaitingStrategy : IScreenStrategy<IDialogScreen>
    {
        public void ExecuteStrategy(IDialogScreen screen)
        {
            screen.Accept.Click();
        }
    }
}