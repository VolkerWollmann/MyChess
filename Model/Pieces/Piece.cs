using System.Collections.Generic;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    public class Piece : IEnginePiece
    {
        private readonly ChessConstants.Piece _Piece;
        private readonly ChessConstants.Color _Color;
        public ChessConstants.Piece GetPieceType()
        {
            return _Piece;
        }

        public ChessConstants.Color GetColor()
        {
            return _Color;
        }

        public virtual List<Move> GetMoves(Board board)
        {
            return new List<Move>();
        }

        public Piece(ChessConstants.Color color, ChessConstants.Piece piece)
        {
            _Color = color;
            _Piece = piece;
        }
    }
}
