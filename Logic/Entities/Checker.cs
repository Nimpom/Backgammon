namespace Backgammon.Logic
{

    class Checker
    {
        public int checkerId;
        public Player player { get; }
   
        
        public Checker(int checkerId, Player player)
        {
            this.checkerId = checkerId;
            this.player = player;
        }

    }
}
