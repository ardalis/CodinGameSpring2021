﻿using System;
using System.Collections.Generic;

namespace Bot.Model
{
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
            _strategy = new CompleteGrowSeedActionStrategy();
        }

        public Action GetNextAction()
        {
            PrintPossibleActions();
            return _strategy.SelectAction(this);
        }

        private void PrintPossibleActions()
        {
            foreach (var action in possibleActions)
            {
                Console.Error.WriteLine(action);
            }
        }
    }
}