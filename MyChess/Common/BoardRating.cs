using System;
using static MyChess.Common.ChessConstants;

namespace MyChess.Common
{
    /// <summary>
    /// ![CDATA[
    /// Order by Evaluation, Value
    ///         WhiteCheckMate,
    ///         WhiteChecked,
    ///         Normal,
    ///                 Value : > 0 better for White
    ///                         = 0 equal
    ///                         > 0 better for Black
    ///         BlackChecked,
    ///         BlackCheckMate
    /// ]]>
    /// </summary>
    public class BoardRating 
    {
        public Situation Situation;
        public Evaluation Evaluation;
        public int Value;

        /// <summary>
        /// Partial Compare
        /// </summary>
        /// <param name="other"></param>
        // < 0 This instance precedes obj in the sort order.
        // = 0 This instance occurs in the same position in the sort order as obj.
        // > 0 This instance follows obj in the sort order.
        public int PartialCompare(BoardRating other, ChessConstants.Color color)
        {
            if (this.Evaluation == Evaluation.WhiteCheckMate)
            {
                if (other.Evaluation == Evaluation.WhiteCheckMate)
                    return 0;

                return 1;
            }

            if (this.Evaluation == Evaluation.BlackCheckMate)
            {
                if (other.Evaluation == Evaluation.BlackCheckMate)
                    return 0;

                return -1;
            }

            return 0;
        }
    }
}
