﻿using System.Collections.Generic;
using System.Diagnostics;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Bishop : Piece
    {
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            // left, down
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-i, -i);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition, this));
            }

            // left, up
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-i, i);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition, this));
            }

            // right, down
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(i, -i);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition, this));
            }

            // right, up
            for (int i = 1; i <= ChessConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(i, i);
                if (!this.Board.IsValidPosition(newPosition, this.Color))
                    break;
                moves.Add(new Move(this.Position, newPosition, this));
            }

            return moves;
        }
        public Bishop(ChessConstants.Color color) : base(color, ChessConstants.Piece.Bishop)
        {

        }
    }
}