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
            StartField.Text = text;
        }

        public void SetEndField(string text)
        {
            EndField.Text = text;
        }

        /// Search depth (plies) from the input field, clamped to 1-8;
        /// 0 if the field holds no usable number (caller falls back to the engine default).
        public int GetDepth()
        {
            if (!int.TryParse(DepthField.Text, out int depth) || depth < 1)
                return 0;

            return Math.Min(depth, 8);
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
