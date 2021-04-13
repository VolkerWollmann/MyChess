using System.Windows.Controls;
using MyChess.Common;

namespace MyChess.Controls.Pieces
{
    public class PieceUserControlFactory
    {
        public static UserControl CreatePieceUserControl(IPiece piece)
        {
            return piece.GetPieceType() switch
            {
                ChessConstants.Piece.Pawn => new PawnUserControl(piece),
                ChessConstants.Piece.Bishop => new BishopUserControl(piece),
                ChessConstants.Piece.Rook => new RookUserControl(piece),
                ChessConstants.Piece.Queen => new QueenUserControl(piece),
                ChessConstants.Piece.King => new KingUserControl(piece),
                ChessConstants.Piece.Knight => new KnightUserControl(piece),
                _ => null
            };
        }
    }
}
