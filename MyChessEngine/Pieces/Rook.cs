using System.Diagnostics;
using MyChessEngineBase;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Rook : Piece
    {
        public bool HasMoved;

        public override MoveList GetThreatenMoveList()
        {
            return GetMoveList();
        }

        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            // left
            for (int row = Position.Row-1; row >= 0; row--)
            {
                Position newPosition = new Position(row, Position.Column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // right
            for (int row = Position.Row + 1; row < ChessEngineConstants.Length; row++)
            {
                Position newPosition = new Position(row, Position.Column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // down
            for (int column = Position.Column - 1; column >=0; column--)
            {
                Position newPosition = new Position(Position.Row, column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // up
            for (int column = Position.Column + 1; column < ChessEngineConstants.Length; column++)
            {
                Position newPosition = new Position(Position.Row, column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            return moveList;
        }

        static readonly Position WhiteKingRookField = new Position("H1");
        static readonly Position WhiteQueenRookField = new Position("A1");
        static readonly Position BlackKingRookField = new Position("H8");
        static readonly Position BlackQueenRookField = new Position("A8");
        public override bool ExecuteMove(Move move)
        {
            base.ExecuteMove(move);

            if (!HasMoved)
            {
                HasMoved = true;
                if (Board.Kings[Color] is { } myKing)
                {
                    if ((myKing.Color == Color.White) )
                    {
                        if (myKing.KingMoves != MoveType.Normal)
                        {
                            if (Position.AreEqual(WhiteQueenRookField))
                                myKing.KingMoves &= (MoveType.WhiteCastle | MoveType.Normal);

                            if (Position.AreEqual(WhiteKingRookField))
                                myKing.KingMoves &= (MoveType.WhiteCastleLong | MoveType.Normal);
                        }
                    }
                    else
                    {
                        if (myKing.KingMoves != MoveType.Normal)
                        {
                            if (Position.AreEqual(BlackQueenRookField))
                                myKing.KingMoves &= (MoveType.BlackCastle | MoveType.Normal);

                            if (Position.AreEqual(BlackKingRookField))
                                myKing.KingMoves &= (MoveType.WhiteCastleLong | MoveType.Normal);
                        }
                    }
                }
            }

            return true;
        }

        public Rook(Color color, Position position, bool hasMoved) : base(color, PieceType.Rook, position)
        {
            HasMoved = hasMoved;
        }

        public Rook(Color color, Position position) : this(color, position, false)
        {

        }

        public Rook(Color color, string position, bool hasMoved) : base(color, PieceType.Rook, new Position(position))
        {
            HasMoved = hasMoved;
        }

        public Rook(Color color, string position) : this(color, new Position(position), false)
        {

        }
    }
}
