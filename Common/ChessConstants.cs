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
            return color == ChessConstants.Color.White ? ChessConstants.Color.Black : ChessConstants.Color.White;
        }

        public enum FieldColor
        {
            Standard,
            Start,
            End
        }
    }
}
