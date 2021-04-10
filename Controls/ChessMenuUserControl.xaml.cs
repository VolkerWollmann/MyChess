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
