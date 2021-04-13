﻿using System.Windows.Media;
using MyChess.Common;

namespace MyChess.Controls.Pieces
{
    /// <summary>
    /// Interaction logic for KnightUserControl.xaml
    /// </summary>
    public partial class KnightUserControl : IUserControlPiece
    {
        public KnightUserControl()
        {
            InitializeComponent();
        }

        private readonly IPiece _Piece;
        public IPiece GetPiece()
        {
            return _Piece;
        }
        public KnightUserControl(IPiece piece) : this()
        {
            _Piece = piece;

            Figure.Fill = _Piece.GetColor() == ChessConstants.Color.White ? new SolidColorBrush(Colors.Khaki) : new SolidColorBrush(Colors.Black);
        }
    }
}
