using System;
using System.Collections.Generic;
using System.Linq;

class CompleteGrowSeedWithLimitsActionStrategy : IStrategy
{
    Dictionary<int, int> _myTreeCounts = new Dictionary<int, int>();
    Dictionary<int, int> _oppTreeCounts = new Dictionary<int, int>();

    public Action SelectAction(Game game)
    {
        for (int size = 0; size < 4; size++)
        {
            _myTreeCounts[size] = game.trees
                .Count(t => t.isMine && t.size == size); ;
            _oppTreeCounts[size] = game.trees
                .Count(t => !t.isMine && t.size == size); ;
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
        Console.Error.WriteLine("Tree Sizes:Me:Op");
        foreach (int key in _myTreeCounts.Keys)
        {
            Console.Error.WriteLine($"{key.ToString(":10")} : {_myTreeCounts[key]}");
        }
    }

    private Action CompleteATree(Game game)
    {
        var actions = game.possibleActions
            .Where(a => a.type == Action.COMPLETE)
            .OrderBy(a => a.targetCellIdx);

        if (game.day >= 22)
        {
            // don't complete trees that give 0 points
            return actions.FirstOrDefault(a => (game.nutrients + game.board[a.targetCellIdx].richness > 0));
        }

        // complete *then* grow if we have enough sun
        if (game.mySun >= 11 + _myTreeCounts[3] &&
            _myTreeCounts[3] > 3)
        {
            return actions.FirstOrDefault();
        }
        
        // TODO: Prioritize completing tree that's going to be in shade tomorrow?

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
        Func<Action, int> sort, sort2;

        // grow trees we have the fewest of next size (cheapest)
        sort = a =>
        {
            var tree = game.GetTreeFromLocation(a.targetCellIdx);
            return _myTreeCounts[tree.size+1];
        };

        // grow bigger trees before smaller trees (to make room)
        sort2 = a =>
        {
            var tree = game.GetTreeFromLocation(a.targetCellIdx);
            return 4-tree.size;
        };

        var actions = game.possibleActions
            .Where(a => a.type == Action.GROW)
            .OrderBy(sort)
            .ThenBy(sort2)
            .ThenBy(a => a.targetCellIdx)
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
        if (treeSize == 1) return 2;
        if (treeSize == 2) return 2;
        if (treeSize == 3) return 5;

        return 0;
    }

    private Action SeedATree(Game game)
    {
        // only plant seeds if we have none
        if (_myTreeCounts[0] >= 1) return null;

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
            Console.Error.WriteLine($"Score:neighbors: {score}");
        }

        // favor toward the center
        if (targetIndex < 7) score += 4;
        if (targetIndex < 19) score += 2;

        if (verbose)
        {
            Console.Error.WriteLine($"Score:location1: {score}");
        }

        if (game.board[sourceIndex].ShadeFreeLocations().Contains(targetIndex))
        {
            score += 3;
        }
        if(game.nutrients < 10)
        {
            if (targetIndex < 7) score += 4;
            if (targetIndex < 19) score += 2;
        }

        if (verbose)
        {
            Console.Error.WriteLine($"Score:location2: {score}");
        }

        // avoid throwing seeds from small trees
        if (myTrees.First(t => t.cellIndex == sourceIndex).size < 2)
        {
            score -= 1;
        }

        if (verbose)
        {
            Console.Error.WriteLine($"Score:smalltrees: {score}");
        }

        if (game.nutrients < 4 && targetIndex >= 19)
        {
            score = -100;
        }

        return score;
    }
}

