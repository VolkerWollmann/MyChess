﻿using System.Diagnostics;


namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Knight : Piece
    {
        static int[,] delta = new int[,]
        {
            { -2, -1 }, { -2,  1 }, {  2, -1 }, { 2, 1 },
            { -1, -2 }, {  1, -2 }, { -1,  2 }, { 1, 2 }
        };

        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            for (int i = 0; i < 8; i++)
            {
                Position newPosition = this.Position.GetDeltaPosition(delta[i, 0], delta[i, 1]);
                AddPosition(moveList, newPosition);
            }

            return moveList;
        }

        public override int GetWeight()
        {
            return (Color == Color.White) ? ChessEngineConstants.Knight: -ChessEngineConstants.Knight;
        }

        public Knight(Color color) : base(color, PieceType.Knight)
        {

        }
    }
}
