using System.Diagnostics;
using MyChessEngineBase;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Queen : Piece
    {
        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            // left
            for (int row = Position.Row - 1; row >= 0; row--)
            {
                Position newPosition = new Position(row, Position.Column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // right
            for (int row = Position.Row + 1; row < ChessEngineConstants.Length; row++)
            {
                Position newPosition = new Position(row, Position.Column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // down
            for (int column = Position.Column - 1; column >= 0; column--)
            {
                Position newPosition = new Position(Position.Row, column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // up
            for (int column = Position.Column + 1; column < ChessEngineConstants.Length; column++)
            {
                Position newPosition = new Position(Position.Row, column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

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

        public Queen(Color color) : base(color, PieceType.Queen)
        {

        }
    }
}