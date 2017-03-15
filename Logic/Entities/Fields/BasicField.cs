using System.Collections.Generic;
using System.Linq;

namespace Backgammon.Logic.Entities.Fields
{

    class BasicField
    {
        protected LinkedList<Checker> checkers = null;
        protected int position;

        public BasicField(LinkedList<Checker> checkers)
        {
            this.checkers = checkers;
        }
        public BasicField(LinkedList<Checker> checkers, int position)
        {
            this.checkers = checkers;
            this.position = position;
        }
        public BasicField()
        {

        }

        public int getPosition()
        {
            return this.position;
        }

        // Returns the numbers of checkers in field
        public int getCheckerCount()
        {
            return checkers.Count;
        }

        // Returns the list of checkers in the field
        public LinkedList<Checker> getCheckers()
        {
            return checkers;
        }

        //Returns the Player that owns the first checker in list.
        public Player getPlayerInField()
        {
            //TODO: Add check if any or no checkers.
            return checkers.First.Value.player;
        }

        //Removes a checker from field and returns it
        public Checker removeChecker()
        {
            Checker firstChecker = checkers.First();
            checkers.RemoveFirst();
            return firstChecker;
        }
        //Adds a checker to the list
        public void addChecker(Checker checker)
        {
            checkers.AddFirst(checker);
        }
        //Returns specific checker from the list
        // Used for printing game board in console
        public Checker getCheckerAt(int p)
        {
            return checkers.ElementAt(p);
        }

        // Returns the first checker in the list
        public Checker getFirstChecker()
        {
            return checkers.First();
        }

        public void clear()
        {
            this.checkers.Clear();
        }
       
    }

}
