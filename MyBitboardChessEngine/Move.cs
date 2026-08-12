using System.Diagnostics;

namespace MyBitboardChessEngine
{
    public enum MoveFlag : byte
    {
        Normal = 0,
        DoublePush = 1,
        EnPassant = 2,
        CastleKingSide = 3,
        CastleQueenSide = 4,
        Promotion = 5      // promotion is always to a queen
    }

    [DebuggerDisplay("{ToString()}")]
    public readonly struct Move
    {
        public readonly byte From;
        public readonly byte To;
        public readonly sbyte Piece;     // moving piece code 0..11
        public readonly sbyte Captured;  // captured piece code or NoPiece
        public readonly MoveFlag Flag;

        public Move(int from, int to, int piece, int captured = Constants.NoPiece, MoveFlag flag = MoveFlag.Normal)
        {
            From = (byte)from;
            To = (byte)to;
            Piece = (sbyte)piece;
            Captured = (sbyte)captured;
            Flag = flag;
        }

        public bool IsCapture => Captured != Constants.NoPiece;

        /// 16-bit encoding (6 bits from, 6 bits to, 4 bits flag) for the
        /// transposition table. 0 doubles as "no move": A1->A1 is never legal.
        public ushort Packed => (ushort)(From | (To << 6) | ((int)Flag << 12));

        public override string ToString()
        {
            return $"{Constants.NameOf(From)}-{Constants.NameOf(To)}";
        }
    }
}
