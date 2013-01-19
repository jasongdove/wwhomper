namespace sharperbot.Screens
{
    public interface IScreenStrategy<in T> : IScreenStrategy where T : IGameScreen
    {
        void ExecuteStrategy(T screen);
    }
}