using System;
using System.Collections.Generic;
using System.Text;

namespace MyIntegerChessEngine.Pieces
{
    internal class Knight : Piece
    {
        static readonly int[,] Delta = new int[,]
        {
            { -2, -1 }, { -2,  1 }, {  2, -1 }, { 2, 1 },
            { -1, -2 }, {  1, -2 }, { -1,  2 }, { 1, 2 }
        };

        internal MoveList GetMoveList(Board board, Position position)
        {
            var result = new MoveList();
            Piece knight = board.GetPiece(position);
            for (int i= 0; i < Delta.GetLength(0); i++)
            {
                Position targetPosition = position.GetDeltaPosition(Delta[i,0], Delta[i,1]);

                Piece pieceAtTarget = board.GetPiece(targetPosition);
                if (pieceAtTarget.PieceType == Constants.BoardBorder)
                    continue; // Out of bounds
                if (pieceAtTarget.Color == knight.Color)
                    continue; // Can't capture own piece
                
                result.Add(new Move(position, targetPosition, knight));
            }

            return result;
        }
    }
}
