using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using MyChess.Common;
using MyChess.Model;

namespace MyChess.Controls.Pieces
{
    public class PieceFactory
    {
        public static UserControl CreatePiece(IPiece piece)
        {
            return new PawnUserControl(piece);
        }
    }
}
