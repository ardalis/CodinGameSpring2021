using System.Linq;

class LastActionStrategy : IStrategy
{
    public Action SelectAction(Game game)
    {
        return game.possibleActions.Last();
    }
}
