﻿using System.Windows.Media;
using MyChess.Common;
using MyChessEngineBase;
using Color = MyChessEngineBase.Color;

namespace MyChess.Controls.Pieces
{
    /// <summary>
    /// Interaction logic for KingUserControl.xaml
    /// </summary>
    public partial class KingUserControl : IUserControlPiece
    {
        public KingUserControl()
        {
            InitializeComponent();
        }

        private readonly IPiece _Piece;
        public IPiece GetPiece()
        {
            return _Piece;
        }
        public KingUserControl(IPiece piece) : this()
        {
            _Piece = piece;

            Figure.Fill = _Piece.Color == Color.White ? 
                new SolidColorBrush(Colors.Khaki) : 
                new SolidColorBrush(Colors.Black);
        }
    }
}
