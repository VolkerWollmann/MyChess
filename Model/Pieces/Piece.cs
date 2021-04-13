using MyChess.Common;

namespace MyChess.Model.Pieces
{
    public class Piece : IPiece
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

        public Piece(ChessConstants.Color color, ChessConstants.Piece piece)
        {
            _Color = color;
            _Piece = piece;
        }
    }
}
