using System.Collections.Generic;
using System.Diagnostics;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Bishop : Piece
    {
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            // left, down
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-i, -i);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // left, up
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-i, i);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // right, down
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(i, -i);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // right, up
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(i, i);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            return moves;
        }
        public Bishop(ChessConstants.Color color) : base(color, ChessConstants.Piece.Bishop)
        {

        }
    }
}