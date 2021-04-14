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
    }
}
