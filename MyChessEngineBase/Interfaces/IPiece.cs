namespace MyChessEngineBase
{
    public interface IPiece
    {
        PieceType Type { get; }

        Color Color { get; }

        int LastPly { get; set; }
        int PromotionPly { get; set; }

        int LastEnPassantPlyMarking { get; set; }

        bool Compare(IPiece other);

        bool IsMoved();
    }
}
