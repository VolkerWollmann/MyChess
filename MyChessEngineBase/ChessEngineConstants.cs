using System;
using System.Collections.Generic;

namespace MyChessEngineBase
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
        None,
        Normal,
        WhiteChecked,
        BlackChecked,
        WhiteVictory,
        BlackVictory,
        StaleMate
    }

    public enum Evaluation
    {
        None,
        WhiteCheckMate,
        WhiteStaleMate,
        Normal,
        BlackStaleMate,
        BlackCheckMate,
        Remis
    }

    [Flags]
    public enum MoveType
    {
        Normal=1,
        PawnDoubleStep=2,
        EnpassantWhiteLeft=4,
        EnpassantWhiteRight=8,
        EnpassantBlackLeft=16,
        EnpassantBlackRight=32,
        WhiteCastle=64,
        WhiteCastleLong=128,
        BlackCastle=256,
        BlackCastleLong=512,
        Promotion=1024
    }

    public enum IsValidPositionReturns
    {
        NoPosition,
        EmptyField,
        EnemyBeatPosition,
        OwnBeatPosition,
        EnemyKingThreatPosition
    }

    public class ChessEngineConstants
    {
        public const int Length = 8;

        public const int CheckMate = 100000;
        public const int King = 10000;
        public const int Queen = 90;
        public const int Rook = 50;
        public const int Bishop = 35;
        public const int Knight = 30;
        public const int Pawn = 10;

        public static string[,] FieldNames = new[,]
        {
            {"A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8" },
            {"B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8" },
            {"C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8" },
            {"D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8" },
            {"E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8" },
            {"F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8" },
            {"G1", "G2", "G3", "G4", "G5", "G6", "G7", "G8" },
            {"H1", "H2", "H3", "H4", "H5", "H6", "H7", "H8" },
        };

        public static List<Color> BothColors = new List<Color> { Color.White, Color.Black };

        public static Color NextColorToMove(Color color)
        {
            return color == Color.White ? Color.Black : Color.White;
        }
    }
}
