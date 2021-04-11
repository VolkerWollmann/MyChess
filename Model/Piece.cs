using System;
using System.Collections.Generic;
using System.Text;
using Accessibility;
using MyChess.Common;


namespace MyChess.Model
{
    public class Piece : IPiece
    {
        private ChessConstants.Piece _Piece;
        private ChessConstants.Color _Color;
        public ChessConstants.Piece GetPieceType()
        {
            return _Piece;
        }

        public ChessConstants.Color GetColor()
        {
            return _Color;
        }

        public Piece(ChessConstants.Color color, ChessConstants.Piece piece)
        {
            _Color = color;
            _Piece = piece;
        }

    }
}
