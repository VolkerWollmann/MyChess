using System;
using System.Collections.Generic;
using System.Text;

namespace MyIntegerChessEngine.Pieces
{
    internal static class Knight
    {
        internal static MoveList GetThreatenMoveList(Board board, Position position)
        {
            return GetMoveList(board, position);
        }

        internal static MoveList GetMoveList(Board board, Position position)
        {
            var result = new MoveList();
            Piece knight = board.GetPiece(position);
            for (int i= 0; i < Constants.KnightDeltas.GetLength(0); i++)
            {
                Position targetPosition = position.GetDeltaPosition(Constants.KnightDeltas[i,0], Constants.KnightDeltas[i,1]);

                Piece pieceAtTarget = board.GetPiece(targetPosition);
                if (pieceAtTarget.IsBorder)
                    continue; // Out of bounds
                if (!pieceAtTarget.IsEmpty && pieceAtTarget.IntColor == knight.IntColor)
                    continue; // Can't capture own piece
                
                result.Add(new Move(position, targetPosition, knight));
            }

            return result;
        }
    }
}
