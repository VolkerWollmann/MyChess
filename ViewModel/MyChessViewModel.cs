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

            RowDefinition menuRowDefinition = new RowDefinition();
            menuRowDefinition.Height = GridLength.Auto;
            GameGrid.RowDefinitions.Add(menuRowDefinition);

            RowDefinition chessBoRowDefinitionRowDefinition = new RowDefinition();
            GameGrid.RowDefinitions.Add(chessBoRowDefinitionRowDefinition);
           
            RowDefinition engineOutputRowDefinition = new RowDefinition();
            engineOutputRowDefinition.Height = new GridLength(100);
            GameGrid.RowDefinitions.Add(engineOutputRowDefinition);

            Menu = new ChessMenuUserControl();
            GameGrid.Children.Add(Menu);
            Grid.SetRow(Menu, 0);

            ChessBoard = new ChessBoardUserControl();
            GameGrid.Children.Add(ChessBoard);
            Grid.SetRow(ChessBoard, 1);

            EngineOutput = new EngineOutputControl();
            GameGrid.Children.Add(EngineOutput);
            Grid.SetRow(EngineOutput, 2);
        }
    }
}
