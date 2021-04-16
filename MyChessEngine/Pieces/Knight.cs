using System.Collections.Generic;
using System.Diagnostics;
using MyChess.Common;
using MyChessEngineCommon;


namespace MyChess.Model.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Knight : Piece
    {
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            int[,] delta = new int[,]
            {
                { -2, -1 }, { -2,  1 }, {  2, -1 }, { 2, 1 },
                { -1, -2 }, {  1, -2 }, { -1,  2 }, { 1, 2 }
            } ;

            for (int i = 0; i < 8; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(delta[i, 0], delta[i, 1]);
                AddPosition(moves, newPosition);
            }

            return moves;
        }

        public Knight(ChessEngineConstants.Color color) : base(color, ChessEngineConstants.Piece.Knight)
        {

        }
    }
}
