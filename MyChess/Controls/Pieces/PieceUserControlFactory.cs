using System.Windows.Controls;
using MyChess.Common;
using MyChessEngineCommon;

namespace MyChess.Controls.Pieces
{
    public class PieceUserControlFactory
    {
        public static UserControl CreatePieceUserControl(IPiece piece)
        {
            return piece.Type switch
            {
                ChessEngineConstants.Piece.Pawn => new PawnUserControl(piece),
                ChessEngineConstants.Piece.Bishop => new BishopUserControl(piece),
                ChessEngineConstants.Piece.Rook => new RookUserControl(piece),
                ChessEngineConstants.Piece.Queen => new QueenUserControl(piece),
                ChessEngineConstants.Piece.King => new KingUserControl(piece),
                ChessEngineConstants.Piece.Knight => new KnightUserControl(piece),
                _ => null
            };
        }
    }
}
