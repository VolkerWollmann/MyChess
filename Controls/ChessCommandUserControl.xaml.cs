using System;
using System.Windows;
using System.Windows.Controls;
using MyChess.Helper;

namespace MyChess.Controls
{
    /// <summary>
    /// Interaction logic for ChessCommandUserControl.xaml
    /// </summary>
    public partial class ChessCommandUserControl
    {
        private EventHandler<ChessMenuEventArgs> EventHandler;
        public ChessCommandUserControl()
        {
            InitializeComponent();
        }

        public void SetStartField(string text)
        {
            this.StartField.Text = text;
        }

        public void SetEndField(string text)
        {
            this.EndField.Text = text;
        }

        public void SetEventHandler(EventHandler<ChessMenuEventArgs> eventHandler)
        {
            EventHandler = eventHandler;
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button) sender;
            ChessMenuEventArgs chessMenuEventArgs = new ChessMenuEventArgs((string)b.Tag);
            EventHandler(null, chessMenuEventArgs);
        }
    }
}
