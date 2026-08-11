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

        /// Shows/hides the "Berechne..." indicator while the engine is searching.
        public bool Calculating
        {
            set => CalculatingText.Visibility =
                value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            get => CalculatingText.Visibility == System.Windows.Visibility.Visible;
        }

        /// Strongest line (move sequence) of the last engine calculation.
        public string Line
        {
            set
            {
                LineText.Text = value;
                NotifyPropertyChanged("LineText");
            }

            get => LineText.Text;
        }
    }
}
