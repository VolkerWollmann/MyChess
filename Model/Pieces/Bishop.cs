using System;
using System.Collections.Generic;
using System.Text;
using MyChess.Common;

namespace MyChess.Model
{
    public class Bishop : Piece
    {
        public Bishop(ChessConstants.Color color) : base(color, ChessConstants.Piece.Bishop)
        {

        }
    }
}