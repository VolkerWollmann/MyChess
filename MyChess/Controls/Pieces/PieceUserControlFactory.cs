using System.Windows.Controls;
using MyChessEngine;

namespace MyChess.Controls.Pieces
{
    public class PieceUserControlFactory
    {
        public static UserControl CreatePieceUserControl(IPiece piece)
        {
            return piece.Type switch
            {
                ChessEngineConstants.PieceType.Pawn => new PawnUserControl(piece),
                ChessEngineConstants.PieceType.Bishop => new BishopUserControl(piece),
                ChessEngineConstants.PieceType.Rook => new RookUserControl(piece),
                ChessEngineConstants.PieceType.Queen => new QueenUserControl(piece),
                ChessEngineConstants.PieceType.King => new KingUserControl(piece),
                ChessEngineConstants.PieceType.Knight => new KnightUserControl(piece),
                _ => null
            };
        }
    }
}
