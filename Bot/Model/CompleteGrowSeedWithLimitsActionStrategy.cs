using System;
using System.Collections.Generic;
using System.Linq;

class CompleteGrowSeedWithLimitsActionStrategy : IStrategy
{
    Dictionary<int, int> _myTreeCounts = new Dictionary<int, int>();

    public Action SelectAction(Game game)
    {
        for (int size = 0; size < 4; size++)
        {
            var treeCount = game.trees.Count(t => t.isMine && t.size == size);
            _myTreeCounts[size] = treeCount;
        }

        Action act = null;
        act = CompleteATree(game);
        if (act != null) return act;

        act = GrowATree(game);
        if (act != null) return act;

        act = SeedATree(game);
        if (act != null) return act;

        return game.possibleActions.Last();
    }

    private Action CompleteATree(Game game)
    {
        var actions = game.possibleActions
            .Where(a => a.type == Action.COMPLETE)
            .OrderBy(a => a.targetCellIdx);

        if (game.day >= 20) return actions.FirstOrDefault();

        // if number of this kind of tree > 2
        var completeActionForExcessTrees = actions.FirstOrDefault(a =>
        {
            var tree = game.GetTreeFromLocation(a.targetCellIdx);
            var count = _myTreeCounts[tree.size];
            return count > 2;
        });
        return completeActionForExcessTrees;
    }

    private Action GrowATree(Game game)
    {
        Func<Action, int> sort;
        if (game.day < 10)
        {
            sort = a => a.targetCellIdx;
        }
        else
        {
            sort = a =>
            {
                var tree = game.GetTreeFromLocation(a.targetCellIdx);
                return 5 - tree.size;
            };
        }

        var actions = game.possibleActions
            .Where(a => a.type == Action.GROW)
            .OrderBy(sort);

        // if number of this kind of tree > 2
        var growTreesWithoutTooManyBiggerTrees = actions.FirstOrDefault(a =>
        {
            var tree = game.GetTreeFromLocation(a.targetCellIdx);
            var count = _myTreeCounts[tree.size + 1]; // shouldn't need to check out of bounds here
            return count < 3;
        });
        return growTreesWithoutTooManyBiggerTrees;
    }

    private Action SeedATree(Game game)
    {
        // only plant seeds if we have fewer than 2
        if (_myTreeCounts[0] >= 2) return null;

        var seedActions = game.possibleActions
            .Where(a => a.type == Action.SEED)
            .OrderBy(a => a.targetCellIdx);

        return seedActions.FirstOrDefault();
    }

    // Strategies to consider
    // Don't seed next to my existing trees
    // If it's Day 20+, don't seed - save the sun for grow/complete
}

