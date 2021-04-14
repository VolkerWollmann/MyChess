using static MyChess.Common.ChessConstants;

namespace MyChess.Common
{
    /// <summary>
    /// ![CDATA[
    /// Order by Advantage, Value
    ///         WhiteMate,
    ///         WhiteChecked,
    ///         Normal,
    ///                 Value : > 0 better for White
    ///                         = 0 equal
    ///                         > 0 better for Black
    ///         BlackChecked,
    ///         BlackMate
    /// ]]>
    /// </summary>
    public class BoardRating
    {
        public Situation Situation;
        public Advantage Advantage;
        public int Value;
    }
}
