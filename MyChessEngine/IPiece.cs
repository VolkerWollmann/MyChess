using MyChessEngineCommon;

namespace MyChess.Common
{
    public interface IPiece
    {
        ChessEngineConstants.Piece Type { get; }

        ChessEngineConstants.Color Color { get; }
    }
}
