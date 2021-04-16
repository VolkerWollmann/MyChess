using System.Windows.Controls;

namespace MyChess.ViewModel
{
    public class ViewModelInitialize
    {
        private static MyChessViewModel _myChessViewModel; 
        public static void InitializeViewModel(Grid grid)
        {
            _myChessViewModel = new MyChessViewModel(grid);
        }
    }
}

