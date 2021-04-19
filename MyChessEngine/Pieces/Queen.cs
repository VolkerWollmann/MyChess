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
            for (int row = this.Position.Row - 1; row >= 0; row--)
            {
                Position newPosition = new Position(row, this.Position.Column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // right
            for (int row = this.Position.Row + 1; row <= ChessEngineConstants.Length; row++)
            {
                Position newPosition = new Position(row, this.Position.Column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // down
            for (int column = this.Position.Column - 1; column >= 0; column--)
            {
                Position newPosition = new Position(this.Position.Row, column);
                if (!AddPosition(moveList, newPosition))
                    break;
            }

            // up
            for (int column = this.Position.Column + 1; column <= ChessEngineConstants.Length; column++)
            {
                Position newPosition = new Position(this.Position.Row, column);
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

        public override int GetWeight()
        {
            return (Color == Color.White) ? ChessEngineConstants.Queen : -ChessEngineConstants.Queen;
        }
        public Queen(Color color) : base(color, PieceType.Queen)
        {

        }
    }
}