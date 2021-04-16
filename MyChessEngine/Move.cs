using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace MyChessEngine
{
    [DebuggerDisplay("{ToString()}")]
    public class Move 
    {
        public Position Start;
        public Position End;
        public IPiece Piece;
        public MoveType Type;
        public BoardRating Rating { get; set; } = null;

        public Move(Position start, Position end, IPiece piece, MoveType type)
        {
            Start = start;
            End = end;
            Piece = piece;
            Type = type;
        }

        public Move(Position start, Position end, IPiece piece) : this(start, end, piece, MoveType.Normal)
        {
        }

        public Move(string startString, string endString, IPiece piece, MoveType type)
        {
            Start = new Position(startString);
            End = new Position(endString);
            Piece = piece;
            Type = type;
        }


        public Move(string startString, string endString, IPiece piece) : 
            this(startString, endString, piece, MoveType.Normal)
        {
        }

        public override string ToString()
        {
            if (Piece != null)
                return
                    $"{Piece.Color.ToString().Substring(0, 1),1} {Piece.Type.ToString(),-10} {Start} -> {End} T:{Type}";
            else
                return $"-           {Start} -> {End} T:{Type}";
        }
    }

    public class MoveSorter : IComparer<Move>
    {
        private static readonly Dictionary<Evaluation, int> _whiteDictionary = new Dictionary<Evaluation, int>()
        {
            {Evaluation.BlackCheckMate, 5},
            {Evaluation.Normal, 4 },
            {Evaluation.WhiteStaleMate,3 },
            {Evaluation.BlackStaleMate,2 },
            {Evaluation.WhiteCheckMate,1},

        };


        /// <summary>
        /// Compare 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareWhite(Move x, Move y)
        {
            int xIndex = _whiteDictionary[x.Rating.Evaluation];
            int yIndex = _whiteDictionary[y.Rating.Evaluation];

            if ((xIndex == 4) && (yIndex == 4))
            {
                if (x.Rating.Value > y.Rating.Value)
                    return -1;
                if (x.Rating.Value < y.Rating.Value)
                    return 1;
                if (x.Rating.Value == y.Rating.Value)
                    return 0;
            }

            if (xIndex > yIndex)
                return -1;

            if (xIndex < yIndex)
                return 1;

            return 0;
        }

        private static readonly Dictionary<Evaluation, int> _blackDictionary = new Dictionary<Evaluation, int>()
        {
            {Evaluation.WhiteCheckMate, 5},
            {Evaluation.Normal, 4 },
            {Evaluation.WhiteStaleMate,3 },
            {Evaluation.BlackStaleMate,2 },
            {Evaluation.BlackCheckMate,1},

        };


        /// <summary>
        /// Compare 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareBlack(Move x, Move y)
        {
            int xIndex = _blackDictionary[x.Rating.Evaluation];
            int yIndex = _blackDictionary[y.Rating.Evaluation];

            if ((xIndex == 4) && (yIndex == 4))
            {
                if (x.Rating.Value < y.Rating.Value)
                    return -1;
                if (x.Rating.Value > y.Rating.Value)
                    return 1;
                if (x.Rating.Value == y.Rating.Value)
                    return 0;
            }

            if (xIndex > yIndex)
                return -1;

            if (xIndex < yIndex)
                return 1;

            return 0;
        }

        public int Compare(Move x, Move y)
        {
            if (_Color == Color.White)
                return CompareWhite(x, y);
            else
                return CompareBlack(x, y);
        }

        private Color _Color;
        public MoveSorter(Color color)
        {
            _Color = color;
        }

    }
}
