using MyChessEngine.Pieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyChessEngine
{
    [DebuggerDisplay("{PositionString} : {Piece}  Threat:{Threat}")]
    public class Field
    {
        public Piece Piece;
        public bool Threat = false;
        public string PositionString {  get; private set; }

        public Field(string positionString, Piece piece)
        {
            Piece = piece;
            PositionString = positionString;
        }

        public Field(string positionString)
        {
            Piece = null;
            PositionString = positionString;
        }
    }
}
