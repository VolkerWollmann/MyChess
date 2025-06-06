using System.Diagnostics;
using MyChessEngineBase;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Color = {Color}  P={Position}")]
    public class Bishop : Piece
    {
        public override MoveList GetThreatenMoveList()
        {
            return GetMoveList();
        }


        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            // left, down
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = Position.GetDeltaPosition(-i, -i);
                if (newPosition == null)
                    break;

                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // left, up
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = Position.GetDeltaPosition(-i, i);
                if (newPosition == null)
                    break;

                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // right, down
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = Position.GetDeltaPosition(i, -i);
                if (newPosition == null)
                    break;

                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // right, up
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = Position.GetDeltaPosition(i, i);
                if (newPosition == null)
                    break;

                if (!AddPosition(moveList, newPosition))
                    break;

            }

            return moveList;
        }

        public Bishop(Color color, Position position) : base(color, PieceType.Bishop, position)
        {

        }
        public Bishop(Color color, string position) : base(color, PieceType.Bishop, new Position(position))
        {

        }
    }
}