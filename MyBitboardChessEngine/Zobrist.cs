namespace MyBitboardChessEngine
{
    /// Deterministic 64-bit Zobrist keys (fixed-seed SplitMix64). Unlike the
    /// TranspositionChessEngine, the key here is not recomputed by scanning
    /// the board: Board.MakeMove maintains it incrementally with a handful of
    /// XORs, which makes probing the transposition table essentially free.
    internal static class Zobrist
    {
        /// [piece code 0..11, square 0..63]
        internal static readonly ulong[,] PieceKeys = new ulong[12, 64];

        /// One key per castle right (CastleRights bit order).
        internal static readonly ulong[] CastleKeys = new ulong[4];

        /// One key per file of a pawn capturable en passant right now.
        internal static readonly ulong[] EnPassantFileKeys = new ulong[8];

        internal static readonly ulong SideKey;

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

            for (int i = 0; i < 4; i++)
                CastleKeys[i] = Next();

            for (int file = 0; file < 8; file++)
                EnPassantFileKeys[file] = Next();

            SideKey = Next();
        }

        internal static ulong RightsKey(CastleRights rights)
        {
            ulong key = 0;
            for (int i = 0; i < 4; i++)
            {
                if (((int)rights & (1 << i)) != 0)
                    key ^= CastleKeys[i];
            }
            return key;
        }
    }
}
