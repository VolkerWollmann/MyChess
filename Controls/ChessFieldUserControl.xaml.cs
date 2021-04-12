using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MyChess.Common;
using MyChess.Controls.Pieces;
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

        public void SetPiece(IPiece piece)
        {
            if (this.FieldStackPanel.Children.Count > 0)
                this.FieldStackPanel.Children.RemoveAt(0);

            if (piece != null)
            {
                UserControl pieceUserControl = PieceFactory.CreatePiece(piece);
                this.FieldStackPanel.Children.Add(pieceUserControl);
                DockPanel.SetDock(pieceUserControl, Dock.Bottom);
            }
        }
    }
}
