using System.Windows.Controls;
using System.Windows.Media;

namespace MyChess.Controls
{
    /// <summary>
    /// Interaction logic for ChessFieldUserControl.xaml
    /// </summary>
    public partial class ChessFieldUserControl : UserControl
    {
        public int Row
        {
            get;
        }

        public int Column
        {
            get;
        }
        public ChessFieldUserControl(int row, int column)
        {
            InitializeComponent();
            Row = row;
            Column = column;

            this.Background = (Row + Column) % 2 == 0 ? new SolidColorBrush(Colors.SandyBrown) : new SolidColorBrush(Colors.Bisque);

            this.Caption.Text = ((char)(Row+65)).ToString() + (Column+1).ToString();
        }
    }
}
