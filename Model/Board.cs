﻿using System;
using System.Collections.Generic;
using System.Text;
using MyChess.Common;
using MyChess.Model.Pieces;

namespace MyChess.Model
{
    public class Board
    {
        private readonly Piece[,] board;

        public Board()
        {
            board = new Piece[8, 8];
        }

        public Piece this[Position position]
        {
            get => board[position.Row, position.Column];
            set => board[position.Row, position.Column] = value;
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