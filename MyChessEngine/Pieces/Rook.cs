using System.Diagnostics;
using System.Linq;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Rook : Piece
    {
        public bool HasMoved;

        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            // left
            for (int row = this.Position.Row-1; row >= 0; row--)
            {
                Position newPosition = new Position(row, this.Position.Column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // right
            for (int row = this.Position.Row + 1; row < ChessEngineConstants.Length; row++)
            {
                Position newPosition = new Position(row, this.Position.Column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // down
            for (int column = this.Position.Column - 1; column >=0; column--)
            {
                Position newPosition = new Position(this.Position.Row, column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // up
            for (int column = this.Position.Column + 1; column < ChessEngineConstants.Length; column++)
            {
                Position newPosition = new Position(this.Position.Row, column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            return moveList;
        }

        public override bool ExecuteMove(Move move)
        {
            base.ExecuteMove(move);

            if (!HasMoved)
            {
                HasMoved = true;
                if (Board.Kings[this.Color] is King myKing)
                {
                    if (myKing.Color == Color)
                    {
                        if ((this.Position.Row == 0) || (this.Position.Column == 0))
                            myKing.KingMoves &= (MoveType.WhiteCastle | MoveType.Normal);

                        if ((this.Position.Row == 0) || (this.Position.Column == 7))
                            myKing.KingMoves &= (MoveType.WhiteCastleLong | MoveType.Normal);

                        if ((this.Position.Row == 7) || (this.Position.Column == 0))
                            myKing.KingMoves &= (MoveType.BlackCastle | MoveType.Normal);

                        if ((this.Position.Row == 7) || (this.Position.Column == 7))
                            myKing.KingMoves &= (MoveType.WhiteCastleLong | MoveType.Normal);
                    }
                }
            }

            return true;
        }

        public Rook(Color color, bool hasMoved) : base(color, PieceType.Rook)
        {
            HasMoved = hasMoved;
        }

        public Rook(Color color) : this(color, false)
        {

        }
    }
}
