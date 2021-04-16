using System;
using System.Windows;
using System.Windows.Controls;
using MyChess.Helper;
using MyChessEngine;

namespace MyChess.Controls
{
    /// <summary>
    /// Interaction logic for ChessBoard.xaml
    /// </summary>
    public partial class ChessBoardUserControl
    {
        readonly ChessFieldUserControl[,] Field;

        private readonly ChessCommandUserControl ChessCommandUserControl;

        private ChessFieldUserControl StartField;
        private ChessFieldUserControl EndField;

        public Move GetMove()
        {
            if (StartField == null || EndField==null)
                return null;

            return new Move(StartField.GetPosition(), EndField.GetPosition(), null);
        }

        public void SetField(ChessFieldUserControl field)
        {
            bool reset = true;

            if (field != null)
            {
                if (StartField == null)
                {
                    StartField = field;
                    StartField.SetFieldColor(FieldColor.Start);

                    ChessCommandUserControl.SetStartField((char) (StartField.Column + 65) +
                                                         (StartField.Row + 1).ToString());
                    reset = false;
                }
                else if (EndField == null)
                {
                    EndField = field;
                    EndField.SetFieldColor(FieldColor.End);

                    ChessCommandUserControl.SetEndField((char) (EndField.Column + 65) + (EndField.Row + 1).ToString());

                    reset = false;
                }

            }
 
            if ( reset) 
            {
                StartField?.SetFieldColor(FieldColor.Standard);
                EndField?.SetFieldColor(FieldColor.Standard);

                ChessCommandUserControl.SetStartField("");
                ChessCommandUserControl.SetEndField("");

                StartField = null;
                EndField = null;
            }

        }


        public void SetEventHandler(EventHandler<ChessMenuEventArgs> eventHandler)
        {
            ChessCommandUserControl.SetEventHandler(eventHandler);
        }

        public void SetPiece(Position position, IPiece piece)
        {
            Field[position.Row,position.Column].SetPiece(piece);
        }
            
        public ChessBoardUserControl()
        {
            InitializeComponent();

            int row;
            int column;

            for (row = 0; row < 8; row++)
            {
                RowDefinition r = new RowDefinition {Height = new GridLength(1, GridUnitType.Star)};
                ChessBoardGrid.RowDefinitions.Add(r);
            }

            for (column = 0; column < 8; column++)
            {
                ColumnDefinition c = new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)};
                ChessBoardGrid.ColumnDefinitions.Add(c);
            }

            ColumnDefinition restColumn = new ColumnDefinition {Width = GridLength.Auto};
            ChessBoardGrid.ColumnDefinitions.Add(restColumn);

            Field = new ChessFieldUserControl[8, 8];

            for ( row = 0; row < 8; row++)
            for (column = 0; column < 8; column++)
            {
                ChessFieldUserControl field = new ChessFieldUserControl(row, column, this );

                Field[row, column] = field;

                ChessBoardGrid.Children.Add(field);

                Grid.SetRow(field, 7-row);
                Grid.SetColumn(field, column );
                
            }

            ChessCommandUserControl = new ChessCommandUserControl();
            ChessBoardGrid.Children.Add(ChessCommandUserControl);
            Grid.SetColumn(ChessCommandUserControl, 8);
            Grid.SetRow(ChessCommandUserControl, 0);
            Grid.SetRowSpan(ChessCommandUserControl, 6);

        }
    }
}
