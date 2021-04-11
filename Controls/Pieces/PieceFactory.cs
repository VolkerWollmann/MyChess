using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using MyChess.Common;

namespace MyChess.Controls.Pieces
{
    public class PieceFactory
    {
        public static UserControl CreatePiece(ChessConstants.Piece piece)
        {
            return new PawnUserControl();
        }
    }
}
