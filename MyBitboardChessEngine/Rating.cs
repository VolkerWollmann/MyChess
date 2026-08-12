using System.Diagnostics;

namespace MyBitboardChessEngine
{
    [DebuggerDisplay("{ToString()}")]
    public sealed class Rating
    {
        /// White-positive score: material in centipawns plus a sub-pawn
        /// mobility term; mate scores are +-(Mate - pliesToMate).
        public int Value;

        public GameState State;

        /// Principal variation, root move first ("E2-E4;E7-E5;...").
        public string MoveList = "";

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

    /// A calculated best move with its rating - the search result surface,
    /// analogous to the Move+Rating pairs of the other engines.
    [DebuggerDisplay("{ToString()}")]
    public sealed class EngineMove
    {
        public readonly Move Move;
        public readonly Rating Rating;

        public EngineMove(Move move, Rating rating)
        {
            Move = move;
            Rating = rating;
        }

        public string Start => Constants.NameOf(Move.From);
        public string End => Constants.NameOf(Move.To);

        public override string ToString()
        {
            return $"{Move} {Rating}";
        }
    }
}
