using MyChess.Common;

namespace MyChess.Model.Pieces
{
    public class PieceFactory
    {
        public static Piece Copy(Piece piece)
        {
            return piece.Type switch
            {
                ChessConstants.Piece.Pawn => new Pawn(piece.Color),
                ChessConstants.Piece.Bishop => new Bishop(piece.Color),
                ChessConstants.Piece.King => new King(piece.Color),
                ChessConstants.Piece.Knight => new Knight(piece.Color),
                ChessConstants.Piece.Queen => new Queen(piece.Color),
                ChessConstants.Piece.Rook => new Rook(piece.Color),
                _ => null
            };
        }
    }
}
