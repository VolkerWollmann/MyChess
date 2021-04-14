using System.Collections.Generic;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    public class Bishop : Piece
    {
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            // left, down
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-i, -i);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition));
            }

            // left, up
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-i, i);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition));
            }

            // right, down
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(i, -i);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition));
            }

            // right, up
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(i, i);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition));
            }

            return moves;
        }
        public Bishop(ChessConstants.Color color) : base(color, ChessConstants.Piece.Bishop)
        {

        }
    }
}