using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Rook : Piece
    {
        private bool HasMoved;

        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            // left
            for (int row = 1; row <= ChessConstants.Length; row++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-row, 0);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // right
            for (int row = 1; row <= ChessConstants.Length; row++)
            {
                Position newPosition = this.Position.GetDeltaPosition(row, 0);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // down
            for (int column = 1; column <= ChessConstants.Length; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(0, -column);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // up
            for (int column = 1; column <= ChessConstants.Length; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(0, column);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            return moves;
        }

        public override bool ExecuteMove(Move move)
        {
            Board[move.End] = this;
            Board[move.Start] = null;

            if (!HasMoved)
            {
                HasMoved = true;
                if (Board.GetAllPieces(Color).First(piece => (piece.Type == ChessConstants.Piece.King)) is King myKing)
                {
                    if ((this.Position.Row == 0) || (this.Position.Column == 0))
                        myKing.Rochades.Remove(ChessConstants.MoveType.WhiteCastleLong);

                    if ((this.Position.Row == 0) || (this.Position.Column == 7))
                        myKing.Rochades.Remove(ChessConstants.MoveType.WhiteCastle);

                    if ((this.Position.Row == 0) || (this.Position.Column == 0))
                        myKing.Rochades.Remove(ChessConstants.MoveType.WhiteCastleLong);

                    if ((this.Position.Row == 0) || (this.Position.Column == 7))
                        myKing.Rochades.Remove(ChessConstants.MoveType.WhiteCastle);
                }
            }

            return true;
        }

        public Rook(ChessConstants.Color color) : base(color, ChessConstants.Piece.Rook)
        {

        }
    }
}
