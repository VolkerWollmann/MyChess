using MyChess.Common;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyChess.Model.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Queen : Piece
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
                moves.Add(new Move(this.Position, newPosition, this));
            }

            // right
            for (int row = 1; row <= ChessConstants.Length; row++)
            {
                Position newPosition = this.Position.GetDeltaPosition(row, 0);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition, this));
            }

            // down
            for (int column = 1; column <= ChessConstants.Length; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(0, -column);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition, this));
            }

            // up
            for (int column = 1; column <= ChessConstants.Length; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(0, column);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition, this));
            }

            // left, down
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-i, -i);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition, this));
            }

            // left, up
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-i, i);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition, this));
            }

            // right, down
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(i, -i);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition, this));
            }

            // right, up
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(i, i);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition, this));
            }

            return moves;
        }

        public Queen(ChessConstants.Color color) : base(color, ChessConstants.Piece.Queen)
        {

        }
    }
}