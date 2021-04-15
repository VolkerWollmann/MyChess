using System.Diagnostics;

namespace MyChess.Common
{
    [DebuggerDisplay("R:{Start.Row} C:{Start.Column} -> R:{End.Row} C:{End.Column}   P:{PieceName()} T: {Type}")]
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


        private string PieceName()
        {
            return $"{Piece.Color.ToString().Substring(0,1),1} {Piece.Type.ToString(),-10}";
        }
    }
}
