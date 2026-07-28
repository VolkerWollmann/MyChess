using System;
using System.Collections.Generic;
using System.Text;

namespace MyIntegerChessEngine
{
    public class Constants
    {
        public const int GridSize = 12;
        public const int Planes = 5;      //0: piece, 1: last ply, 2: promotion 3: en passant marking 4: threatened fields
        public const int BroadPlane = 0;
        public const int LastPlyPlane = 1;   // Administrative plane : last ply when piece moved, for en passant and castling rights
        public const int PromotionPlane = 2;
        public const int EnPassantPlane = 3;
        public const int ThreatPlane = 4;    // Fields threatened by the opponent, written by MarkThreatenedFields
        
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

    }
}
