namespace MyChess.Common
{
    public interface IPiece
    {
        ChessConstants.Piece Type { get; }

        ChessConstants.Color Color { get; }
    }
}
