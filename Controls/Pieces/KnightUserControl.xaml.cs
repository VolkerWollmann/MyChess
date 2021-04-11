﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyChess.Common;

namespace MyChess.Controls.Pieces
{
    /// <summary>
    /// Interaction logic for KnightUserControl.xaml
    /// </summary>
    public partial class KnightUserControl : UserControl, IUserControlPiece
    {
        public KnightUserControl()
        {
            InitializeComponent();
        }

        private IPiece _Piece;
        public IPiece GetPiece()
        {
            return _Piece;
        }
        public KnightUserControl(IPiece piece) : this()
        {
            _Piece = piece;

            if (_Piece.GetColor() == ChessConstants.Color.White)
                this.Figure.Fill = new SolidColorBrush(Colors.Khaki);
            else
                this.Figure.Fill = new SolidColorBrush(Colors.Black);
        }
    }
}
