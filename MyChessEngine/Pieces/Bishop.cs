using MyChessEngineBase;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Color = {Color}  P={Position}")]
    public class Bishop : Piece
    {
        public override MoveList GetThreatenMoveList()
        {
            return GetMoveList(true);
        }

        public override MoveList GetMoveList()
        {
            return GetMoveList(false);
        }
        public MoveList GetMoveList(bool threat)
        {
            MoveList moveList = new MoveList();

            // left, down
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = Position.GetDeltaPosition(-i, -i);
                if (newPosition == null)
                    break;

                if (!AddPosition(moveList, newPosition, threat))
                    break;
            }

            // left, up
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = Position.GetDeltaPosition(-i, i);
                if (newPosition == null)
                    break;

                if (!AddPosition(moveList, newPosition, threat))
                    break;
            }

            // right, down
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = Position.GetDeltaPosition(i, -i);
                if (newPosition == null)
                    break;

                if (!AddPosition(moveList, newPosition, threat))
                    break;
            }

            // right, up
            for (int i = 1; i <= ChessEngineConstants.Length; i++)
            {
                Position newPosition = Position.GetDeltaPosition(i, i);
                if (newPosition == null)
                    break;

                if (!AddPosition(moveList, newPosition, threat))
                    break;

            }

            return moveList;
        }

        public Bishop(Color color, Position position, int promotionPly) : base(color, PieceType.Bishop, position, false, promotionPly)
        {

        }
        public Bishop(Color color, string position, int promotionPly) : base(color, PieceType.Bishop, new Position(position), false, promotionPly)
        {

        }
    }
}