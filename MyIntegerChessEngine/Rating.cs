using System.Diagnostics;

namespace MyIntegerChessEngine
{
    public enum GameState
    {
        Normal,
        WhiteLoss,   // white king is off the board
        BlackLoss    // black king is off the board
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
