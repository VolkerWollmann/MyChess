using System.Collections.Generic;
using System.Diagnostics;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Bishop : Piece
    {
        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

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
        public Bishop(Color color) : base(color, PieceType.Bishop)
        {

        }
    }
}