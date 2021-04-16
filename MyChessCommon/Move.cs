using System.Diagnostics;
using MyChess.Common;

namespace MyChessEngineCommon
{
    [DebuggerDisplay("{ToString()}")]
    public class Move
    {
        public Position Start;
        public Position End;
        public IPiece Piece;
        public ChessEngineConstants.MoveType Type;

        public Move(Position start, Position end, IPiece piece, ChessEngineConstants.MoveType type)
        {
            Start = start;
            End = end;
            Piece = piece;
            Type = type;
        }

        public Move(Position start, Position end, IPiece piece) : this(start, end, piece, ChessEngineConstants.MoveType.Normal)
        {
        }

        public Move(string startString, string endString, IPiece piece, ChessEngineConstants.MoveType type)
        {
            Start = new Position(startString);
            End = new Position(endString);
            Piece = piece;
            Type = type;
        }


        public Move(string startString, string endString, IPiece piece) : 
            this(startString, endString, piece, ChessEngineConstants.MoveType.Normal)
        {
        }


        public override string ToString()
        {
            if (Piece != null)
                return
                    $"{Piece.Color.ToString().Substring(0, 1),1} {Piece.Type.ToString(),-10} {Start.ToString()} -> {End.ToString()} T:{Type}";
            else
                return $"-           {Start.ToString()} -> {End.ToString()} T:{Type}";
        }
    }
}
