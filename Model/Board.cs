using System;
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
            if (position == null)
                return false;

            if (this[position].Color == color)
                return false;

            return true;
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
