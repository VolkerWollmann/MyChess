using System;
using System.Collections.Generic;
using System.Text;

namespace MyTranspositionChessEngine
{
    public class Constants
    {
        public const int GridSize = 12;
        public const int Planes = 4;      //0: piece, 1: last ply, 2: en passant marking 3: threatened fields
        public const int BroadPlane = 0;
        public const int LastPlyPlane = 1;   // Administrative plane : last ply when piece moved, for en passant and castling rights
        public const int EnPassantPlane = 2;
        public const int ThreatPlane = 3;    // Fields threatened by the opponent, written by MarkThreatenedFields
        
        public const int BoardBorder = -1;

        public const int PieceMask = 7;
        public const int NoPiece = 0;
        public const int Pawn = 1;
        public const int Knight = 2;
        public const int Bishop = 3;
        public const int Rook = 4;
        public const int Queen = 5;
        public const int King = 6;

        public const int ColorMask = 8;
        public const int White = 0;
        public const int Black = 8;

        public const int DefaultSearchDepth = 3;

        // Material values for the evaluation function
        public const int PawnValue = 100;
        public const int KnightValue = 300;
        public const int BishopValue = 350;
        public const int RookValue = 500;
        public const int QueenValue = 900;
        public const int KingValue = 10000;

        // Weight of one ply of depth in win/loss ratings. Win ratings are
        // depth-dominated and material-free, so the ranking is always
        // direct king kill > mate in one > mate in two > ... > material win.
        // 1000 keeps every win rating above any reachable material rating.
        public const int WinDepthValue = 1000;

        // Evaluation bonus per threatened field at the leaf evaluation.
        // The count is bounded by the 64 board fields, so the term always
        // stays below one pawn: it never outweighs material and only
        // decides between moves of equal material. 0 turns the threat-field
        // evaluation off completely.
        // static readonly instead of const: the JIT still folds it, but the
        // on/off branch does not trigger an unreachable-code warning.
        public static readonly int ThreatFieldValue = 1;

        // Piece geometry, single source for move generation, threat
        // marking/counting and move ordering. The element order is load-bearing:
        // move generation order breaks ties between equally rated moves.
        public static readonly int[,] KnightDeltas =
        {
            { -2, -1 }, { -2,  1 }, {  2, -1 }, { 2, 1 },
            { -1, -2 }, {  1, -2 }, { -1,  2 }, { 1, 2 }
        };

        public static readonly int[,] StraightDirections =
        {
            { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }
        };

        public static readonly int[,] DiagonalDirections =
        {
            { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 }
        };

        // Diagonals first, then straights - the order queen and king use
        public static readonly int[,] AllDirections =
        {
            { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 },
            { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }
        };

        public static int PieceValue(int pieceType)
        {
            return pieceType switch
            {
                Pawn => PawnValue,
                Knight => KnightValue,
                Bishop => BishopValue,
                Rook => RookValue,
                Queen => QueenValue,
                King => KingValue,
                _ => 0
            };
        }

    }
}
