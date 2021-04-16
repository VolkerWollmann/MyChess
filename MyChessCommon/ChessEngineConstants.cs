namespace MyChessEngineCommon
{
    public class ChessEngineConstants
    {
        public const int Length = 8;

        public enum Piece
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

        public static Color NextColorToMove(Color color)
        {
            return color == Color.White ? Color.Black : Color.White;
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
            Normal,
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
    }
}
