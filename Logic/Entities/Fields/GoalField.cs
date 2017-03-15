using System.Collections.Generic;

namespace Backgammon.Logic.Entities.Fields
{
    class GoalField : BasicField
    {
        private Player player;

        public GoalField(Player player, int pos)
        {
            this.player = player;
            this.checkers = new LinkedList<Checker>();
            this.position = pos;
        }

        public Player getOwner()
        {
            return player;
        }


    }
}
