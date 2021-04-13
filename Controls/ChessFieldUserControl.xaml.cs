using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyChess.Common;
using MyChess.Controls.Pieces;
using MShapes = System.Windows.Shapes;

namespace MyChess.Controls
{
    /// <summary>
    /// Interaction logic for ChessFieldUserControl.xaml
    /// </summary>
    public partial class ChessFieldUserControl
    {
        public int Row
        {
            get;
        }

        public int Column
        {
            get;
        }

        private readonly ChessBoardUserControl ChessBoardUserControl;

        public ChessFieldUserControl(int row, int column, ChessBoardUserControl chessBoardUserControl)
        {
            InitializeComponent();
            
            Row = row;
            Column = column;
            ChessBoardUserControl = chessBoardUserControl;


            SetFieldColor(ChessConstants.FieldColor.Standard);

            this.MouseLeftButtonDown += OnMouseLeftButtonDown;

        }

        public void SetFieldColor(ChessConstants.FieldColor fieldColor)
        {
            SolidColorBrush fieldSolidColorBrush = new SolidColorBrush(Colors.Bisque);
            switch (fieldColor)
            {
                case ChessConstants.FieldColor.Standard:
                    fieldSolidColorBrush = (Row + Column) % 2 == 0 ? new SolidColorBrush(Colors.SandyBrown) : new SolidColorBrush(Colors.Bisque);
                    break;

                case ChessConstants.FieldColor.Start:
                    fieldSolidColorBrush = new SolidColorBrush(Colors.LightGreen);
                    break;

                case ChessConstants.FieldColor.End:
                    fieldSolidColorBrush = new SolidColorBrush(Colors.LightPink);
                    break;
            }

            this.Background = fieldSolidColorBrush;
        }

        public void SetPiece(IPiece piece)
        {
            if (this.FieldStackPanel.Children.Count > 0)
                this.FieldStackPanel.Children.RemoveAt(0);

            if (piece != null)
            {
                UserControl pieceUserControl = PieceUserControlFactory.CreatePieceUserControl(piece);
                this.FieldStackPanel.Children.Add(pieceUserControl);
                DockPanel.SetDock(pieceUserControl, Dock.Bottom);
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChessBoardUserControl.SetField(this);
        }
    }
}
