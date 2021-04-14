namespace MyChess.Common
{
    public class ChessConstants
    {
        public const string QuitCommand = "QuitCommand";
        public const string NewCommand = "NewCommand";
        public const string ClearCommand = "ClearCommand";
        public const string Test1Command = "Test1Command";
        public const string MoveCommand = "MoveCommand";

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

        public static Color NextColor(Color color)
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
            Victory
        }

        public enum Evaluation
        {
            WhiteCheckMate,
            WhiteChecked,
            Normal,
            BlackChecked,
            BlackCheckMate
        }
    }
}
