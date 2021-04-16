using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyChess.Common;
using MyChess.Controls.Pieces;
using MyChessEngineCommon;
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

        public Position GetPosition()
        {
            return new Position(Row, Column);
        }

        private readonly ChessBoardUserControl ChessBoardUserControl;

        public ChessFieldUserControl(int row, int column, ChessBoardUserControl chessBoardUserControl)
        {
            InitializeComponent();
            
            Row = row;
            Column = column;
            ChessBoardUserControl = chessBoardUserControl;


            SetFieldColor(ChessEngineConstants.FieldColor.Standard);

            MouseLeftButtonDown += OnMouseLeftButtonDown;

        }

        public void SetFieldColor(ChessEngineConstants.FieldColor fieldColor)
        {
            SolidColorBrush fieldSolidColorBrush = new SolidColorBrush(Colors.Bisque);
            switch (fieldColor)
            {
                case ChessEngineConstants.FieldColor.Standard:
                    fieldSolidColorBrush = (Row + Column) % 2 == 0 ? new SolidColorBrush(Colors.SandyBrown) : new SolidColorBrush(Colors.Bisque);
                    break;

                case ChessEngineConstants.FieldColor.Start:
                    fieldSolidColorBrush = new SolidColorBrush(Colors.LightGreen);
                    break;

                case ChessEngineConstants.FieldColor.End:
                    fieldSolidColorBrush = new SolidColorBrush(Colors.LightPink);
                    break;
            }

            Background = fieldSolidColorBrush;
        }

        public void SetPiece(IPiece piece)
        {
            if (FieldStackPanel.Children.Count > 0)
                FieldStackPanel.Children.RemoveAt(0);

            if (piece != null)
            {
                UserControl pieceUserControl = PieceUserControlFactory.CreatePieceUserControl(piece);
                FieldStackPanel.Children.Add(pieceUserControl);
                DockPanel.SetDock(pieceUserControl, Dock.Bottom);
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChessBoardUserControl.SetField(this);
        }
    }
}
