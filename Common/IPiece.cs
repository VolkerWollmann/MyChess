namespace MyChess.Common
{
    public interface IPiece
    {
        ChessConstants.Piece GetPieceType();

        ChessConstants.Color GetColor();
    }
}
