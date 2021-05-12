using System;
using System.Collections.Generic;
using System.Linq;

class Game 
{
    private readonly IStrategy _strategy;
    public int day;
    public int nutrients;
    public List<Cell> board;
    public List<Action> possibleActions;
    public List<Tree> trees;
    public int mySun, opponentSun;
    public int myScore, opponentScore;
    public bool opponentIsWaiting;


    public Game()
    {
        board = new List<Cell>();
        possibleActions = new List<Action>();
        trees = new List<Tree>();

        // set strategy here
        //_strategy = new LastActionStrategy();
        //_strategy = new RandomActionStrategy();
        //_strategy = new CompleteGrowSeedActionStrategy();
        //_strategy = new CompleteGrowSeedWithLimitsActionStrategy();
        _strategy = new ConquerTheEdgeStrategy(this);
    }

    public Action GetNextAction()
    {
        Console.Error.WriteLine($"Current Day: {day}");
        Console.Error.WriteLine($"Nutrients : {nutrients}");
        PrintPossibleActions();
        return _strategy.SelectAction();
    }

    private void PrintPossibleActions()
    {
        foreach (var action in possibleActions)
        {
            Console.Error.WriteLine(action);
        }
    }

    public Tree GetTreeFromLocation(int cellIndex)
    {
        return trees.FirstOrDefault(t => t.cellIndex == cellIndex);
    }
}
