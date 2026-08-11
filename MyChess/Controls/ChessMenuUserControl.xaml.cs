using System;
using System.Windows;
using System.Windows.Controls;
using MyChess.Common;
using MyChess.Helper;

namespace MyChess.Controls
{
    /// <summary>
    /// Interaction logic for ChessMenu.xaml
    /// </summary>
    public partial class ChessMenuUserControl
    {
        private EventHandler<ChessMenuEventArgs> EventHandler;
        public ChessMenuUserControl()
        {
            InitializeComponent();
        }

        public void SetEventHandler(EventHandler<ChessMenuEventArgs> eventHandler)
        {
            EventHandler = eventHandler;
        }

        public void SetSelectedEngine(string engineTag)
        {
            ChessEngineCommand.IsChecked = engineTag == ChessGameConstants.ChessEngineCommand;
            IntegerChessEngineCommand.IsChecked = engineTag == ChessGameConstants.IntegerChessEngineCommand;
        }

        private void MenuCommand_Click(object sender, RoutedEventArgs e)
        {

            if (sender is MenuItem menuItem )
            {
                ChessMenuEventArgs chessMenuEventArgs = new ChessMenuEventArgs((string)menuItem.Tag);
                EventHandler?.Invoke(this, chessMenuEventArgs);
            }
            else
                throw new NotImplementedException("Not implemented");
        }
    }
}
