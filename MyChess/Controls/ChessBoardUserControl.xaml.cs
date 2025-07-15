using System;
using System.Windows;
using System.Windows.Controls;
using MyChess.Helper;
using MyChessEngineBase;

namespace MyChess.Controls
{
    /// <summary>
    /// Interaction logic for ChessBoard.xaml
    /// </summary>
    public partial class ChessBoardUserControl
    {
        readonly ChessFieldUserControl[,] Field;

        private readonly ChessCommandUserControl ChessCommandUserControl;

        private ChessFieldUserControl Start;
        private ChessFieldUserControl End;

        public Move GetMove()
        {
            if (Start == null || End==null)
                return null;

            if ((Start.Row == 0) && (Start.Column == 4) &&
                (End.Row == 0) && (End.Column == 6))
            {
                return new Move(Start.GetPosition(), End.GetPosition(), null, MoveType.WhiteCastle);
            }
            else if ((Start.Row == 0) && (Start.Column == 4) &&
                         (End.Row == 0) && (End.Column == 3))
            {
                return new Move(Start.GetPosition(), End.GetPosition(), null, MoveType.WhiteCastleLong);
            }
            else
                return new Move(Start.GetPosition(), End.GetPosition(), null);
        }

        public void SetField(ChessFieldUserControl field)
        {
            bool reset = true;

            if (field != null)
            {
                if (Start == null)
                {
                    Start = field;
                    Start.SetFieldColor(FieldColor.Start);

                    ChessCommandUserControl.SetStartField((char) (Start.Column + 65) +
                                                         (Start.Row + 1).ToString());
                    reset = false;
                }
                else if (End == null)
                {
                    End = field;
                    End.SetFieldColor(FieldColor.End);

                    ChessCommandUserControl.SetEndField((char) (End.Column + 65) + (End.Row + 1).ToString());

                    reset = false;
                }

            }
 
            if ( reset) 
            {
                Start?.SetFieldColor(FieldColor.Standard);
                End?.SetFieldColor(FieldColor.Standard);

                ChessCommandUserControl.SetStartField("");
                ChessCommandUserControl.SetEndField("");

                Start = null;
                End = null;
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
