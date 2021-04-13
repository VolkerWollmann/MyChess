using System.Windows.Controls;
using System.Windows.Media;
using MyChess.Common;

namespace MyChess.Controls.Pieces
{
    /// <summary>
    /// Interaction logic for BishopUserControl.xaml
    /// </summary>
    public partial class BishopUserControl : UserControl, IUserControlPiece
    {
        public BishopUserControl()
        {
            InitializeComponent();
        }

        private readonly IPiece _Piece;
        public IPiece GetPiece()
        {
            return _Piece;
        }
        public BishopUserControl(IPiece piece) : this()
        {
            _Piece = piece;

            this.Figure.Fill = _Piece.GetColor() == ChessConstants.Color.White ? 
                new SolidColorBrush(Colors.Khaki) : 
                new SolidColorBrush(Colors.Black);
        }
    }
}
