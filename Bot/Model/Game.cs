using System.Collections.Generic;

namespace Bot.Model
{
    interface IStrategy
    {
        Action SelectAction(Game game);
    }

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
            _strategy = new RandomActionStrategy();
        }

        public Action GetNextAction()
        {
            return _strategy.SelectAction(this);
        }
    }
}