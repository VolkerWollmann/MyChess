using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using MyChess.Controls;

namespace MyChess.ViewModel
{
    public class MyChessViewModel
    {
        private readonly Grid ChessGrid;

        private readonly ChessMenuUserControl Menu;

        public MyChessViewModel(Grid chessGrid)
        {
            ChessGrid = chessGrid;
            
            Menu = new ChessMenuUserControl();
            ChessGrid.Children.Add(Menu);
        }
    }
}
