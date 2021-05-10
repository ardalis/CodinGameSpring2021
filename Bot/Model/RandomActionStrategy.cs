using Bot.Model;

class RandomActionStrategy : IStrategy
{
    public Action SelectAction(Game game)
    {
        int index = new System.Random().Next(game.possibleActions.Count);
        return game.possibleActions[index];
    }
}
