using System.Collections.Generic;
using System.Windows.Documents;
using MyChess.Common;
using MyChess.Model.Pieces;

namespace MyChess.Model
{
    public class Board
    {
        private readonly Piece[,] Pieces;

        public Board()
        {
            Pieces = new Piece[8, 8];
        }

        public Piece this[Position position]
        {
            get => Pieces[position.Row, position.Column];
            set
            {
                Pieces[position.Row, position.Column] = value;
                Pieces[position.Row, position.Column].Board = this;
                Pieces[position.Row, position.Column].Position = position;
            }
        }

        public bool IsValidPosition(Position position, ChessConstants.Color color)
        {
            // does not work for pawn
            if (position == null)
                return false;

            if (!position.IsValidPosition())
                return false;

            if (this[position] == null)
                return true;

            if (this[position].Color == color)
                return false;

            return true;
        }

        public List<Piece> GetAllPieces(ChessConstants.Color color)
        {
            List<Piece> pieces = new List<Piece>();
            Position.AllPositions().ForEach(position =>
            {
                if ((this[position] != null) && this[position].Color == color)
                    pieces.Add(this[position]);
            });

            return pieces;
        }

        public void Clear()
        {
            Position.AllPositions().ForEach(position => { this[position] = null; });
        }


        public Board Copy()
        {
            Board copy = new Board();

            Position.AllPositions().ForEach(position =>
            {
                copy[position] = null;
                if (this[position] != null)
                {
                    copy[position] = PieceFactory.Copy(this[position]);
                }
            });

            return copy;
        }
    }
}
