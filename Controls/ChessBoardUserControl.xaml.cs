using System;
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

namespace MyChess.Controls
{
    /// <summary>
    /// Interaction logic for ChessBoard.xaml
    /// </summary>
    public partial class ChessBoardUserControl : UserControl
    {
        public ChessBoardUserControl()
        {
            InitializeComponent();

            int row;
            int column;

            for (row = 0; row < 8; row++)
            {
                RowDefinition r = new RowDefinition();
                r.Height = new GridLength(1, GridUnitType.Star);
                ChessBoardGrid.RowDefinitions.Add(r);
            }

            for (column = 0; column < 8; column++)
            {
                ColumnDefinition c = new ColumnDefinition();
                c.Width = new GridLength(1, GridUnitType.Star);
                ChessBoardGrid.ColumnDefinitions.Add(c);
            }

            ColumnDefinition restColumn = new ColumnDefinition();
            restColumn.Width = GridLength.Auto;
            ChessBoardGrid.ColumnDefinitions.Add(restColumn);

            for ( row = 0; row < 8; row++)
            for (column = 0; column < 8; column++)
            {
                ChessFieldUserControl field = new ChessFieldUserControl();
                ChessBoardGrid.Children.Add(field);

                Grid.SetRow(field, row);
                Grid.SetColumn(field, column );
                
            }

            TextBlock tb = new TextBlock();
            tb.Text = "hase";
            ChessBoardGrid.Children.Add(tb);
            Grid.SetColumn(tb,8);
            Grid.SetRow(tb,0);
            Grid.SetRowSpan(tb,8);
        }
    }
}
