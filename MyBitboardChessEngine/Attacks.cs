using System.Numerics;

namespace MyBitboardChessEngine
{
    /// Precomputed attack tables and classical ray-based slider attacks.
    /// Squares are A1 = 0 .. H8 = 63 (rank * 8 + file), one bit per square.
    public static class Attacks
    {
        public const ulong FileA = 0x0101010101010101UL;
        public const ulong FileH = FileA << 7;

        public static readonly ulong[] KnightAttacks = new ulong[64];
        public static readonly ulong[] KingAttacks = new ulong[64];

        /// [color, square]: squares a pawn of that color attacks FROM the square.
        public static readonly ulong[,] PawnAttacks = new ulong[2, 64];

        // Ray directions: positive rays grow toward higher square indices.
        private const int North = 0;     // +8
        private const int East = 1;      // +1
        private const int NorthEast = 2; // +9
        private const int NorthWest = 3; // +7
        private const int South = 4;     // -8
        private const int West = 5;      // -1
        private const int SouthEast = 6; // -7
        private const int SouthWest = 7; // -9

        private static readonly ulong[,] Rays = new ulong[8, 64];

        static Attacks()
        {
            (int df, int dr)[] deltas =
            {
                (0, 1), (1, 0), (1, 1), (-1, 1),   // positive rays
                (0, -1), (-1, 0), (1, -1), (-1, -1) // negative rays
            };

            for (int square = 0; square < 64; square++)
            {
                int file = square & 7;
                int rank = square >> 3;

                for (int direction = 0; direction < 8; direction++)
                {
                    ulong ray = 0;
                    int f = file + deltas[direction].df;
                    int r = rank + deltas[direction].dr;
                    while (f >= 0 && f < 8 && r >= 0 && r < 8)
                    {
                        ray |= 1UL << (r * 8 + f);
                        f += deltas[direction].df;
                        r += deltas[direction].dr;
                    }
                    Rays[direction, square] = ray;
                }

                KnightAttacks[square] = TargetMask(file, rank,
                    new[] { (-2, -1), (-2, 1), (2, -1), (2, 1), (-1, -2), (1, -2), (-1, 2), (1, 2) });

                KingAttacks[square] = TargetMask(file, rank,
                    new[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) });

                PawnAttacks[Constants.White, square] = TargetMask(file, rank, new[] { (-1, 1), (1, 1) });
                PawnAttacks[Constants.Black, square] = TargetMask(file, rank, new[] { (-1, -1), (1, -1) });
            }
        }

        private static ulong TargetMask(int file, int rank, (int df, int dr)[] deltas)
        {
            ulong mask = 0;
            foreach ((int df, int dr) in deltas)
            {
                int f = file + df;
                int r = rank + dr;
                if (f >= 0 && f < 8 && r >= 0 && r < 8)
                    mask |= 1UL << (r * 8 + f);
            }
            return mask;
        }

        /// Ray attack toward higher square indices: everything up to and
        /// including the first blocker.
        private static ulong PositiveRay(int direction, int square, ulong occupied)
        {
            ulong attacks = Rays[direction, square];
            ulong blockers = attacks & occupied;
            if (blockers != 0)
                attacks ^= Rays[direction, BitOperations.TrailingZeroCount(blockers)];
            return attacks;
        }

        /// Ray attack toward lower square indices.
        private static ulong NegativeRay(int direction, int square, ulong occupied)
        {
            ulong attacks = Rays[direction, square];
            ulong blockers = attacks & occupied;
            if (blockers != 0)
                attacks ^= Rays[direction, 63 - BitOperations.LeadingZeroCount(blockers)];
            return attacks;
        }

        public static ulong RookAttacks(int square, ulong occupied)
        {
            return PositiveRay(North, square, occupied)
                   | PositiveRay(East, square, occupied)
                   | NegativeRay(South, square, occupied)
                   | NegativeRay(West, square, occupied);
        }

        public static ulong BishopAttacks(int square, ulong occupied)
        {
            return PositiveRay(NorthEast, square, occupied)
                   | PositiveRay(NorthWest, square, occupied)
                   | NegativeRay(SouthEast, square, occupied)
                   | NegativeRay(SouthWest, square, occupied);
        }

        public static ulong QueenAttacks(int square, ulong occupied)
        {
            return RookAttacks(square, occupied) | BishopAttacks(square, occupied);
        }

        public static int PopLsb(ref ulong bits)
        {
            int square = BitOperations.TrailingZeroCount(bits);
            bits &= bits - 1;
            return square;
        }
    }
}
