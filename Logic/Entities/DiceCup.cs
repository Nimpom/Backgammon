using System;
using System.Collections.Generic;
using System.Linq;

namespace Backgammon.Logic
{

    class DiceCup
    {
        private Dice D1;
        private Dice D2;
        static Random random = new Random();
        private List<int>  moves = new List<int>();

        public DiceCup(Dice D1, Dice D2)
        {
            this.D1 = D1;
            this.D2 = D2;
        }

        public DiceCup(Dice D1, Dice D2, int[] moves)
        {
            this.D1 = D1;
            this.D2 = D2;
            if (moves != null){
                foreach (int m in moves)
                {
                    this.moves.Add(m);
                }
            }
        }

        // Rolls the dices
        public void rollDices()
        {
            moves.Clear();
            D1.diceValue = random.Next(1, 7);
            D2.diceValue = random.Next(1, 7);

            if (D1.diceValue == D2.diceValue)
            {
                
                for (int i = 0; i < 4; i++)
                {
                    moves.Add(D1.diceValue);
                }
            }
            else
            {
                moves.Add(D1.diceValue);
                moves.Add(D2.diceValue);
            }
        }

        //Returns a list of moves
        public List<int> getMoves()
        {
            return moves;
        }

        public void setMoves(int[] moves)
        {
            if (moves != null)
            {
                foreach (int m in moves)
                {
                    this.moves.Add(m);
                }
            }
        }

        //Deletes a move from list of possible moves
        public void removeMove(int move)
        {
            for (int i = 0; i < moves.Count(); i++)
            {
                if (moves[i] == move)
                {
                    moves.RemoveAt(i);
                    break;
                }
            }
        }

        // Resets diceCup
        public void resetDiceCup()
        {
            moves.Clear();
        }
    }
}
