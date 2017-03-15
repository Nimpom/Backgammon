using System.Collections.Generic;

namespace Backgammon.Logic.Entities.Fields
{
    class BoardField : BasicField
    {
        private int position;

        public BoardField(LinkedList<Checker> checkers, int position)
            : base(checkers, position)
        {

        }
    }
}
