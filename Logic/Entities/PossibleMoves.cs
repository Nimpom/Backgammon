using Backgammon.Logic.Entities.Fields;

namespace Backgammon.Logic.Entities
{

    class PossibleMoves
    {
        public BasicField from { get; }
        public BasicField to { get; }
        public int move { get; }

        public PossibleMoves(BasicField from, BasicField to, int move)
        {
            this.from = from;
            this.to = to;
            this.move = move;
        }
    }
}
