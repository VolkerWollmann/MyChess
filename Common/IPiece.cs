using System;
using System.Collections.Generic;
using System.Text;
using MyChess.Common;

namespace MyChess.Common
{
    interface IPiece
    {
        ChessConstants.Piece GetPieceType();

        ChessConstants.Color GetColor();
    }
}
