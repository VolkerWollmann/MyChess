using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MyChessEngineBase;

namespace MyIntegerChessEngine
{
    public enum CastleType
    {
        None,
        WhiteKingSide,
        WhiteQueenSide,
        BlackKingSide,
        BlackQueenSide
    }

    public class Move(Position start, Position end, Piece piece, CastleType castleType = CastleType.None)
    {
        public Piece Piece = piece;
        public Position Start = start;
        public Position End = end;
        public CastleType CastleType = castleType;
    }

    public class MoveList : List<Move>
    {
        new public void Add(Move move) => base.Add(move);
    }

}


