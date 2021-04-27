using System.Diagnostics;
using MyChessEngineBase;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Knight : Piece
    {
        static readonly int[,] Delta = new int[,]
        {
            { -2, -1 }, { -2,  1 }, {  2, -1 }, { 2, 1 },
            { -1, -2 }, {  1, -2 }, { -1,  2 }, { 1, 2 }
        };

        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            for (int i = 0; i < 8; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(Delta[i, 0], Delta[i, 1]);
                if (newPosition != null)
                    AddPosition(moveList, newPosition);
            }

            return moveList;
        }

        public Knight(Color color) : base(color, PieceType.Knight)
        {

        }
    }
}
