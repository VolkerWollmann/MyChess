using System;
using System.Collections.Generic;
using System.Text;
using MyChess.Common;

namespace MyChess.Model
{
    public class King : Piece
    {
        public King(ChessConstants.Color color) : base(color, ChessConstants.Piece.King)
        {

        }
    }
}
