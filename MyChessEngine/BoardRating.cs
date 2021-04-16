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
    }

}
