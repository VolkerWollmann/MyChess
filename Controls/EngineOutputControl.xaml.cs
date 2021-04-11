using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for EngineOutputControl.xaml
    /// </summary>
    public partial class EngineOutputControl : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public EngineOutputControl()
        {
            InitializeComponent();
        }

        public string Text
        {
            set
            { 
                OutputText.Text = value;
                NotifyPropertyChanged("OutputText");
            }

            get => OutputText.Text;
        }
    }
}
