using System.Collections.Generic;
using System.Diagnostics;
using MyChessEngineBase;
using MyChessEngineBase.Rating;

namespace MyChessEngineBase.Rating
{
    [DebuggerDisplay("{ToString()}")]
    public class BoardRating
    {
        public Situation Situation;
        public Evaluation Evaluation;
        public int Weight;
        public int Depth;

        public string MoveList;

        public BoardRating()
        {
            Situation = Situation.None;
            Evaluation = Evaluation.None;
            Weight = 0;
            Depth = 0;
            MoveList = string.Empty;
		}

        public void AddMove(string move)
        {
	        MoveList = move + ";" + MoveList;

		}

        public override string ToString()
        {
            return $"Situation:{Situation} Evaluation:{Evaluation} Weight:{Weight} Depth:{Depth}";
        }
    }

    #region IBoardRatingComparer
    public interface IBoardRatingComparer : IComparer<BoardRating>
    {

    }

    /// <summary>
    /// ![CDATA[
    /// Pre order by Evaluation, Value (e.g for White )
    ///         BlackCheckMate,
    ///         Normal,
    ///                 Value : > 0 better for White
    ///                         = 0 equal
    ///                         > 0 better for Black
    ///         WhiteStaleMate,
    ///         BlackStaleMate,
    ///         WhiteCheckMate
    /// ]]>
    /// </summary>
    public class WhiteBoardRatingComparer : IBoardRatingComparer
    {
       
        /// <summary>
        /// Compare for white : W
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>-1 : x less y
        ///           0 : x = y
        ///           1 : x greater y
        /// </returns>
        public int Compare(BoardRating x, BoardRating y)
        {
            if ((x == null) || (y == null))
                return 0;

            if (x.Weight > y.Weight)
                return 1;
            if (x.Weight < y.Weight)
                return -1;

            // Equal weight: in winning lines the faster win (smaller depth) is
            // better - direct king kill > mate in one > mate in two - while in
            // losing lines the later loss (larger depth) is better, so the
            // loser defends the king instead of grabbing material.
            if (x.Weight >= 0)
            {
                if (x.Depth < y.Depth)
                    return 1;
                if (x.Depth > y.Depth)
                    return -1;
            }
            else
            {
                if (x.Depth > y.Depth)
                    return 1;
                if (x.Depth < y.Depth)
                    return -1;
            }

            return 0;
        }
    }


    public class BlackBoardRatingComparer : IBoardRatingComparer
    {
        /// <summary>
        /// Compare for black
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>-1 for x greater y</returns>
        public int Compare(BoardRating x, BoardRating y)
        {
            if ((x == null) || (y == null))
                return 0;

            if (x.Weight < y.Weight)
                return 1;
            if (x.Weight > y.Weight)
                return -1;

            // Equal weight: faster win for black (weight <= 0), later loss
            // for black (weight > 0), mirroring WhiteBoardRatingComparer.
            if (x.Weight <= 0)
            {
                if (x.Depth < y.Depth)
                    return 1;
                if (x.Depth > y.Depth)
                    return -1;
            }
            else
            {
                if (x.Depth > y.Depth)
                    return 1;
                if (x.Depth < y.Depth)
                    return -1;
            }

            return 0;
        }
    }

    public class BoardRatingComparerFactory
    {
        private static readonly Dictionary<Color, IBoardRatingComparer> Comparer =
            new Dictionary<Color, IBoardRatingComparer>
            {
                {Color.White, new WhiteBoardRatingComparer()},
                {Color.Black, new BlackBoardRatingComparer()}
            };

        public static IBoardRatingComparer GetComparer(Color color)
        {
            return Comparer[color];
        }
    }

    #endregion
}
