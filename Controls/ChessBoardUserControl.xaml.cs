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

        public void SetPiece()
        {
            Field[0,0].SetPiece(ChessConstants.Piece.Pawn);
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
                ChessFieldUserControl field = new ChessFieldUserControl(row, column);

                Field[row, column] = field;

                ChessBoardGrid.Children.Add(field);

                Grid.SetRow(field, 7-row);
                Grid.SetColumn(field, column );
                
            }

            TextBlock tb = new TextBlock {Text = "rabbit"};
            ChessBoardGrid.Children.Add(tb);
            Grid.SetColumn(tb,8);
            Grid.SetRow(tb,0);
            Grid.SetRowSpan(tb,8);
        }
    }
}
