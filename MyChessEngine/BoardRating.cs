using System.Collections.Generic;
using System.Diagnostics;

namespace MyChessEngine
{
    [DebuggerDisplay("Situation:{Situation} Evaluation:{Evaluation} Weight:{Weight}")]
    public class BoardRating
    {
        public Situation Situation;
        public Evaluation Evaluation;
        public int Weight;

        public BoardRating()
        {
            Situation = Situation.Normal;
            Evaluation = Evaluation.Normal;
            Weight = 0;
        }

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
    public class BoardRatingComparer : IComparer<BoardRating>
    {
        private static readonly Dictionary<Evaluation, int> WhiteDictionary = new Dictionary<Evaluation, int>()
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
        public int CompareWhite(BoardRating x, BoardRating y)
        {
            int xIndex = WhiteDictionary[x.Evaluation];
            int yIndex = WhiteDictionary[y.Evaluation];

            if ((xIndex == 4) && (yIndex == 4))
            {
                if (x.Weight > y.Weight)
                    return -1;
                if (x.Weight < y.Weight)
                    return 1;
                if (x.Weight == y.Weight)
                    return 0;
            }

            if (xIndex > yIndex)
                return -1;

            if (xIndex < yIndex)
                return 1;

            return 0;
        }

        private static readonly Dictionary<Evaluation, int> BlackDictionary = new Dictionary<Evaluation, int>()
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
        public int CompareBlack(BoardRating x, BoardRating y)
        {
            int xIndex = BlackDictionary[x.Evaluation];
            int yIndex = BlackDictionary[y.Evaluation];

            if ((xIndex == 4) && (yIndex == 4))
            {
                if (x.Weight < y.Weight)
                    return -1;
                if (x.Weight > y.Weight)
                    return 1;
                if (x.Weight == y.Weight)
                    return 0;
            }

            if (xIndex > yIndex)
                return -1;

            if (xIndex < yIndex)
                return 1;

            return 0;
        }

        public int Compare(BoardRating x, BoardRating y)
        {
            if (_Color == Color.White)
                return CompareWhite(x, y);
            else
                return CompareBlack(x, y);
        }

        private readonly Color _Color;
        public BoardRatingComparer(Color color)
        {
            _Color = color;
        }
    }

}
