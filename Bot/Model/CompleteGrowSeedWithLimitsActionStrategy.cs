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

        PrintCounts();

        Action act = null;
        act = CompleteATree(game);
        if (act != null) return act;

        act = GrowATree(game);
        if (act != null) return act;

        act = SeedATree(game);
        if (act != null) return act;

        return game.possibleActions.First();
    }

    private void PrintCounts()
    {
        Console.Error.WriteLine("Tree Sizes:Counts");
        foreach (int key in _myTreeCounts.Keys)
        {
            Console.Error.WriteLine($"{key} : {_myTreeCounts[key]}");
        }
    }

    private Action CompleteATree(Game game)
    {
        var actions = game.possibleActions
            .Where(a => a.type == Action.COMPLETE)
            .OrderBy(a => a.targetCellIdx);

        if (game.day >= 21) return actions.FirstOrDefault();

        // if size 3 trees is at max size
        var completeActionForExcessTrees = actions.FirstOrDefault(a =>
        {
            var tree = game.GetTreeFromLocation(a.targetCellIdx);
            var count = _myTreeCounts[tree.size];
            return count == GetMaxTreesOfSize(3);
        });
        return completeActionForExcessTrees;
    }

    private Action GrowATree(Game game)
    {
        if (game.day == 23) return null; // don't waste sun growing on last turn
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
            var count = _myTreeCounts[tree.size+1]; // shouldn't need to check out of bounds here
            return count < GetMaxTreesOfSize(tree.size+1);
        });
        return growTreesWithoutTooManyBiggerTrees;
    }

    private int GetMaxTreesOfSize(int treeSize)
    {
        return treeSize + 2;
    }

    private Action SeedATree(Game game)
    {
        // only plant seeds if we have fewer than 2
        if (_myTreeCounts[0] >= 1) return null;
        if (game.day > 20 && _myTreeCounts[0] > 0) return null; // only plant if free

        var seedActions = game.possibleActions
            .Where(a => a.type == Action.SEED)
            .Where(a => ScoreSeedLocation(a.sourceCellIdx, a.targetCellIdx, game) > 0)
            .OrderByDescending(a => ScoreSeedLocation(a.sourceCellIdx, a.targetCellIdx, game))
            .ThenBy(a => a.targetCellIdx);

        return seedActions.FirstOrDefault();
    }

    private int ScoreSeedLocation(int sourceIndex, int targetIndex, Game game)
    {
        // score 0 if adjacent to one of my trees
        var myTrees = game.trees.Where(t => t.isMine);

        if (myTrees.Any(t => game.board[t.cellIndex].neighbours.Contains(targetIndex)))
        {
            return 0;
        }
        if (game.board[sourceIndex].ShadeFreeLocations().Contains(targetIndex))
        {
            return 2;
        }
        return 1;
        // TODO: range 2 locations not in-line with hexrows should score 2
    }

    // Strategies to consider
    // Don't seed next to my existing trees
    // If it's Day 20+, don't seed unless it's free - save the sun for grow/complete
}

