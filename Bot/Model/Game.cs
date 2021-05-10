using Bot.Model;
using System.Collections.Generic;
using System.Linq;

class Game
{
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
    }

    public Action GetNextAction()
    {
        return possibleActions.First();
    }
}
