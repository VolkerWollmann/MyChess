using System;
using System.Collections.Generic;
using System.Text;
using MyChess.Common;


namespace MyChess.Model
{
    public class Piece : IPiece
    {
        public ChessConstants.Piece GetPieceType()
        {
            throw new NotImplementedException();
        }

        public ChessConstants.Color GetColor()
        {
            throw new NotImplementedException();
        }
    }
}
