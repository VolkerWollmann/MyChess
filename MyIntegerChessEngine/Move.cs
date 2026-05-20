using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MyChessEngineBase;

namespace MyIntegerChessEngine
{
    public class Move(Position start, Position end, Piece piece)
    {
        public Piece Piece = piece;
        public Position Start = start;
        public Position End = end;
    }

    public class MoveList : List<Move>
    {
        new public void Add(Move move) => base.Add(move);
    }

}


