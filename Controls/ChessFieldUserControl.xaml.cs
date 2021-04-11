using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using MShapes = System.Windows.Shapes;

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
        }

        public void SetPiece()
        {
            MShapes.Rectangle rectangle = new MShapes.Rectangle();
            rectangle.Height = 20;
            rectangle.Width = 20;
            rectangle.Fill = System.Windows.Media.Brushes.SkyBlue;
            this.FieldGrid.Children.Add(rectangle);
        }
    }
}
