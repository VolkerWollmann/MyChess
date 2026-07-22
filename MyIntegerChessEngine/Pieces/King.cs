using System;
using System.Collections.Generic;
using System.Text;

namespace MyIntegerChessEngine.Pieces
{
    internal class King : Piece
    {
        /// Called by Board.ExecuteMove before the king itself is moved:
        /// executes the rook part of a castle and invalidates the castle rights.
        internal static void ExecuteMove(Board board, Move move)
        {
            if (move.Piece.IntColor == Constants.White)
            {
                board.DisableWhiteCastleKingSidePossible();
                board.DisableWhiteCastleQueenSidePossible();
            }
            else
            {
                board.DisableBlackCastleKingSidePossible();
                board.DisableBlackCastleQueenSidePossible();
            }

            switch (move.CastleType)
            {
                case CastleType.WhiteKingSide:
                    board.MovePiece(new Position("H1"), new Position("F1"));
                    break;
                case CastleType.WhiteQueenSide:
                    board.MovePiece(new Position("A1"), new Position("D1"));
                    break;
                case CastleType.BlackKingSide:
                    board.MovePiece(new Position("H8"), new Position("F8"));
                    break;
                case CastleType.BlackQueenSide:
                    board.MovePiece(new Position("A8"), new Position("D8"));
                    break;
            }
        }

        internal MoveList GetThreatenMoveList(Board board, Position position)
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
                if (pieceAtTarget.PieceType != Constants.NoPiece)
                {
                    if (pieceAtTarget.IntColor == king.IntColor)
                        continue; // Can't capture own piece
                }

                moveList.Add(new Move(position, targetPosition, king));
            }
            return moveList;
        }
        internal override MoveList GetMoveList(Board board, Position position)
        {
            Piece king = board.GetPiece(position);
            MoveList moveList = GetThreatenMoveList(board, position);

            if (!IsMoved())
            {
                if (king.IntColor == Constants.White)
                {
                    // Check for white king-side castling
                    if (board.WhiteCastleKingSidePossible())
                    {
                        moveList.Add(new Move(position, new Position("G1"), king, CastleType.WhiteKingSide));
                    }

                    // Check for white queen-side castling
                    if (board.WhiteCastleQueenSidePossible())
                    {
                        moveList.Add(new Move(position, new Position("C1"), king, CastleType.WhiteQueenSide));
                    }
                }
                else
                {
                    // Check for black king-side castling
                    if (board.BlackCastleKingSidePossible())
                    {
                        moveList.Add(new Move(position, new Position("G8"), king, CastleType.BlackKingSide));
                    }

                    // Check for black queen-side castling
                    if (board.BlackCastleQueenSidePossible())
                    {
                        moveList.Add(new Move(position, new Position("C8"), king, CastleType.BlackQueenSide));
                    }
                }
            }

            return moveList;
        }
    }
}