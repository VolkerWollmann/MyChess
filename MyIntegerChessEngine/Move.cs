using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MyChessEngineBase;

namespace MyIntegerChessEngine
{
    public enum CastleType
    {
        NoCastle = 0,
        WhiteKingSide = 1,
        WhiteQueenSide = 2,
        BlackKingSide = 3,
        BlackQueenSide = 4
    }
    public class Move(Position start, Position end, Piece piece, CastleType castleType = CastleType.NoCastle, int enPassant = 0)
    {
        public Piece Piece = piece;
        public Position Start = start;
        public Position End = end;

        public int EnPassant = enPassant;
        public CastleType CastleType = castleType;
    }

    public class MoveList : List<Move>
    {
        new public void Add(Move move) => base.Add(move);
    }

}


