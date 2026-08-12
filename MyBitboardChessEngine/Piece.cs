using MyChessEngineBase;

namespace MyBitboardChessEngine
{
    /// Minimal IPiece for the UI: the bitboard engine keeps no per-piece
    /// state, so this struct only carries type and color.
    public struct Piece : IPiece
    {
        public PieceType Type { get; }

        public Color Color { get; }

        public int LastPly { get; set; }

        public int PromotionPly { get; set; }

        public int LastEnPassantPlyMarking { get; set; }

        public Piece(PieceType type, Color color)
        {
            Type = type;
            Color = color;
            LastPly = -1;
            PromotionPly = -1;
            LastEnPassantPlyMarking = -1;
        }

        public bool Compare(IPiece other)
        {
            return other != null && Type == other.Type && Color == other.Color;
        }

        public bool IsMoved()
        {
            return false;
        }
    }
}
