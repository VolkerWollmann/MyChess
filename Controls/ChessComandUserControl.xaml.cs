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

namespace MyChess.Controls
{
    /// <summary>
    /// Interaction logic for ChessComandUserControl.xaml
    /// </summary>
    public partial class ChessComandUserControl : UserControl
    {
        public ChessComandUserControl()
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

    }
}
