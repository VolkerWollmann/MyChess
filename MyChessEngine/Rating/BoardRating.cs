using System.Collections.Generic;
using System.Diagnostics;

namespace MyChessEngine.Rating
{
    [DebuggerDisplay("{ToString()}")]
    public class BoardRating
    {
        public Situation Situation;
        public Evaluation Evaluation;
        public int Weight;
        public int Depth;

        public BoardRating()
        {
            Situation = Situation.Normal;
            Evaluation = Evaluation.Normal;
            Weight = 0;
            Depth = 0;
        }

        public override string ToString()
        {
            return $"Sit:{Situation} Eval:{Evaluation} W:{Weight} D:{Depth}";
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
        private static readonly Dictionary<Evaluation, int> WhiteDictionary = new Dictionary<Evaluation, int>()
        {
            {Evaluation.BlackCheckMate, 5},
            {Evaluation.Normal, 4},
            {Evaluation.WhiteStaleMate, 3},
            {Evaluation.BlackStaleMate, 2},
            {Evaluation.WhiteCheckMate, 1},

        };


        /// <summary>
        /// Compare for white
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>-1 for x greater y</returns>
        public int Compare(BoardRating x, BoardRating y)
        {
            int xIndex = WhiteDictionary[x.Evaluation];
            int yIndex = WhiteDictionary[y.Evaluation];

            if (xIndex == yIndex)
            {
                if (xIndex == 4)
                {
                    if (x.Weight > y.Weight)
                        return -1;
                    if (x.Weight < y.Weight)
                        return 1;
                    if (x.Weight == y.Weight)
                        return 0;
                }

                if (xIndex == 5)
                {
                    if (x.Depth < y.Depth)
                        return -1;

                    if (x.Depth > y.Depth)
                        return +1;

                    return 0;
                }

                if (xIndex == 1)
                {
                    if (x.Depth > y.Depth)
                        return -1;

                    if (x.Depth < y.Depth)
                        return +1;

                    return 0;
                }

                return 0;
            }

            if (xIndex > yIndex)
                return -1;

            if (xIndex < yIndex)
                return 1;

            return 0;
        }
    }


    public class BlackBoardRatingComparer : IBoardRatingComparer
    {

        private static readonly Dictionary<Evaluation, int> BlackDictionary = new Dictionary<Evaluation, int>()
        {
            {Evaluation.WhiteCheckMate, 5},
            {Evaluation.Normal, 4 },
            {Evaluation.WhiteStaleMate,3 },
            {Evaluation.BlackStaleMate,2 },
            {Evaluation.BlackCheckMate,1},

        };


        /// <summary>
        /// Compare for black
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>-1 for x greater y</returns>
        public int Compare(BoardRating x, BoardRating y)
        {
            int xIndex = BlackDictionary[x.Evaluation];
            int yIndex = BlackDictionary[y.Evaluation];

            if (xIndex == yIndex)
            {
                if (xIndex == 4)
                {
                    if (x.Weight < y.Weight)
                        return -1;
                    if (x.Weight > y.Weight)
                        return 1;
                    if (x.Weight == y.Weight)
                        return 0;
                }

                if (xIndex == 5)
                {
                    if (x.Depth < y.Depth)
                        return -1;

                    if (x.Depth > y.Depth)
                        return +1;

                    return 0;
                }

                if (xIndex == 1)
                {
                    if (x.Depth > y.Depth)
                        return -1;

                    if (x.Depth < y.Depth)
                        return +1;

                    return 0;
                }

                return 0;
            }


            if (xIndex > yIndex)
                return -1;

            if (xIndex < yIndex)
                return 1;

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
