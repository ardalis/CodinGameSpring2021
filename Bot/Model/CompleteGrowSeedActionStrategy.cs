using System.Linq;

class CompleteGrowSeedActionStrategy : IStrategy
{
    public Action SelectAction(Game game)
    {
        Action act = null;
        act = game.possibleActions
            .OrderBy(a => a.targetCellIdx)
            .FirstOrDefault(a => a.type == Action.COMPLETE);
        if (act != null) return act;

        act = game.possibleActions
            .OrderBy(a => a.targetCellIdx)
            .FirstOrDefault(a => a.type == Action.GROW);
        if (act != null) return act;

        act = game.possibleActions
            .OrderBy(a => a.targetCellIdx)
            .FirstOrDefault(a => a.type == Action.SEED);
        if (act != null) return act;

        return game.possibleActions.Last();
    }
}

