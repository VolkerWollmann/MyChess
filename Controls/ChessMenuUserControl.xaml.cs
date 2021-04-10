using System;
using System.Windows;
using System.Windows.Controls;
using MyChess.Constants;

namespace MyChess.Controls
{
    /// <summary>
    /// Interaction logic for ChessMenu.xaml
    /// </summary>
    public partial class ChessMenuUserControl : UserControl
    {
        public ChessMenuUserControl()
        {
            InitializeComponent();
        }

        private void MenuCommand_Click(object sender, RoutedEventArgs e)
        {

            if (sender is MenuItem mi )
            {
                if ( mi.Name == ChessConstants.QuitCommand)
                    Application.Current.Shutdown();
            }
            else
                throw new NotImplementedException("Not implemented");
        }
    }
}
