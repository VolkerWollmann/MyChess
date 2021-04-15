namespace MyChess.Common
{
    public class Move
    {
        public Position Start;
        public Position End;
        public IPiece Piece;
        public ChessConstants.MoveType Type;

        public Move(Position start, Position end, IPiece piece, ChessConstants.MoveType type)
        {
            Start = start;
            End = end;
            Piece = piece;
            Type = type;
        }

        public Move(Position start, Position end, IPiece piece):this(start, end, piece, ChessConstants.MoveType.Normal)
        {
        }

    }
}
