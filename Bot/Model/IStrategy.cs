namespace Bot.Model
{
    interface IStrategy
    {
        Action SelectAction(Game game);
    }
}