using MyChessEngineBase;
namespace MyChessEngine
{
    public interface IPiece
    {
        PieceType Type { get; }

        Color Color { get; }
    }
}
