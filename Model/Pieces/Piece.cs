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

            return true;
        }

        #endregion

        public Piece(ChessConstants.Color color, ChessConstants.Piece piece)
        {
            Color = color;
            Type = piece;
        }


    }
}
