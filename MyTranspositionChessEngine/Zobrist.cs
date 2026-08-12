namespace MyTranspositionChessEngine
{
    /// Deterministic 64-bit Zobrist keys (fixed-seed SplitMix64): one key per
    /// (piece, color, square), per en-passant-capturable pawn square, per
    /// castle right and for black to move. XORing the keys of everything that
    /// influences move generation and evaluation yields the position key the
    /// transposition table is addressed with; two positions that allow the
    /// same moves and evaluate the same get the same key.
    internal static class Zobrist
    {
        /// Index: [(pieceType - 1) + (black ? 6 : 0), square 0..63]
        internal static readonly ulong[,] PieceKeys = new ulong[12, 64];

        /// Key for a pawn that may be captured en passant right now.
        internal static readonly ulong[] EnPassantKeys = new ulong[64];

        /// White king side, white queen side, black king side, black queen side.
        internal static readonly ulong[] CastleKeys = new ulong[4];

        internal static readonly ulong BlackToMoveKey;

        static Zobrist()
        {
            ulong state = 0x9E3779B97F4A7C15UL;

            ulong Next()
            {
                state += 0x9E3779B97F4A7C15UL;
                ulong z = state;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                return z ^ (z >> 31);
            }

            for (int piece = 0; piece < 12; piece++)
            for (int square = 0; square < 64; square++)
                PieceKeys[piece, square] = Next();

            for (int square = 0; square < 64; square++)
                EnPassantKeys[square] = Next();

            for (int i = 0; i < 4; i++)
                CastleKeys[i] = Next();

            BlackToMoveKey = Next();
        }
    }
}
