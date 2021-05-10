using System;

class RandomActionStrategy : IStrategy
{
    public Action SelectAction(Game game)
    {
        int index = new Random().Next(game.possibleActions.Count);
        return game.possibleActions[index];
    }
}
