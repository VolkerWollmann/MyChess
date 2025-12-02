using MyChessEngineBase;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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

        public Knight(Color color, Position position, int promotionPly = -1, bool isMoved=false) : base(color, PieceType.Knight, position, isMoved, promotionPly)
        {

        }

        public Knight(Color color, string position, int promotionPly= -1, bool isMoved=false) : base(color, PieceType.Knight, new Position(position), isMoved, promotionPly)
        {

        }
    }
}
