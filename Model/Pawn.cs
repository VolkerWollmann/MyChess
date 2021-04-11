using System;
using System.Collections.Generic;
using System.Text;
using MyChess.Common;

namespace MyChess.Model
{
    public class Pawn : Piece
    {
        public Pawn(ChessConstants.Color color): base(color, ChessConstants.Piece.Pawn)
        {

        }
    }
}
