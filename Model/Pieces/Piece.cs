using System.Collections.Generic;
using System.Diagnostics;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Piece : IEnginePiece
    {
        #region IPiece

        public ChessConstants.Piece Type { get; }

        public ChessConstants.Color Color { get; }

        #endregion

        #region IEnginePiece
        public virtual List<Move> GetMoves()
        {
            return new List<Move>();
        }

        public Board Board{ get; set; }

        public Position Position { get; set; }
        public virtual Piece Copy()
        {
            return Type switch
            {
                ChessConstants.Piece.Pawn => new Pawn(Color),
                ChessConstants.Piece.Bishop => new Bishop(Color),
                ChessConstants.Piece.Knight => new Knight(Color),
                ChessConstants.Piece.Queen => new Queen(Color),
                ChessConstants.Piece.Rook => new Rook(Color),
                _ => null
            };
        }

        public virtual bool ExecuteMove(Move move)
        {
            Board[move.End] = Board[move.Start];
            Board[move.Start] = null;

            return true;
        }

        #endregion

        /// <summary>
        /// Adds position to to moves.
        /// </summary>
        /// <param name="moves">moves so far</param>
        /// <param name="position"></param>
        /// <returns>returns false, if this is last move in that direction</returns>
        public bool AddPosition(List<Move> moves, Position position)
        {
            if (!this.Board.IsValidPosition(position, this.Color))
                return false;

            moves.Add(new Move(this.Position, position, this));
            if (this.Board[position] != null)
                return false;

            return true;
        }

        public Piece(ChessConstants.Color color, ChessConstants.Piece piece)
        {
            Color = color;
            Type = piece;
        }


    }
}
