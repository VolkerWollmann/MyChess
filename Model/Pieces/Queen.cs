using System;
using System.Collections.Generic;
using System.Text;
using MyChess.Common;

namespace MyChess.Model
{
    public class Queen : Piece
    {
        public Queen(ChessConstants.Color color) : base(color, ChessConstants.Piece.Queen)
        {

        }
    }
}