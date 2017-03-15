using System.Collections.Generic;
using System.Linq;
using Backgammon.Logic.Entities.Fields;
using Backgammon.Logic.Exceptions;
using Backgammon.Logic.Entities;

namespace Backgammon.Logic
{

    class Rules
    {
        EliminatedField eliminatedField;
        BasicField goalFieldP1, goalFieldP2;
        BoardField[] boardFields;
        DiceCup diceCup;
        Player player1, player2;

        public Rules(BoardField[] boardFields, EliminatedField eliminatedField, BasicField goalFieldP1, BasicField goalFieldP2, DiceCup diceCup, Player player1, Player player2)
        {
            this.eliminatedField = eliminatedField;
            this.goalFieldP1 = goalFieldP1;
            this.goalFieldP2 = goalFieldP2;
            this.diceCup = diceCup;
            this.player1 = player1;
            this.player2 = player2;
            this.boardFields = boardFields;
        }

        // Validates move against list of valid moves
        public bool validateMove(BasicField fromField, BasicField toField, List<PossibleMoves> possibleMoves)
        {
            bool returnValue = false;

            foreach (PossibleMoves move in possibleMoves)
            {
                if (move.from.Equals(fromField) && move.to.Equals(toField))
                {
                    returnValue = true;
                    break;
                }
            }
            if (!returnValue)
            {
                throw new NoValidMoveException("Not a valid move");
            }

            return returnValue;

        }

        //Checks if a move is valid
        public bool isValidMove(BasicField fromField, BasicField toField, Player activePlayer)
        {
            // When moving out from eliminated field
            if (fromField.Equals(eliminatedField))
            {
                if (!okToMoveToField(toField, activePlayer))
                {
                    //throw new NoValidMoveException("You can not move a checker to this field");
                    return false;
                }
                else
                {
                    // Depending on moving direction
                    int d;
                    if (activePlayer.Equals(player1))
                    {
                        d = -1;
                    }
                    else
                    {
                        d = 24;
                    }
                    int to = toField.getPosition();
                    if (!moveMatchesDice(diceCup, d, to))
                    {
                        return false;
                    }
                    return true;
                }
            }

            // When Player 1 tries to move to goal field
            if (toField.Equals(goalFieldP1) && activePlayer.Equals(player1))
            {
                // If fromField does not contain checkers from active player
                if (!okToMoveFromField(fromField, activePlayer))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            // When Player 2 tries to move to goal field
            if (toField.Equals(goalFieldP2) && activePlayer.Equals(player2))
            {
                // If fromField does not contain checkers from active player
                if (!okToMoveFromField(fromField, activePlayer))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            // If move does not match any dice
            if (!moveMatchesDice(diceCup, fromField.getPosition(), toField.getPosition()))
            {
                return false;
            }

            //If it't not ok to move from fromfield
            if (!okToMoveFromField(fromField, activePlayer))
            {
                return false;
            }

            // If it't not ok to move to tofield
            if (!okToMoveToField(toField, activePlayer))
            {
                return false;
            }

            // Moving in right direction player 1
            if(activePlayer.Equals(player1) && 
                !fromField.Equals(eliminatedField) && 
                !toField.Equals(goalFieldP1) && 
                fromField.getPosition() >= toField.getPosition())
            {
                return false;
            }

            // Moving in right direction player 2
            if (activePlayer.Equals(player2) &&
                !fromField.Equals(eliminatedField) &&
                !toField.Equals(goalFieldP2) &&
                fromField.getPosition() <= toField.getPosition())
            {
                return false;
            }

            // If player has checkers in elimination field but tries to move another checker
            if (eliminatedField.hasCheckerFrom(activePlayer) && !fromField.Equals(eliminatedField))
            {
                return false;
            }

            return true;
        }

        // Checks if target field is valid
        public bool okToMoveToField(BasicField field, Player activePlayer)
        {
            // If there is more then one checker in to-field
            if (field.getCheckerCount() > 1)
            {
                // If the checker is not owned by active player
                if (!field.getPlayerInField().Equals(activePlayer))
                {
                    return false;
                }
            }
            return true;
        }

        // Checks if source field is valid
        public bool okToMoveFromField(BasicField field, Player activePlayer)
        {
            // There must be atleast one checker to be moved and it has to be owned by active player
            if (field.getCheckerCount() > 0 && field.getPlayerInField().Equals(activePlayer))
            {
                return true;
            }
            return false;
        }
        
        // Checks that move matches any rolled dice
        public bool moveMatchesDice(DiceCup diceCup, int from, int to)
        {
            int moveDistance = 0;

            moveDistance = to - from;

            // If moveDistance < 0 reverse
            if (moveDistance < 0)
            {
                moveDistance = -moveDistance;
            }

            // If number of steps do not matches a dice
            if (!diceCup.getMoves().Contains<int>(moveDistance))
            {
                return false;
            }

            return true;
        }

        // Checks if all players checker is on homestretch
        public bool allCheckerOnHomeStretch(Player player, List<BasicField> fieldsToCheck)
        {
            int numberOfCheckers = 0;
            foreach(BasicField field in fieldsToCheck)
            {
                // If checkers in field is owned by player
                if ((field.getCheckerCount() > 0) && (field.getPlayerInField().Equals(player)))
                {
                    numberOfCheckers = numberOfCheckers + field.getCheckerCount();
                }
            }
            // If all checkers is in area its ok to move to goal
            if(numberOfCheckers >= 15)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Checks all possible moves
        public List<PossibleMoves> checkPossibleMoves(Player player)
        {
            Player activePlayer = player;
            List<PossibleMoves> possibleMoves = new List<PossibleMoves>();

            foreach (int move in diceCup.getMoves())
            {
                // If player got eliminated checkers
                if (eliminatedField.hasCheckerFrom(activePlayer))
                {
                    // Set to player 1
                    int to = move - 1;
                    // If player 2
                    if (activePlayer.Equals(player2))
                    {
                        to = 24 - move;
                    }
                    try
                    {
                        if (isValidMove(eliminatedField, boardFields[to], activePlayer))
                        {
                            possibleMoves.Add(new PossibleMoves(eliminatedField, boardFields[to], move));
                        }
                    }
                    catch
                    {

                    }
                }
                else
                {
                    // Choose moving direction depending on player
                    // Player 1
                    if (activePlayer.Equals(player1))
                    {
                        // Loop through all board fields to check possible moves
                        for (int i = 0; i < 24; i++)
                        {
                            int to = i + move;
                            if (to <= 23)
                            {
                                try
                                {
                                    // Checks if move is a valid move
                                    if (isValidMove(boardFields[i], boardFields[to], activePlayer))
                                    {
                                        possibleMoves.Add(new PossibleMoves(boardFields[i], boardFields[to], move));
                                    }
                                }
                                catch
                                {
                                    // Do nothing as this only creates a list with possible moves
                                }
                            }
                        }

                        // Check if possible to move to goal
                        //Create a list with fields to check
                        List<BasicField> fieldsToCheck = new List<BasicField>();
                        for (int i = 18; i < 24; i++)
                        {
                            fieldsToCheck.Add(boardFields[i]);
                        }
                        fieldsToCheck.Add(goalFieldP1);
                        if (allCheckerOnHomeStretch(activePlayer, fieldsToCheck))
                        {
                            for (int i = 18; i < 24; i++)
                            {
                                // If field corrensponding with move has checker owned by active player
                                if ((boardFields[24 - move].getCheckerCount() > 0) && (boardFields[24 - move].getPlayerInField().Equals(activePlayer)))
                                {
                                    // Possible to move checker to goal
                                    possibleMoves.Add(new PossibleMoves(boardFields[24 - move], goalFieldP1, move));
                                    break;
                                }
                                // If there is any checker in field higher than move value. Not possible to move to goal
                                if (((24 - boardFields[i].getPosition()) > move) && (boardFields[i].getCheckerCount() > 0) && (boardFields[i].getPlayerInField().Equals(activePlayer)))
                                {
                                    // Not possible to use this move to place checker in goal
                                    break;
                                }

                                // If checker on field equals to or lower than move value. Possible to move to goal
                                if (((24 - boardFields[i].getPosition()) <= move) && (boardFields[i].getCheckerCount() > 0) && (boardFields[i].getPlayerInField().Equals(activePlayer)))
                                {
                                    // Possible to move checker to goal
                                    possibleMoves.Add(new PossibleMoves(boardFields[i], goalFieldP1, move));
                                    break;
                                }

                            }
                        }

                    }
                    // Player 2
                    else
                    {
                        for (int i = 23; i >= 0; i--)
                        {
                            int to = i - move;
                            if (to >= 0)
                            {
                                try
                                {
                                    if (isValidMove(boardFields[i], boardFields[to], activePlayer))
                                    {
                                        possibleMoves.Add(new PossibleMoves(boardFields[i], boardFields[to], move));
                                    }
                                }
                                catch
                                {
                                    // Do nothing as this only creates a list with possible moves
                                }
                            }
                        }

                        // Check if possible to move to goal
                        //Create a list with fields to check
                        List<BasicField> fieldsToCheck = new List<BasicField>();
                        for (int i = 5; i >= 0; i--)
                        {
                            fieldsToCheck.Add(boardFields[i]);
                        }
                        fieldsToCheck.Add(goalFieldP2);
                        if (allCheckerOnHomeStretch(activePlayer, fieldsToCheck))
                        {
                            for (int i = 5; i >= 0; i--)
                            {
                                // If field corrensponding with move has checker owned by active player
                                if ((boardFields[move - 1].getCheckerCount() > 0) && (boardFields[move - 1].getPlayerInField().Equals(activePlayer)))
                                {
                                    // Possible to move checker to goal
                                    possibleMoves.Add(new PossibleMoves(boardFields[move - 1], goalFieldP2, move));
                                    break;
                                }
                                // If there is any checker in field higher than move value. Not possible to move to goal
                                if (((boardFields[i].getPosition()) > (move - 1)) && (boardFields[i].getCheckerCount() > 0) && (boardFields[i].getPlayerInField().Equals(activePlayer)))
                                {
                                    // Not possible to use this move to place checker in goal
                                    break;
                                }

                                // If checker on field equals to or lower than move value. Possible to move to goal
                                if (((boardFields[i].getPosition()) <= (move - 1)) && (boardFields[i].getCheckerCount() > 0) && (boardFields[i].getPlayerInField().Equals(activePlayer)))
                                {
                                    // Possible to move checker to goal
                                    possibleMoves.Add(new PossibleMoves(boardFields[i], goalFieldP2, move));
                                    break;
                                }

                            }
                        }
                    }
                }
            }
            return possibleMoves;
        }

    }
}
