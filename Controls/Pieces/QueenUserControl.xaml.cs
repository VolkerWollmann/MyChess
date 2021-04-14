﻿using System.Windows.Media;
using MyChess.Common;

namespace MyChess.Controls.Pieces
{
    /// <summary>
    /// Interaction logic for QueenControl1.xaml
    /// </summary>
    public partial class QueenUserControl : IUserControlPiece
    {
        public QueenUserControl()
        {
            InitializeComponent();
        }

        private readonly IPiece _Piece;
        public IPiece GetPiece()
        {
            return _Piece;
        }
        public QueenUserControl(IPiece piece) : this()
        {
            _Piece = piece;

            Figure.Fill = _Piece.Color == ChessConstants.Color.White ? 
                new SolidColorBrush(Colors.Khaki) : 
                new SolidColorBrush(Colors.Black);
        }
    }
}
