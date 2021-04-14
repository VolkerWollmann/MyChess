using System.Collections.Generic;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    public class Piece : IEnginePiece
    {
        #region IPiece
        private readonly ChessConstants.Piece _Piece;
        private readonly ChessConstants.Color _Color;
        public ChessConstants.Piece Type => _Piece;

        public ChessConstants.Color Color => _Color;
        
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
            _Color = color;
            _Piece = piece;
        }

    }
}
