using System;
using System.Collections.Generic;
using System.Text;

namespace MyIntegerChessEngine.Pieces
{
    internal class Rook
    {
        internal MoveList GetMoveList(Board board, Position position)
        {
            var result = new MoveList();
            Piece rook = board.GetPiece(position);

            int[,] directions = new int[,]
            {
                { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }
            };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int dx = directions[i, 0];
                int dy = directions[i, 1];
                Position targetPosition = position.GetDeltaPosition(dx, dy);

                while (true)
                {
                    Piece pieceAtTarget = board.GetPiece(targetPosition);
                    if (pieceAtTarget.PieceType == Constants.BoardBorder)
                        break; // Out of bounds
                    if (pieceAtTarget.Color == rook.Color)
                        break; // Can't capture own piece

                    result.Add(new Move(position, targetPosition, rook));

                    if (pieceAtTarget.Color != Constants.NoPiece)
                        break; // Capture opponent piece and stop

                    targetPosition = targetPosition.GetDeltaPosition(dx, dy);
                }
            }

            return result;
        }
    }
}
