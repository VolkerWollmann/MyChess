using System.Windows;
using MyChess.ViewModel;

namespace MyChess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModelInitialize.InitializeViewModel(MyChessGameGrid);
        }
    }
}
