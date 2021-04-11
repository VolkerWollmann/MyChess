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
        private EventHandler EventHandler;
        public ChessMenuUserControl()
        {
            InitializeComponent();
        }

        public void SetEventHandler(EventHandler eventHandler)
        {
            EventHandler = eventHandler;
        }

        private void MenuCommand_Click(object sender, RoutedEventArgs e)
        {

            if (sender is MenuItem menuItem )
            {
                if (menuItem.Name == ChessConstants.QuitCommand)
                    Application.Current.Shutdown();

                if (menuItem.Name == ChessConstants.Test1Comnand)
                    EventHandler?.Invoke(this, new EventArgs());
            }
            else
                throw new NotImplementedException("Not implemented");
        }

        internal void SetEventHandler()
        {
            throw new NotImplementedException();
        }
    }
}
