using System;
using System.Collections.Generic;
using System.Text;

namespace MyIntegerChessEngine.Pieces
{
    internal class King
    {
        internal MoveList GetMoveList(Board board, Position position)
        {
            MoveList moveList = new MoveList();
            Piece king = board.GetPiece(position);

            int[,] directions = new int[,]
            {
                { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 },
                { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }
            };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int dx = directions[i, 0];
                int dy = directions[i, 1];
                Position targetPosition = position.GetDeltaPosition(dx, dy);

                Piece pieceAtTarget = board.GetPiece(targetPosition);
                if (pieceAtTarget.PieceType == Constants.BoardBorder)
                    continue; // Out of bounds
                if (pieceAtTarget.Color == king.Color)
                    continue; // Can't capture own piece

                moveList.Add(new Move(position, targetPosition, king));
            }

            return moveList;
        }
    }
}