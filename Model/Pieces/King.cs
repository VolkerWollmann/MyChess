using System.Collections.Generic;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    public class King : Piece
    {
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            for (int row = -1; row <= 1; row++)
                for (int column = -1; column <= 1; column++)
                {
                    Position newPosition = this.Position.GetDeltaPosition(row, column);

                    if (this.Board.IsValidPosition(newPosition, this.Color))
                        moves.Add(new Move(this.Position, newPosition, this));
                }

            // ToDo : Castle

            return moves;
        }

        public King(ChessConstants.Color color) : base(color, ChessConstants.Piece.King)
        {

        }
    }
}
