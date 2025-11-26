namespace MyChessEngineBase
{
    public interface IPiece
    {
        PieceType Type { get; }

        Color Color { get; }

        bool IsMoved { get; }
        int PromotionPly { get; set; }

        int LastEnPassantPlyMarking { get; set; }

    }
}
