using System.Collections.Generic;
using System.Windows.Controls;
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

                    if (newPosition != null)
                    {
                        if ((this.Board[newPosition] == null) || this.Board[newPosition].Color != this.Color)
                            moves.Add(new Move(this.Position, newPosition));
                    }
                }

            return moves;
        }

        public King(ChessConstants.Color color) : base(color, ChessConstants.Piece.King)
        {

        }
    }
}
