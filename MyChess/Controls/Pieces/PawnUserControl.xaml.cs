using System.Windows.Media;
using MyChess.Common;
using MyChessEngine;
using Color = MyChessEngine.Color;

namespace MyChess.Controls.Pieces
{
    /// <summary>
    /// Interaction logic for PawnUserControl.xaml
    /// </summary>
    public partial class PawnUserControl : IUserControlPiece
    {
        public PawnUserControl()
        {
            InitializeComponent();
        }

        private readonly IPiece _Piece;

        public IPiece GetPiece()
        {
            return _Piece;
        }

        public PawnUserControl(IPiece piece):this()
        {
            _Piece = piece;

            Figure.Fill = _Piece.Color == Color.White ? 
                new SolidColorBrush(Colors.Khaki) : 
                new SolidColorBrush(Colors.Black);
        }
    }
}
