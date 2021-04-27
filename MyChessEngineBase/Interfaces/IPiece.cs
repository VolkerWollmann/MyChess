using MyChessEngineBase;
namespace MyChessEngineBase
{
    public interface IPiece
    {
        PieceType Type { get; }

        Color Color { get; }
    }
}
