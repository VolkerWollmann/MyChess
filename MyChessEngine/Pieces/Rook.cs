using System.Diagnostics;
using MyChessEngineBase;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Color = {Color} Position={Position}")]
    public class Rook : Piece
    {
        public override MoveList GetThreatenMoveList()
        {
            return GetMoveList(true);
        }

        public override MoveList GetMoveList()
        {
            return GetMoveList(false);
        }
        private  MoveList GetMoveList(bool threat)
        {
            MoveList moveList = new MoveList();

            int localColumn = Position.Column;
            int localRow = Position.Row;

            // left
            for (int row = localRow - 1; row >= 0; row--)
            {
                Position newPosition = new Position(localColumn, row);
                if (!AddPosition(moveList, newPosition, threat))
                    break;
            }

            // right
            for (int row = localRow + 1; row < ChessEngineConstants.Length; row++)
            {
                Position newPosition = new Position(localColumn, row);
                if (!AddPosition(moveList, newPosition, threat))
                    break;
            }

            // down
            for (int column = localColumn - 1; column >=0; column--)
            {
                Position newPosition = new Position(column,localRow);
                if (!AddPosition(moveList, newPosition, threat))
                    break;
            }

            // up
            for (int column = localColumn + 1; column < ChessEngineConstants.Length; column++)
            {
                Position newPosition = new Position(column, localRow);
                if (!AddPosition(moveList, newPosition, threat))
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
	        if (!IsMoved())
            {
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

			base.ExecuteMove(move);

			return true;
        }

        public Rook(Color color, Position position, int lastPly=-1, int promotionPly=-1) : base(color, PieceType.Rook, position, lastPly, promotionPly)
        {
           
        }

        public Rook(Color color, string position, int lastPly=-1, int promotionPly=-1) : base(color, PieceType.Rook, new Position(position), lastPly, promotionPly)
        {
        }

    }
}
