using System.Collections.Generic;

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
        StaleMate,
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

        public const int King = 10000;
        public const int Queen = 90;
        public const int Rook = 50;
        public const int Bishop = 35;
        public const int Knight = 30;
        public const int Pawn = 10;

        public static List<Color> BothColors = new List<Color> { Color.White, Color.Black };

        public static Color NextColorToMove(Color color)
        {
            return color == Color.White ? Color.Black : Color.White;
        }
    }
}
