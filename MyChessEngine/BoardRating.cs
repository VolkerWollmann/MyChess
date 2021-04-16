using MyChessEngine;

namespace MyChessEngine
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
        public ChessEngineConstants.Situation Situation;
        public ChessEngineConstants.Evaluation Evaluation;
        public int Value;

        /// <summary>
        /// Partial Compare
        /// </summary>
        /// <param name="other"></param>
        // < 0 This instance precedes obj in the sort order.
        // = 0 This instance occurs in the same position in the sort order as obj.
        // > 0 This instance follows obj in the sort order.
        public int PartialCompare(BoardRating other, ChessEngineConstants.Color color)
        {
            if (this.Evaluation == ChessEngineConstants.Evaluation.WhiteCheckMate)
            {
                if (other.Evaluation == ChessEngineConstants.Evaluation.WhiteCheckMate)
                    return 0;

                return 1;
            }

            if (this.Evaluation == ChessEngineConstants.Evaluation.BlackCheckMate)
            {
                if (other.Evaluation == ChessEngineConstants.Evaluation.BlackCheckMate)
                    return 0;

                return -1;
            }

            return 0;
        }
    }
}
