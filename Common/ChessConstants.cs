namespace MyChess.Common
{
    public class ChessConstants
    {
        public static string QuitCommand = "QuitCommand";
        public static string Test1Command = "Test1Command";

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
    }
}
