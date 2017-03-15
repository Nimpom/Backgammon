using System;
using System.Collections.Generic;
using Backgammon.Logic.Entities.Fields;
using Backgammon.Logic.Entities;
using Backgammon.Logic.Exceptions;

namespace Backgammon.Logic
{

    class GameBoard
    {
        public Player player1 { get; set; }
        public Player player2 { get; set; }
        public BoardField[] boardFields { get; }
        public DiceCup diceCup { get; set; }
        public EliminatedField eliminatedField { get; }
        public GoalField goalFieldP1 { get; }
        public GoalField goalFieldP2 { get; }
        public List<PossibleMoves> possibleMoves { get; set; }
        public Player activePlayer { get; set; }
        public Checker[] P1Checkers { get; }
        public Checker[] P2Checkers { get; }
        public Rules rules { get; }


        public GameBoard(BoardField[] boardFields, EliminatedField eliminatedField, GoalField goalFieldP1, GoalField goalFieldP2, Checker[] P1Checkers, Checker[] P2Checkers, DiceCup diceCup, Rules rules, Player player1, Player player2)
        {
            this.boardFields = boardFields;
            this.eliminatedField = eliminatedField;
            this.goalFieldP1 = goalFieldP1;
            this.goalFieldP2 = goalFieldP2;
            this.player1 = player1;
            this.player2 = player2;
            this.diceCup = diceCup;
            this.rules = rules;
            this.P1Checkers = P1Checkers;
            this.P2Checkers = P2Checkers;
        }

        // Moves a checker from one field to another field
        public void moveChecker(Player activePlayer, BasicField fromField, BasicField toField, List<PossibleMoves> possibleMoves)
        {
            try { 
                if (rules.validateMove(fromField, toField,possibleMoves))
                {
                    // If opponent has ONE checker in the field
                    if (toField.getCheckerCount() == 1 && !toField.getPlayerInField().Equals(activePlayer))
                    {
                        // Move opponents checker to eliminationField
                        Checker opponentChecker = toField.removeChecker();
                        eliminatedField.addChecker(opponentChecker);
                    }

                    // Moves active players checker
                    Checker checker = null;
                    if (eliminatedField.hasCheckerFrom(activePlayer))
                    {
                        checker = eliminatedField.removeChecker(activePlayer);
                    }
                    else
                    {
                        checker = fromField.removeChecker();
                    }
                    toField.addChecker(checker);
                    // Remove made move from list of moves
                    foreach (PossibleMoves move in possibleMoves)
                    {
                        if(move.from.Equals(fromField) && move.to.Equals(toField))
                        {
                            diceCup.removeMove(move.move);
                            break;
                        }
                    }
                }
            }
            catch (NoValidMoveException info)
            {
                throw new NoValidMoveException(info.Message);
            }
        }

        // Places the checkers in the board fields
        public void initStartState(int[] P1Positions, int[] P2Positions)
        {

            // Resets all fields
            for (int i = 0; i < 24; i++)
            {
                boardFields[i].clear();
            }
            eliminatedField.clear();
            goalFieldP1.clear();
            goalFieldP2.clear();

            // Array counters
            int p1Place = 0;
            int p2Place = 0;

            for (int i = 0; i < 24; i++)
            {
                // Player 1
                for(int j = 0; j < P1Positions[i]; j++)
                {
                    boardFields[i].addChecker(P1Checkers[p1Place]);
                    p1Place++;
                }

                // Player 2
                for (int j = 0; j < P2Positions[i]; j++)
                {
                    boardFields[i].addChecker(P2Checkers[p2Place]);
                    p2Place++;
                }
            }

            // Elimination field
            for (int j = 0; j < P1Positions[25]; j++)
            {
                eliminatedField.addChecker(P1Checkers[p1Place]);
                p1Place++;
            }
            for (int j = 0; j < P2Positions[25]; j++)
            {
                eliminatedField.addChecker(P2Checkers[p2Place]);
                p2Place++;
            }

            // Goalfield P1
            for (int j = 0; j < P1Positions[27]; j++)
            {
                goalFieldP1.addChecker(P1Checkers[p1Place]);
                p1Place++;
            }

            // Goalfield P2
            for (int j = 0; j < P2Positions[26]; j++)
            {
                goalFieldP2.addChecker(P2Checkers[p2Place]);
                p2Place++;
            }

        }

    }
}
