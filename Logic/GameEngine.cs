using System;
using System.Collections.Generic;
using Backgammon.Logic.Entities.Fields;
using Backgammon.Logic.Entities;
using Backgammon.Logic.Exceptions;
using Backgammon.DataLayer;

namespace Backgammon.Logic
{
    class GameEngine
    {
        private GameBoard gameBoard;
        private DataStorage dataStorage;

        public GameEngine(DataStorage dataStorage, GameBoard gameBoard)
        {
            this.dataStorage = dataStorage;
            this.gameBoard = gameBoard;
        }

        // Start new game
        public void startGame()
        {
            // Player 1 always starts
            gameBoard.activePlayer = gameBoard.player1;
            gameBoard.diceCup.resetDiceCup();
            saveData();
        }

        // Continue saved game
        public void continueGame()
        {
            // Set player names & colors
            gameBoard.player1.name = dataStorage.getPlayerName("1");
            gameBoard.player1.color = dataStorage.getPlayerColor("1");
            gameBoard.player2.name = dataStorage.getPlayerName("2");
            gameBoard.player2.color = dataStorage.getPlayerColor("2");

            // set moves in diceCup
            int[] moves = dataStorage.getDiceCupMoves();
            gameBoard.diceCup.setMoves(moves);

            // Get and set active player
            int activePlayerId = int.Parse(dataStorage.getActviePlayer());
            if (gameBoard.player1.id.Equals(activePlayerId))
            {
                gameBoard.activePlayer = gameBoard.player1;
            }
            else
            {
                gameBoard.activePlayer = gameBoard.player2;
            }
            
            // Get checker positions and place them in fields
            int[] P1Positions = dataStorage.getCheckerPosition("1");
            int[] P2Positions = dataStorage.getCheckerPosition("2");

            initStartState(P1Positions, P2Positions);

            // Get possible moves
            gameBoard.possibleMoves = new List<PossibleMoves>();
            gameBoard.possibleMoves = gameBoard.rules.checkPossibleMoves(gameBoard.activePlayer);

        }

        public void initStartState(int[] P1Positions, int[] P2Positions)
        {
            gameBoard.initStartState(P1Positions, P2Positions);
        }

        public bool saveGameExist()
        {
            return dataStorage.savedGameExist();
        }

        public int getNumberOfCheckersInField(int index)
        {
            return gameBoard.boardFields[index].getCheckerCount();
        }

        public int getNumberOfChekersInGoalFieldP1()
        {
            return gameBoard.goalFieldP1.getCheckerCount();
        }

        public int getNumberOfChekersInGoalFieldP2()
        {
            return gameBoard.goalFieldP2.getCheckerCount();
        }

        public int getNumberOfEliminatedCheckers()
        {
            return gameBoard.eliminatedField.getCheckerCount();
        }

        public String getEliminatedCheckerColor(int index)
        {
            return gameBoard.eliminatedField.getCheckerAt(index).player.color;
        }

        public String getColor(int index)
        {
            return gameBoard.boardFields[index].getPlayerInField().color;
        }

        public String getGoalColorP1()
        {
            return gameBoard.player1.color;
        }
        public String getGoalColorP2()
        {
            return gameBoard.player2.color;
        }

        public void rollDices()
        {
            gameBoard.diceCup.resetDiceCup();
            gameBoard.diceCup.rollDices();
            gameBoard.possibleMoves = new List<PossibleMoves>();
            gameBoard.possibleMoves = gameBoard.rules.checkPossibleMoves(gameBoard.activePlayer);
            checkPlayerTurn();
            saveData();
        }

        public List<int> getDices()
        {
            return gameBoard.diceCup.getMoves();
        }

        public String getActivePlayersName()
        {
            return gameBoard.activePlayer.name;
        }

        public String getNonActivePlayerName()
        {
            if (gameBoard.player1.Equals(gameBoard.activePlayer))
            {
                return gameBoard.player2.name;
            }
            else
            {
                return gameBoard.player1.name;
            }
        }

        public String getPlayer1Name()
        {
            return gameBoard.player1.name;
        }

        public String getPlayer2Name()
        {
            return gameBoard.player2.name;
        }

        public String player1Color()
        {
            return gameBoard.player1.color;
        }

        public String player2Color()
        {
            return gameBoard.player2.color;
        }

        public GameBoard getGameBoard()
        {
            return gameBoard;
        }

        public DiceCup getDiceCup()
        {
            return gameBoard.diceCup;
        }

        public Player getPlayer1()
        {
            return gameBoard.player1;
        }

        public Player getPlayer2()
        {
            return gameBoard.player2;
        }

        public Player getActivePlayer()
        {
            return gameBoard.activePlayer;
        }

        public void makeMove(Move move)
        {
            BasicField fromField;
            BasicField toField;

            /*
             0-23   = BoardFields
             25     = EliminationField
             26     = Player2 GoalField
             27     = Player1 GoalField
            */
            // Checks if from field is eliminationField, GoalField, Boardfield
            if (move.from == 25)
            {
                fromField = gameBoard.eliminatedField;
            } 
            else if(move.from == 26 || move.from == 27)
            {
                throw new NoValidMoveException("Move is not allowed");
            }
            else
            {
                fromField = gameBoard.boardFields[move.from];
            }

            // Checks if to field is goalField, eliminationField, Boardfield
            if (move.to == 26)
            {
                toField = gameBoard.goalFieldP2;
            } else if (move.to == 27)
            {
                toField = gameBoard.goalFieldP1;
            }
            else if (move.to == 25)
            {
                throw new NoValidMoveException("Move is not allowed");
            }
            else
            {
                toField = gameBoard.boardFields[move.to];
            }

            // Moves checker
            try
            {
                gameBoard.moveChecker(gameBoard.activePlayer, fromField, toField, gameBoard.possibleMoves);
                gameBoard.possibleMoves = gameBoard.rules.checkPossibleMoves(gameBoard.activePlayer);
                checkPlayerTurn();
                saveData();
            }
            catch(NoValidMoveException info)
            {
                throw new NoValidMoveException(info.Message);
            }
            
            
        }

        // Switch active player if no possible moves
        public void checkPlayerTurn()
        {
            if (gameBoard.activePlayer.Equals(gameBoard.player1) && !anyMoreMoves())
            {
                gameBoard.activePlayer = gameBoard.player2;
            }
            else if (gameBoard.activePlayer.Equals(gameBoard.player2) && !anyMoreMoves())
            {
                gameBoard.activePlayer = gameBoard.player1;
            }
        }

        // Checks if there is any winner
        public bool hasAWinner()
        {
            if(gameBoard.goalFieldP1 !=null && gameBoard.goalFieldP2 != null) { 
                if ((gameBoard.goalFieldP1.getCheckerCount() >= 15) || (gameBoard.goalFieldP2.getCheckerCount() >= 15))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        // Returns the name of the winner
        public String returnWinner()
        {
            String winner = "";
            if (gameBoard.goalFieldP1.getCheckerCount() >= 15)
            {
                winner = getPlayer1Name();
            } else if (gameBoard.goalFieldP2.getCheckerCount() >= 15) {
                winner = getPlayer2Name();
            }

            return winner;
        }

        // Checks if there are any more possible moves
        public bool anyMoreMoves()
        {
            if ((gameBoard.diceCup.getMoves().Count <= 0) || (gameBoard.possibleMoves.Count <= 0))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // Returns all possible moves from specific position
        public List<int> getPossibleMoveFromPos(int index)
        {
            List<int> list = new List<int>();
            foreach(PossibleMoves moves in gameBoard.possibleMoves)
            {
                if (moves.from.getPosition() == index)
                {
                    list.Add(moves.to.getPosition());
                }
            }
            return list;
        }

        // Saves all data
        public void saveData()
        {
            dataStorage.saveData(gameBoard);
        }

        
    }
}
