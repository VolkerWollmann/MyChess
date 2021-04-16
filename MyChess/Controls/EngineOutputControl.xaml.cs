using System.ComponentModel;

namespace MyChess.Controls
{
    /// <summary>
    /// Interaction logic for EngineOutputControl.xaml
    /// </summary>
    public partial class EngineOutputControl
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
