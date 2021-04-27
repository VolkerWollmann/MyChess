using System.Windows.Media;
using MyChess.Common;
using MyChessEngine;
using Color = MyChessEngineBase.Color;

namespace MyChess.Controls.Pieces
{
    /// <summary>
    /// Interaction logic for RookUserControl.xaml
    /// </summary>
    public partial class RookUserControl : IUserControlPiece
    {
        public RookUserControl()
        {
            InitializeComponent();
        }

        private readonly IPiece _Piece;

        public IPiece GetPiece()
        {
            return _Piece;
        }

        public RookUserControl(IPiece piece) : this()
        {
            _Piece = piece;

            Figure.Fill = _Piece.Color == Color.White ? 
                new SolidColorBrush(Colors.Khaki) : 
                new SolidColorBrush(Colors.Black);
        }
    }
}
