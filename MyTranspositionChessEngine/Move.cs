using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MyChessEngineBase;

namespace MyTranspositionChessEngine
{
    [Flags]
    public enum CastleType
    {
        None = 0,
        WhiteKingSide = 1,
        WhiteQueenSide = 2,
        BlackKingSide = 4,
        BlackQueenSide = 8
    }

    public class Move(Position start, Position end, Piece piece, CastleType castleType = CastleType.None)
    {
        public Piece Piece = piece;
        public Position Start = start;
        public Position End = end;
        public CastleType CastleType = castleType;

        /// Expected rating after this move, set by Board.CalculateMove.
        public Rating Rating;
    }

    public class MoveList : List<Move>
    {
        new public void Add(Move move) => base.Add(move);
    }

}


