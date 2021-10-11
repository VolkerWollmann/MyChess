using System.Collections.Generic;
using MyChessEngineBase;
using MyChessEngineBase.Rating;

namespace MyChessEngineInteger
{
    public class Move
    {
        public int StartRow;
        public int StartColumn;
        public int EndRow;
        public int EndColumn;
        public MoveType MoveType;

        public bool IsAMove { get; } = true;

        public BoardRating Rating { get; set; }


        public Move(int startRow, int startColumn, int endRow, int endColumn, MoveType moveType, BoardRating boardRating)
        {
            StartRow = startRow;
            StartColumn = startColumn;
            EndRow = endRow;
            EndColumn = endColumn;
            MoveType = moveType;
            Rating = boardRating;
        }

        public Move(int startRow, int startColumn, int endRow, int endColumn) : this(startRow, startColumn,
            endRow, endColumn, MoveType.Normal, null)
        {

        }

        public Move(MyChessEngineBase.Move move) : this(move.Start.Row, move.Start.Column, move.End.Row, move.End.Column,
            move.Type, move.Rating)
        {

        }

        public static Move CreateNoMove(BoardRating rating)
        {
            return new Move(-1, -1, -1, -1, MoveType.Normal, rating);
        }

    }

    public class MoveSorter : IComparer<Move>
    {
        public int Compare(Move x, Move y) => _Comparer.Compare(x?.Rating, y?.Rating);

        private readonly IBoardRatingComparer _Comparer;
        public MoveSorter(Color color)
        {
            _Comparer = BoardRatingComparerFactory.GetComparer(color);
        }

    }
}
