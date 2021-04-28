using System.Windows.Controls;
using MyChessEngineBase;

namespace MyChess.Controls.Pieces
{
    public class PieceUserControlFactory
    {
        public static UserControl CreatePieceUserControl(IPiece piece)
        {
            return piece.Type switch
            {
                PieceType.Pawn => new PawnUserControl(piece),
                PieceType.Bishop => new BishopUserControl(piece),
                PieceType.Rook => new RookUserControl(piece),
                PieceType.Queen => new QueenUserControl(piece),
                PieceType.King => new KingUserControl(piece),
                PieceType.Knight => new KnightUserControl(piece),
                _ => null
            };
        }
    }
}
