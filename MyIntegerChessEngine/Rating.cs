using System.Diagnostics;

namespace MyIntegerChessEngine
{
    public enum GameState
    {
        Normal,
        WhiteLoss,   // white king is off the board or checkmated
        BlackLoss,   // black king is off the board or checkmated
        Remis        // side to move has no legal move without being checked (stalemate)
    }

    [DebuggerDisplay("{ToString()}")]
    public class Rating
    {
        public int Value;
        public GameState State;

        public Rating(int value, GameState state)
        {
            Value = value;
            State = state;
        }

        public override string ToString()
        {
            return $"State:{State} Value:{Value}";
        }
    }
}
