using System.Diagnostics;

namespace MyChess.Common
{
    [DebuggerDisplay(" {PieceName()}: {Start.PositionString()} -> {End.PositionString()},  T: {Type}")]
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

        public Move(Position start, Position end, IPiece piece) : this(start, end, piece, ChessConstants.MoveType.Normal)
        {
        }

        public Move(string startString, string endString, IPiece piece, ChessConstants.MoveType type)
        {
            Start = new Position(startString);
            End = new Position(endString);
            Piece = piece;
            Type = type;
        }


        public Move(string startString, string endString, IPiece piece) : 
            this(startString, endString, piece, ChessConstants.MoveType.Normal)
        {
        }


        private string PieceName()
        {
            return $"{Piece.Color.ToString().Substring(0,1),1} {Piece.Type.ToString(),-10}";
        }
    }
}
