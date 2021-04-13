namespace MyChess.Common
{
    public class ChessConstants
    {
        public const string QuitCommand = "QuitCommand";
        public const string NewCommand = "NewCommand";
        public const string ClearCommand = "ClearCommand";
        public const string Test1Command = "Test1Command";

        public const int Length = 8;

        public enum Piece
        {
            None,
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
    }
}
