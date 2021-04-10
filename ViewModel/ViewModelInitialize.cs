using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace MyChess.ViewModel
{
    public class ViewModelInitialize
    {
        private static MyChessViewModel _MyChessViewModel; 
        public static void InitializeViewModel(Grid grid)
        {
            _MyChessViewModel = new MyChessViewModel(grid);
        }
    }
}

