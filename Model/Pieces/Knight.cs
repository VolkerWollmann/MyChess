using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Windows.Documents;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    public class Knight : Piece
    {
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            int[,] delta = new int[8,2]
            {
                { -2, -1 }, { -2,  1 }, {  2, -1 }, { 2, 1 },
                { -1, -2 }, {  1, -2 }, { -1,  2 }, { 1, 2 }
            } ;

            for (int i = 0; i < 8; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(delta[i, 0], delta[i, 1]);

                if (this.Board.IsValidPosition(newPosition, this.Color))
                    moves.Add(new Move(this.Position, newPosition, this));
            }

            return moves;
        }

        public Knight(ChessConstants.Color color) : base(color, ChessConstants.Piece.Knight)
        {

        }
    }
}
