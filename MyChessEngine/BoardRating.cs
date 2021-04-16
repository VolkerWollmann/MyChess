using System;
using System.Collections.Generic;
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
    public class BoardRating 
    {
        public Situation Situation;
        public Evaluation Evaluation;
        public int Value;

        private enum CompareResult
        {
            Whitebetter,
            Equal,
            Blackbetter,
            Value
        }

        ///                     + : this better, - : other better, = : equal
        ///                     other
        ///                     BlackMate   BlackStaleMate  Normal  WhiteStaleMate  WhiteMate
        ///     this
        ///     BlackMate       =           +               +       +               +
        ///     BlackStaleMate  -           =               -       =               +
        ///     Normal          -           +               value   +               +
        ///     WhiteStaleMate  -           =               -       =               +
        ///     WhiteMate       -           -               -       -               =

        /// <summary>
        /// Compare move in favor of white
        /// </summary>
        /// <param name="other"></param>
        // 1 : white better
        // 0 : equal
        // -1 : black better 0 this > other

        public int PartialCompareWhite (BoardRating other )
        {
            
            return 0;
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


        /// <summary>
        /// Compare move in favor of black
        /// </summary>
        /// <param name="other"></param>
        // 1 : white better
        // 0 : equal
        // -1 : black better 0 this > other

        public int PartialCompareBlack(BoardRating other)
        {
 
            return 0;
        }

        public int PartialCompare(BoardRating other, Color color)
        {
            if (color == Color.White)
                return PartialCompareWhite(other);
            else
                return PartialCompareBlack(other);
        }

    }
}
