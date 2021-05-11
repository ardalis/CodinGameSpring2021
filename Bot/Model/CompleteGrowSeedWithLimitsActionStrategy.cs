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

        // halfway through game, cut down big trees if we have enough size 2s
        if (game.day >= 12 && _myTreeCounts[2] > 1)
        {
            return actions.FirstOrDefault();
        }

        // if size 3 trees is at max size
        var completeActionForExcessTrees = actions.FirstOrDefault(a =>
        {
            var tree = game.GetTreeFromLocation(a.targetCellIdx);
            var count = _myTreeCounts[tree.size];
            var max = GetMaxTreesOfSize(3);
            if (game.day > 21) max = max - 1;
            return count == GetMaxTreesOfSize(3);
        });
        return completeActionForExcessTrees;
    }

    private Action GrowATree(Game game)
    {
        if (game.day == 23) return null; // don't waste sun growing on last turn
        Func<Action, int> sort;

        sort = a =>
        {
            var tree = game.GetTreeFromLocation(a.targetCellIdx);
            return a.targetCellIdx - tree.size;
        };

        var actions = game.possibleActions
            .Where(a => a.type == Action.GROW)
            .OrderBy(sort)
            .ToList();

        // if low on nutrients only grow inner trees
        if(game.nutrients < 5)
        {
            actions = actions.Where(a => a.targetCellIdx < 19).ToList();
        }

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

        var chosenAction = seedActions.FirstOrDefault();
        if(chosenAction != null)
        {
            int score = ScoreSeedLocation(chosenAction.sourceCellIdx, chosenAction.targetCellIdx, game, verbose:true);
            Console.Error.WriteLine($"Seed Location Score: {score}");
        }

        return chosenAction;
    }

    private int ScoreSeedLocation(int sourceIndex, int targetIndex, Game game, bool verbose = false)
    {
        int score = 0;
        var myTrees = game.trees.Where(t => t.isMine);
        var theirTrees = game.trees.Where(t => !t.isMine);

        int myAdjacentTrees = myTrees.Count(t => game.board[t.cellIndex].neighbours.Contains(targetIndex));
        int theirAdjacentTrees = theirTrees.Count(t => game.board[t.cellIndex].neighbours.Contains(targetIndex));

        score -= myAdjacentTrees * 2;
        score -= theirAdjacentTrees;

        if(verbose)
        {
            Console.Error.WriteLine($"Score with neighbors: {score}");
        }

        // favor toward the center
        if (targetIndex < 7) score += 2;
        if (targetIndex < 19) score += 1;

        if (verbose)
        {
            Console.Error.WriteLine($"Score with location1: {score}");
        }

        if (game.board[sourceIndex].ShadeFreeLocations().Contains(targetIndex))
        {
            score += 3;
        }
        if(game.nutrients < 10)
        {
            if (targetIndex < 7) score += 2;
            if (targetIndex < 19) score += 1;
        }

        if (verbose)
        {
            Console.Error.WriteLine($"Score with shade and location2: {score}");
        }

        // avoid throwing seeds from small trees
        if (myTrees.First(t => t.cellIndex == sourceIndex).size < 2)
        {
            score -= 1;
        }

        if (verbose)
        {
            Console.Error.WriteLine($"Score with small trees rule: {score}");
        }

        if (game.nutrients < 4 && targetIndex >= 19)
        {
            score = -100;
        }

        return score;
    }
}

