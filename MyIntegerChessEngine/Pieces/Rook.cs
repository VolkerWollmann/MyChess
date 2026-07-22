using System;
using System.Collections.Generic;
using System.Text;

namespace MyIntegerChessEngine.Pieces
{
    internal class Rook : Piece
    {
        /// Called by Board.ExecuteMove: a rook leaving its home square
        /// invalidates the castle right on that side.
        internal static void ExecuteMove(Board board, Move move)
        {
            if (move.Piece.IntColor == Constants.White)
            {
                if (move.Start is { Column: 0, Row: 0 })
                    board.DisableWhiteCastleQueenSidePossible();
                else if (move.Start is { Column: 7, Row: 0 })
                    board.DisableWhiteCastleKingSidePossible();
            }
            else
            {
                if (move.Start is { Column: 0, Row: 7 })
                    board.DisableBlackCastleQueenSidePossible();
                else if (move.Start is { Column: 7, Row: 7 })
                    board.DisableBlackCastleKingSidePossible();
            }
        }

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
                    if (pieceAtTarget.IsBorder)
                        break; // Out of bounds
                    if (!pieceAtTarget.IsEmpty && pieceAtTarget.IntColor == rook.IntColor)
                        break; // Can't capture own piece

                    result.Add(new Move(position, targetPosition, rook));

                    if (!pieceAtTarget.IsEmpty)
                        break; // Capture opponent piece and stop

                    targetPosition = targetPosition.GetDeltaPosition(dx, dy);
                }
            }

            return result;
        }
    }
}
