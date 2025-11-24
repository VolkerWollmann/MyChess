using System.Diagnostics;
using MyChessEngineBase;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Color = {Color} Position={Position}")]
    public class Knight : Piece
    {
        static readonly int[,] Delta = new int[,]
        {
            { -2, -1 }, { -2,  1 }, {  2, -1 }, { 2, 1 },
            { -1, -2 }, {  1, -2 }, { -1,  2 }, { 1, 2 }
        };

        public override MoveList GetThreatenMoveList()
        {
            return GetMoveList();
        }

        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            for (int i = 0; i < 8; i++)
            {
                Position newPosition = Position.GetDeltaPosition(Delta[i, 0], Delta[i, 1]);
                if (newPosition != null)
                    AddPosition(moveList, newPosition);
            }

            return moveList;
        }

        public Knight(Color color, Position position) : base(color, PieceType.Knight, position, false)
        {

        }

        public Knight(Color color, string position) : base(color, PieceType.Knight, new Position(position), false)
        {

        }
    }
}
