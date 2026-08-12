using System.Diagnostics;

namespace MyTranspositionChessEngine
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

        /// Strongest line found by the search, root move first ("E2-E4;E7-E5;...").
        public string MoveList = "";

        public Rating(int value, GameState state)
        {
            Value = value;
            State = state;
        }

        /// Prepends a move; the search calls this while unwinding,
        /// so MoveList ends up in root-first order.
        public void AddMove(string move)
        {
            MoveList = string.IsNullOrEmpty(MoveList) ? move : move + ";" + MoveList;
        }

        public override string ToString()
        {
            return $"State:{State} Value:{Value}";
        }
    }
}
