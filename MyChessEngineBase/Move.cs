using System.Collections.Generic;
using System.Diagnostics;
using MyChessEngineBase.Rating;

namespace MyChessEngineBase
{
    [DebuggerDisplay("{ToString()} Rating:{Rating}")]
    public class Move 
    {
        public Position Start;
        public Position End;
        public IPiece Piece;
        public IPiece PieceBefore;
        public MoveType Type;
        public int PlyBefore;


		public Position[] AffectedPositionAfter = [new Position(), new Position()];
        public IPiece[] AffectedPieceAfter = [null, null];

        public Position[] AffectedPositionBefore = [new Position(), new Position()];
        public IPiece[] AffectedPieceBefore = [null, null];

        public BoardRating Rating { get; set; }

        public Move(Position start, Position end, IPiece piece, MoveType type) : this(start, end, piece, type, null, new Position())
		{

        }

        public Move(Position start, Position end, IPiece piece, MoveType type, IPiece affectedPiece,
	        Position affectedPosition)
        {
	        Start = start;
	        End = end;
	        Piece = piece;
	        Type = type;
            AffectedPieceAfter[0] = affectedPiece;
            AffectedPositionAfter[0] = affectedPosition;
		}

        public Move(Position start, Position end, IPiece piece) : this(start, end, piece, MoveType.Normal)
        {
        }

        public Move(string startString, string endString, IPiece piece, MoveType type)
        {
            Start = new Position(startString);
            End = new Position(endString);
            Piece = piece;
            Type = type;
        }

        private Move(BoardRating rating)
        {
            Rating = rating;
        }
        public static Move CreateNoMove(BoardRating rating)
        {
            return new Move(rating);
        }


        public Move(string startString, string endString, IPiece piece) : 
            this(startString, endString, piece, MoveType.Normal)
        {
        }

        public override string ToString()
        {
            string s="";
            if (Piece != null)
                s += $"{Piece.Color.ToString().Substring(0, 1),1} {Piece.Type,-10} {Start} -> {End} MT:{Type} ";
            else
                s += $"-           {Start} -> {End} MT:{Type} ";

            return s;
        }

        public string ShortString()
        {
	        return $"{Start}-{End};";
        }
	}

    public class MoveComparer : IComparer<Move>
    {
        public int Compare(Move x, Move y) => _Comparer.Compare(x?.Rating, y?.Rating);

        private readonly IBoardRatingComparer _Comparer;
        public MoveComparer(Color color)
        {
            _Comparer = BoardRatingComparerFactory.GetComparer(color);
        }

    }
}
