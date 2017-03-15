using System;
using System.Collections.Generic;

namespace Backgammon.Logic.Entities.Fields
{
    class EliminatedField : BasicField
    {
        public EliminatedField()
        {
            this.checkers = new LinkedList<Checker>();
            this.position = 25;
        }

        // Checks if player has any checker in the elimination field
        public bool hasCheckerFrom(Player player)
        {
            foreach(Checker checker in checkers)
            {
                if (checker.player.Equals(player))
                {
                    return true;
                }
            }
            return false;
        }

        public Checker removeChecker(Player player)
        {
            Checker tempChecker = null;
            foreach (Checker checker in checkers)
            {
                if (checker.player.Equals(player))
                {
                    tempChecker = checker;
                    checkers.Remove(checker);
                    break;
                }
            }
            return tempChecker;
        }

        

        // Prints elimination field in console
        public void printField()
        {
            foreach (Checker checker in checkers)
            {
                //Console.ForegroundColor = checker.getPlayer().getColor();
                Console.Write("O");
                Console.ResetColor();
            }
        }
    }
}
