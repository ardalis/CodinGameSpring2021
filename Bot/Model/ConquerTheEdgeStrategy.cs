using System;
using System.Collections.Generic;
using System.Linq;

class ConquerTheEdgeStrategy : IStrategy
{
    Dictionary<int, int> _myTreeCounts = new Dictionary<int, int>();
    Dictionary<int, int> _oppTreeCounts = new Dictionary<int, int>();
    private readonly Game _game;

    public ConquerTheEdgeStrategy(Game game)
    {
        _game = game;
        // game isn't fully loaded yet so don't perform logic here
    }

    public Action SelectAction(Game game = null)
    {
        for (int size = 0; size < 4; size++)
        {
            _myTreeCounts[size] = _game.trees
                .Count(t => t.isMine && t.size == size); ;
            _oppTreeCounts[size] = _game.trees
                .Count(t => !t.isMine && t.size == size); ;
        }

        PrintCounts();

        Action act = null;
        act = CompleteATree();
        if (act != null) return act;

        act = GrowATree();
        if (act != null) return act;

        act = SeedATree();
        if (act != null) return act;

        return _game.possibleActions.First();
    }

    private void PrintCounts()
    {
        Console.Error.WriteLine("Tree Sizes:Me:Op:Max");
        foreach (int key in _myTreeCounts.Keys)
        {
            Console.Error.WriteLine($"{key} : {_myTreeCounts[key]} : {_oppTreeCounts[key]} : {GetMaxTreesOfSize(key)}");
        }
    }

    private Action CompleteATree()
    {
        var actions = _game.possibleActions
            .Where(a => a.type == Action.COMPLETE)
            .OrderBy(a => a.targetCellIdx);

        if (_game.day >= 22)
        {
            // don't complete trees that give 0 points
            return actions.FirstOrDefault(a => (_game.nutrients + _game.board[a.targetCellIdx].richness > 0));
        }

        // complete *then* grow if we have enough sun
        if (_game.mySun >= 11 + _myTreeCounts[3] &&
            _myTreeCounts[3] > 3)
        {
            return actions.FirstOrDefault();
        }
        
        // TODO: Prioritize completing tree that's going to be in shade tomorrow?

        // if size 3 trees is at max size
        var completeActionForExcessTrees = actions.FirstOrDefault(a =>
        {
            var tree = _game.GetTreeFromLocation(a.targetCellIdx);
            var count = _myTreeCounts[3];
            var max = GetMaxTreesOfSize(3);
            return count >= max;
        });
        return completeActionForExcessTrees;
    }

    private Action GrowATree()
    {
        if (_game.day == 23) return null; // don't waste sun growing on last turn
        Func<Action, int> sort, sort2;

        // grow trees we have the fewest of next size (cheapest)
        sort = a =>
        {
            var tree = _game.GetTreeFromLocation(a.targetCellIdx);
            return _myTreeCounts[tree.size+1];
        };

        // grow bigger trees before smaller trees (to make room)
        sort2 = a =>
        {
            var tree = _game.GetTreeFromLocation(a.targetCellIdx);
            return 4-tree.size;
        };

        var actions = _game.possibleActions
            .Where(a => a.type == Action.GROW)
            .OrderBy(sort)
            .ThenBy(sort2)
            .ThenBy(a => a.targetCellIdx)
            .ToList();

        // if low on nutrients only grow inner trees
        if(_game.nutrients < 5)
        {
            actions = actions.Where(a => a.targetCellIdx < 19).ToList();
        }

        // if number of this kind of tree > 2
        var growTreesWithoutTooManyBiggerTrees = actions.FirstOrDefault(a =>
        {
            var tree = _game.GetTreeFromLocation(a.targetCellIdx);
            var count = _myTreeCounts[tree.size+1]; // shouldn't need to check out of bounds here
            return count < GetMaxTreesOfSize(tree.size+1);
        });
        return growTreesWithoutTooManyBiggerTrees;
    }

    private int GetMaxTreesOfSize(int treeSize)
    {
        if (treeSize == 1) return 2;
        if (treeSize == 2) return 2;

        if (treeSize == 3)
        {
            if(_game.nutrients <= 12)
            {
                return 2;
            }
            return 4;
        }
        return 0;
    }

    private Action SeedATree()
    {
        // only plant seeds if we have none planted
        if (_myTreeCounts[0] >= 1) return null;

        var seedActions = _game.possibleActions
            .Where(a => a.type == Action.SEED)
            .Where(a => ScoreSeedLocation(a.sourceCellIdx, a.targetCellIdx) > 0)
            .OrderByDescending(a => ScoreSeedLocation(a.sourceCellIdx, a.targetCellIdx))
            .ThenBy(a => a.targetCellIdx);

        var chosenAction = seedActions.FirstOrDefault();
        if(chosenAction != null)
        {
            int score = ScoreSeedLocation(chosenAction.sourceCellIdx, chosenAction.targetCellIdx, verbose:true);
            Console.Error.WriteLine($"Seed Location Score: {score}");
        }

        return chosenAction;
    }

    private int ScoreSeedLocation(int sourceIndex, int targetIndex, bool verbose = false)
    {
        // TODO: Prefer hex points especially 7,9,11,13,15,17
        int score = 0;
        var myTrees = _game.trees.Where(t => t.isMine);
        var theirTrees = _game.trees.Where(t => !t.isMine);
        int[] hexPoints = { 7, 9, 11, 13, 15, 17 };
        int[] mapCorners = { 19, 22, 25, 28, 31, 34 };

        int myAdjacentTrees = myTrees.Count(t => _game.board[t.cellIndex].neighbours.Contains(targetIndex));
        int theirAdjacentTrees = theirTrees.Count(t => _game.board[t.cellIndex].neighbours.Contains(targetIndex));

        score -= myAdjacentTrees * 2;
        score -= theirAdjacentTrees;

        if(verbose)
        {
            Console.Error.WriteLine($"Score:neighbors: {score}");
        }

        // favor around the center
        if ((targetIndex > 6) && (targetIndex < 19)) score += 2;
        if (hexPoints.Contains(targetIndex)) score += 1;
        if (mapCorners.Contains(targetIndex)) score += 2;

        if (verbose)
        {
            Console.Error.WriteLine($"Score:location1: {score}");
        }

        if (_game.board[sourceIndex].ShadeFreeLocations().Contains(targetIndex))
        {
            score += 3;
        }
        //if(game.nutrients < 10)
        //{
        //    if (targetIndex < 7) score += 4;
        //    if (targetIndex < 19) score += 2;
        //}

        if (verbose)
        {
            Console.Error.WriteLine($"Score:location2: {score}");
        }

        // avoid throwing seeds from small trees
        if (myTrees.First(t => t.cellIndex == sourceIndex).size < 2)
        {
            score -= 2;
        }

        if (verbose)
        {
            Console.Error.WriteLine($"Score:smalltrees: {score}");
        }

        if (_game.nutrients < 4 && targetIndex >= 19)
        {
            score = -100;
        }

        return score;
    }
}

