﻿using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyChess.Controls.Pieces;
using MyChessEngineBase;
using MShapes = System.Windows.Shapes;

namespace MyChess.Controls
{
    /// <summary>
    /// Interaction logic for ChessFieldUserControl.xaml
    /// </summary>
    public partial class ChessFieldUserControl
    {
        public int Row
        {
            get;
        }

        public int Column
        {
            get;
        }

        public Position GetPosition()
        {
            return new Position(Column, Row);
        }

        private readonly ChessBoardUserControl ChessBoardUserControl;

        public ChessFieldUserControl(int row, int column, ChessBoardUserControl chessBoardUserControl)
        {
            InitializeComponent();
            
            Row = row;
            Column = column;
            ChessBoardUserControl = chessBoardUserControl;


            SetFieldColor(FieldColor.Standard);

            MouseLeftButtonDown += OnMouseLeftButtonDown;

        }

        public void SetFieldColor(FieldColor fieldColor)
        {
            SolidColorBrush fieldSolidColorBrush = new SolidColorBrush(Colors.Bisque);
            switch (fieldColor)
            {
                case FieldColor.Standard:
                    fieldSolidColorBrush = (Row + Column) % 2 == 0 ? new SolidColorBrush(Colors.SandyBrown) : new SolidColorBrush(Colors.Bisque);
                    break;

                case FieldColor.Start:
                    fieldSolidColorBrush = new SolidColorBrush(Colors.LightGreen);
                    break;

                case FieldColor.End:
                    fieldSolidColorBrush = new SolidColorBrush(Colors.LightPink);
                    break;
            }

            Background = fieldSolidColorBrush;
        }

        public void SetPiece(IPiece piece)
        {
            if (FieldStackPanel.Children.Count > 0)
                FieldStackPanel.Children.RemoveAt(0);

            if (piece != null)
            {
                UserControl pieceUserControl = PieceUserControlFactory.CreatePieceUserControl(piece);
                FieldStackPanel.Children.Add(pieceUserControl);
                DockPanel.SetDock(pieceUserControl, Dock.Bottom);
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChessBoardUserControl.SetField(this);
        }
    }
}
