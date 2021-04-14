using System.Collections.Generic;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    public class Rook : Piece
    {
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            // left
            for (int row = 1; row <= ChessConstants.Length; row++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-row, 0);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition));
            }

            // right
            for (int row = 1; row <= ChessConstants.Length; row++)
            {
                Position newPosition = this.Position.GetDeltaPosition(row, 0);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition));
            }

            // down
            for (int column = 1; column <= ChessConstants.Length; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(0, -column);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition));
            }

            // up
            for (int column = 1; column <= ChessConstants.Length; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(0, column);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition));
            }

            return moves;
        }
        public Rook(ChessConstants.Color color) : base(color, ChessConstants.Piece.Rook)
        {

        }
    }
}
