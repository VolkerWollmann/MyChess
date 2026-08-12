using System;
using System.Collections.Generic;
using System.Text;

namespace MyTranspositionChessEngine.Pieces
{
    internal static class Bishop
    {
        internal static MoveList GetThreatenMoveList(Board board, Position position)
        {
            return GetMoveList(board, position, true);
        }

        internal static MoveList GetMoveList(Board board, Position position)
        {
            return GetMoveList(board, position, false);
        }

        private static MoveList GetMoveList(Board board, Position position, bool threat)
        {
            MoveList moveList = new MoveList();
            Piece bishop = board.GetPiece(position);

            int[,] directions = Constants.DiagonalDirections;

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int dx = directions[i, 0];
                int dy = directions[i, 1];
                Position targetPosition = position.GetDeltaPosition(dx, dy);

                while (true)
                {
                    Piece pieceAtTarget = board.GetPiece(targetPosition);
                    if (pieceAtTarget.IsBorder)
                        break; // Out of bounds
                    if (!pieceAtTarget.IsEmpty && pieceAtTarget.IntColor == bishop.IntColor)
                        break; // Can't capture own piece

                    moveList.Add(new Move(position, targetPosition, bishop));

                    // Threat rays pass through the enemy king so the fields behind it stay threatened
                    if (!pieceAtTarget.IsEmpty && !(threat && pieceAtTarget.PieceType == Constants.King))
                        break; // Capture opponent piece and stop

                    targetPosition = targetPosition.GetDeltaPosition(dx, dy);
                }
            }

            return moveList;
        }
    }
}