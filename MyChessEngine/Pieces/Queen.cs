using MyChess.Common;
using System.Collections.Generic;
using System.Diagnostics;
using MyChessEngineCommon;

namespace MyChess.Model.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Queen : Piece
    {
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            // left
            for (int row = 1; row <= ChessEngineConstants.Length; row++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-row, 0);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // right
            for (int row = 1; row <= ChessEngineConstants.Length; row++)
            {
                Position newPosition = this.Position.GetDeltaPosition(row, 0);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // down
            for (int column = 1; column <= ChessEngineConstants.Length; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(0, -column);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // up
            for (int column = 1; column <= ChessEngineConstants.Length; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(0, column);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // left, down
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-i, -i);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // left, up
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-i, i);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // right, down
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(i, -i);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // right, up
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(i, i);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            return moves;
        }

        public Queen(ChessEngineConstants.Color color) : base(color, ChessEngineConstants.Piece.Queen)
        {

        }
    }
}