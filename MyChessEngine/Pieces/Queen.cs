using System.Diagnostics;
using MyChessEngineBase;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Color = {Color} Position={Position}")]
    public class Queen : Piece
    {
        public override MoveList GetThreatenMoveList()
        {
            return GetMoveList(true);
        }

        public override MoveList GetMoveList()
        {
            return GetMoveList(false);
        }
        public  MoveList GetMoveList(bool threat)
        {
            MoveList moveList = new MoveList();

            int localColumn = Position.Column;
            int localRow = Position.Row;
            // left
            for (int row = localRow - 1; row >= 0; row--)
            {
                Position newPosition = new Position( localColumn, row);
                if (!AddPosition(moveList, newPosition, threat))
                    break;
            }

            // right
            for (int row = localRow + 1; row < ChessEngineConstants.Length; row++)
            {
                Position newPosition = new Position(localColumn, row);
                if (!AddPosition(moveList, newPosition, threat))
                    break;
            }

            // down
            for (int column = localColumn - 1; column >= 0; column--)
            {
                Position newPosition = new Position( column, localRow);
                if (!AddPosition(moveList, newPosition, threat))
                    break;
            }

            // up
            for (int column = localColumn + 1; column < ChessEngineConstants.Length; column++)
            {
                Position newPosition = new Position( column, localRow);
                if (!AddPosition(moveList, newPosition, threat))
                    break;
            }

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

                if (!AddPosition(moveList, newPosition))
                    break;
            }

            return moveList;
        }

        public Queen(Color color, Position position) : base(color, PieceType.Queen, position, false)
        {

        }

        public Queen(Color color, string position) : base(color, PieceType.Queen, new Position(position), false)
        {

        }
    }
}