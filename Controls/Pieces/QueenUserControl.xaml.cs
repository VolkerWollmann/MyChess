using System.Windows.Controls;
using System.Windows.Media;
using MyChess.Common;

namespace MyChess.Controls.Pieces
{
    /// <summary>
    /// Interaction logic for QueenControl1.xaml
    /// </summary>
    public partial class QueenUserControl : UserControl, IUserControlPiece
    {
        public QueenUserControl()
        {
            InitializeComponent();
        }

        private readonly IPiece _Piece;
        public IPiece GetPiece()
        {
            return _Piece;
        }
        public QueenUserControl(IPiece piece) : this()
        {
            _Piece = piece;

            this.Figure.Fill = _Piece.GetColor() == ChessConstants.Color.White ? 
                new SolidColorBrush(Colors.Khaki) : 
                new SolidColorBrush(Colors.Black);
        }
    }
}
