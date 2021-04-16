using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MyChessEngine
{
    /// <summary>
    /// ![CDATA[
    /// Order by Evaluation, Value
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
    [DebuggerDisplay("Situation:{Situation} Evaluation:{Evaluation} Value:{Value}")]
    public class BoardRating 
    {
        public Situation Situation;
        public Evaluation Evaluation;
        public int Value;




        ///                     + : this better, - : other better, = : equal
        ///                     other
        ///                     BlackMate   BlackStaleMate  Normal  WhiteStaleMate  WhiteMate
        ///     this
        ///     BlackMate       =           +               +       +               +
        ///     BlackStaleMate  -           =               -       =               +
        ///     Normal          -           +               value   +               +
        ///     WhiteStaleMate  -           =               -       =               +
        ///     WhiteMate       -           -               -       -               =
        static Dictionary<Tuple<Evaluation, Evaluation>, int> _whiteCompareResults =
            new Dictionary<Tuple<Evaluation, Evaluation>, int>()
            {
                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackCheckMate, Evaluation.BlackCheckMate), 0},
                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackStaleMate, Evaluation.BlackCheckMate), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.Normal,         Evaluation.BlackCheckMate), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteStaleMate, Evaluation.BlackCheckMate), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteCheckMate, Evaluation.BlackCheckMate), -1},

                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackCheckMate, Evaluation.BlackStaleMate), 1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackStaleMate, Evaluation.BlackStaleMate), 0},
                { new Tuple<Evaluation, Evaluation>(Evaluation.Normal,         Evaluation.BlackStaleMate), 1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteStaleMate, Evaluation.BlackStaleMate), 0},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteCheckMate, Evaluation.BlackStaleMate), -1},

                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackCheckMate, Evaluation.Normal), 1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackStaleMate, Evaluation.Normal), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.Normal,         Evaluation.Normal), 2},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteStaleMate, Evaluation.Normal), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteCheckMate, Evaluation.Normal), -1},

                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackCheckMate, Evaluation.WhiteStaleMate), 1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackStaleMate, Evaluation.WhiteStaleMate), 0},
                { new Tuple<Evaluation, Evaluation>(Evaluation.Normal,         Evaluation.WhiteStaleMate), 1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteStaleMate, Evaluation.WhiteStaleMate), 0},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteCheckMate, Evaluation.WhiteStaleMate), -1},

                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackCheckMate, Evaluation.WhiteCheckMate), 0},
                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackStaleMate, Evaluation.WhiteCheckMate), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.Normal,         Evaluation.WhiteCheckMate), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteStaleMate, Evaluation.WhiteCheckMate), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteCheckMate, Evaluation.WhiteCheckMate), -1},

            };

        /// <summary>
        /// Compare move in favor of white
        /// </summary>
        /// <param name="other"></param>
        // 1 : this better for white
        // 0 : equal
        // -1 : other better for white

        public int PartialCompareWhite (BoardRating other )
        {
            int result = _whiteCompareResults[new Tuple<Evaluation, Evaluation>(this.Evaluation, other.Evaluation)];
            if (result == 2)
            {
                if (this.Value > other.Value)
                    result = 1;
                if (this.Value < other.Value)
                    result = -1;
                if (this.Value == other.Value)
                    result = 0;
            }
            return result;
        }

        ///                     + : this better, - : other better, = : equal
        ///                     other
        ///                     BlackMate   BlackStaleMate  Normal  WhiteStaleMate  WhiteMate
        ///     this
        ///     BlackMate       =           -               -       -               -
        ///     BlackStaleMate  +           =               -       =               -
        ///     Normal          +           +               value   +               -
        ///     WhiteStaleMate  +           =               -       =               -
        ///     WhiteMate       +           +               +       +               =


        static Dictionary<Tuple<Evaluation, Evaluation>, int> _blackCompareResults =
            new Dictionary<Tuple<Evaluation, Evaluation>, int>()
            {
                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackCheckMate, Evaluation.BlackCheckMate), 0},
                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackStaleMate, Evaluation.BlackCheckMate), 1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.Normal,         Evaluation.BlackCheckMate), 1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteStaleMate, Evaluation.BlackCheckMate), 1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteCheckMate, Evaluation.BlackCheckMate), 1},

                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackCheckMate, Evaluation.BlackStaleMate), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackStaleMate, Evaluation.BlackStaleMate), 0},
                { new Tuple<Evaluation, Evaluation>(Evaluation.Normal,         Evaluation.BlackStaleMate), 1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteStaleMate, Evaluation.BlackStaleMate), 0},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteCheckMate, Evaluation.BlackStaleMate), 1},

                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackCheckMate, Evaluation.Normal), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackStaleMate, Evaluation.Normal), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.Normal,         Evaluation.Normal), 2},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteStaleMate, Evaluation.Normal), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteCheckMate, Evaluation.Normal), 1},

                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackCheckMate, Evaluation.WhiteStaleMate), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackStaleMate, Evaluation.WhiteStaleMate), 0},
                { new Tuple<Evaluation, Evaluation>(Evaluation.Normal,         Evaluation.WhiteStaleMate), 1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteStaleMate, Evaluation.WhiteStaleMate), 0},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteCheckMate, Evaluation.WhiteStaleMate), 1},

                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackCheckMate, Evaluation.WhiteCheckMate), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.BlackStaleMate, Evaluation.WhiteCheckMate), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.Normal,         Evaluation.WhiteCheckMate), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteStaleMate, Evaluation.WhiteCheckMate), -1},
                { new Tuple<Evaluation, Evaluation>(Evaluation.WhiteCheckMate, Evaluation.WhiteCheckMate), 0},

            };

        /// <summary>
        /// Compare move in favor of black
        /// </summary>
        /// <param name="other"></param>
        // 1 :  this better for black
        // 0 : equal
        // -1 : other better for black

        public int PartialCompareBlack(BoardRating other)
        {
            int result = _blackCompareResults[new Tuple<Evaluation, Evaluation>(this.Evaluation, other.Evaluation)];
            if (result == 2)
            {
                if (this.Value < other.Value)
                    result = 1;
                if (this.Value > other.Value)
                    result = -1;
                if (this.Value == other.Value)
                    result = 0;
            }
            return result;
        }

        /// <summary>
        /// Compare move in favor of color
        /// </summary>
        /// <param name="other">item to compare</param>
        /// <param name="color">seen from this color</param>
        // 1 :  this better for color
        // 0 : equal
        // -1 : other better for color
        public int PartialCompare(BoardRating other, Color color)
        {
            if (color == Color.White)
                return PartialCompareWhite(other);
            else
                return PartialCompareBlack(other);
        }

    }
}
