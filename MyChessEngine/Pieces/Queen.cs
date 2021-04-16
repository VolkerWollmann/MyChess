using System.Collections.Generic;
using System.Diagnostics;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Queen : Piece
    {
        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            // left
            for (int row = 1; row <= ChessEngineConstants.Length; row++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-row, 0);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // right
            for (int row = 1; row <= ChessEngineConstants.Length; row++)
            {
                Position newPosition = this.Position.GetDeltaPosition(row, 0);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // down
            for (int column = 1; column <= ChessEngineConstants.Length; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(0, -column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // up
            for (int column = 1; column <= ChessEngineConstants.Length; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(0, column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // left, down
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-i, -i);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // left, up
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-i, i);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // right, down
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(i, -i);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // right, up
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(i, i);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            return moveList;
        }

        public Queen(Color color) : base(color, PieceType.Queen)
        {

        }
    }
}