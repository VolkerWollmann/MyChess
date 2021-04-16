namespace MyChessEngine
{
    public enum PieceType
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public enum Color
    {
        White,
        Black
    }

    public enum FieldColor
    {
        Standard,
        Start,
        End
    }

    public enum Situation
    {
        Normal,
        Remis,
        Patt,
        WhiteChecked,
        BlackChecked,
        Victory
    }

    public enum Evaluation
    {
        WhiteCheckMate,
        WhiteStaleMate,
        Normal,
        BlackStaleMate,
        BlackCheckMate
    }

    public enum MoveType
    {
        Normal,
        PawnDoubleStep,
        EnpasantWhiteLeft,
        EnpasantWhiteRight,
        EnpasantBlackLeft,
        EnpasantBlackRight,
        WhiteCastle,
        WhiteCastleLong,
        BlackCastle,
        BlackCastleLong,
        Promotion
    }

    public class ChessEngineConstants
    {
        public const int Length = 8;

        public static Color NextColorToMove(Color color)
        {
            return color == Color.White ? Color.Black : Color.White;
        }
    }
}
