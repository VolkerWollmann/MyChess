using MyChessEngine;

namespace MyChessEngine
{
    public interface IPiece
    {
        ChessEngineConstants.PieceType Type { get; }

        ChessEngineConstants.Color Color { get; }
    }
}
