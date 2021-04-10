using System.Resources;
using System.Windows;
using System.Windows.Controls;
using MyChess.Controls;

namespace MyChess.ViewModel
{
    public class MyChessViewModel
    {
        private readonly Grid GameGrid;

        private readonly ChessMenuUserControl Menu;

        private readonly ChessBoardUserControl ChessBoard;

        private readonly EngineOutputControl EngineOutput;

        public MyChessViewModel(Grid gameGrid)
        {
            GameGrid = gameGrid;
            RowDefinition r1 = new RowDefinition();
            GameGrid.RowDefinitions.Add(r1);
            RowDefinition r2 = new RowDefinition();
            r2.Height = new GridLength(100);
            GameGrid.RowDefinitions.Add(r2);

            Menu = new ChessMenuUserControl();
            GameGrid.Children.Add(Menu);

            ChessBoard = new ChessBoardUserControl();
            GameGrid.Children.Add(ChessBoard);
            Grid.SetRow(ChessBoard, 0);

            EngineOutput = new EngineOutputControl();
            GameGrid.Children.Add(EngineOutput);
            Grid.SetRow(EngineOutput, 1);
        }
    }
}
