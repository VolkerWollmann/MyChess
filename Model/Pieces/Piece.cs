using System.Collections.Generic;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
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

        #endregion



        public Piece(ChessConstants.Color color, ChessConstants.Piece piece)
        {
            Color = color;
            Type = piece;
        }

    }
}
