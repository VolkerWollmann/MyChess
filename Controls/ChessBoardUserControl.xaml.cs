using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using MyChess.Common;

namespace MyChess.Controls
{
    /// <summary>
    /// Interaction logic for ChessBoard.xaml
    /// </summary>
    public partial class ChessBoardUserControl : UserControl
    {
        ChessFieldUserControl[,] Field;

        private ChessComandUserControl ChessComandUserControl;

        private ChessFieldUserControl StartField=null;
        private ChessFieldUserControl EndField=null;


        public void SetField(ChessFieldUserControl field)
        {
            if (StartField == null)
            {
                StartField = field;
                StartField.SetFieldColor(ChessConstants.FieldColor.Start);

                ChessComandUserControl.SetStartField(  (char)(StartField.Column + 65) + (StartField.Row+1).ToString() );

            }
            else if (EndField == null)
            {
                EndField = field;
                EndField.SetFieldColor(ChessConstants.FieldColor.End);

                ChessComandUserControl.SetEndField((char)(EndField.Column + 65) + (EndField.Row+1).ToString());
            }
            else
            {
                StartField.SetFieldColor(ChessConstants.FieldColor.Standard);
                EndField.SetFieldColor(ChessConstants.FieldColor.Standard);

                ChessComandUserControl.SetStartField("");
                ChessComandUserControl.SetEndField("");

                StartField = null;
                EndField = null;
            }


        }

        public void SetPiece(int row, int column, IPiece piece)
        {
            Field[row,column].SetPiece(piece);
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

            ChessComandUserControl = new ChessComandUserControl();
            ChessBoardGrid.Children.Add(ChessComandUserControl);
            Grid.SetColumn(ChessComandUserControl, 8);
            Grid.SetRow(ChessComandUserControl, 0);
            Grid.SetRowSpan(ChessComandUserControl, 6);

        }
    }
}
