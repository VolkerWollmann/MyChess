using System;
using System.Collections.Generic;
using System.Text;
using MyChessEngineBase;

namespace MyChessEngineInteger.Pieces
{
    public enum NumPieces
    {
        WhitePawn = 100,
        WhiteKnight = 300,
        WhiteBishop = 350,
        WhiteRook = 500,
        WhiteQueen = 900,
        WhiteKing = 10000,

        BlackPawn = -100,
        BlackKnight = -300,
        BlackBishop = -350,
        BlackRook = -500,
        BlackQueen = -900,
        BlackKing = -10000,
    }
    public class Piece : IPiece
    {
        public PieceType Type { get; }
        public Color Color { get; }

        public Piece(int piece)
        {

        }
    }
}
