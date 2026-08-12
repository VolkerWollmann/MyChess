namespace MyBitboardChessEngine
{
    [Flags]
    public enum CastleRights : byte
    {
        None = 0,
        WhiteKingSide = 1,
        WhiteQueenSide = 2,
        BlackKingSide = 4,
        BlackQueenSide = 8,
        All = 15
    }

    public enum GameState
    {
        Normal,
        WhiteLoss,   // white king is off the board or checkmated
        BlackLoss,   // black king is off the board or checkmated
        Remis        // stalemate or draw by repetition
    }

    public static class Constants
    {
        public const int White = 0;
        public const int Black = 1;

        // Piece types 0..5; a piece code is type + 6 * color (0..11), NoPiece = -1
        public const int Pawn = 0;
        public const int Knight = 1;
        public const int Bishop = 2;
        public const int Rook = 3;
        public const int Queen = 4;
        public const int King = 5;
        public const int NoPiece = -1;

        public const int WhitePawn = 0;
        public const int WhiteKnight = 1;
        public const int WhiteBishop = 2;
        public const int WhiteRook = 3;
        public const int WhiteQueen = 4;
        public const int WhiteKing = 5;
        public const int BlackPawn = 6;
        public const int BlackKnight = 7;
        public const int BlackBishop = 8;
        public const int BlackRook = 9;
        public const int BlackQueen = 10;
        public const int BlackKing = 11;

        public static int MakePiece(int type, int color) => type + 6 * color;
        public static int TypeOf(int piece) => piece % 6;
        public static int ColorOf(int piece) => piece / 6;

        // Material values (the king carries no material value: in legal play it
        // is never captured, and a missing king ends the game before material
        // matters)
        public static readonly int[] PieceValues = { 100, 300, 350, 500, 900, 0 };

        // Mate scores are root-relative: Mate - pliesFromRoot, so a faster mate
        // always outranks a slower one and any material score, and the losing
        // side maximizes the distance - it defends instead of grabbing material.
        public const int Mate = 100000;
        public const int MateThreshold = Mate - 1000;
        public const int Infinity = Mate + 1;

        // Evaluation bonus per attacked field, counted for BOTH sides at the
        // leaf. The difference is bounded by 64 fields, so it always stays
        // below one pawn: it only decides between moves of equal material.
        // Counting both sides keeps the evaluation a pure function of the
        // position - the precondition for reusing table entries across depths.
        public const int MobilityValue = 1;

        public const int DefaultSearchDepth = 6;

        public const int MaxPly = 128;

        /// "E4" -> square index 0..63 (A1 = 0, H8 = 63)
        public static int SquareOf(string name)
        {
            return (name[1] - '1') * 8 + (char.ToUpperInvariant(name[0]) - 'A');
        }

        /// square index -> "E4"
        public static string NameOf(int square)
        {
            return $"{(char)('A' + (square & 7))}{(char)('1' + (square >> 3))}";
        }
    }
}
