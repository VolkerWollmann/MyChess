using System;
using System.Collections.Generic;
using System.Text;

namespace MyTranspositionChessEngine.Pieces
{
    internal static class King
    {
        static readonly Position WhiteKingSideTarget = new("G1");
        static readonly Position WhiteQueenSideTarget = new("C1");
        static readonly Position BlackKingSideTarget = new("G8");
        static readonly Position BlackQueenSideTarget = new("C8");

        internal static readonly Position WhiteKingRookStart = new("H1");
        internal static readonly Position WhiteKingRookTarget = new("F1");
        internal static readonly Position WhiteQueenRookStart = new("A1");
        internal static readonly Position WhiteQueenRookTarget = new("D1");
        internal static readonly Position BlackKingRookStart = new("H8");
        internal static readonly Position BlackKingRookTarget = new("F8");
        internal static readonly Position BlackQueenRookStart = new("A8");
        internal static readonly Position BlackQueenRookTarget = new("D8");

        // Fields between king and rook must be empty, fields the king stands on
        // or passes over must be unthreatened
        static readonly Position[] WhiteKingSideEmptyFields = [new("F1"), new("G1")];
        static readonly Position[] WhiteKingSideKingFields = [new("E1"), new("F1"), new("G1")];
        static readonly Position[] WhiteQueenSideEmptyFields = [new("B1"), new("C1"), new("D1")];
        static readonly Position[] WhiteQueenSideKingFields = [new("C1"), new("D1"), new("E1")];
        static readonly Position[] BlackKingSideEmptyFields = [new("F8"), new("G8")];
        static readonly Position[] BlackKingSideKingFields = [new("E8"), new("F8"), new("G8")];
        static readonly Position[] BlackQueenSideEmptyFields = [new("B8"), new("C8"), new("D8")];
        static readonly Position[] BlackQueenSideKingFields = [new("C8"), new("D8"), new("E8")];

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
                    board.MovePiece(WhiteKingRookStart, WhiteKingRookTarget);
                    break;
                case CastleType.WhiteQueenSide:
                    board.MovePiece(WhiteQueenRookStart, WhiteQueenRookTarget);
                    break;
                case CastleType.BlackKingSide:
                    board.MovePiece(BlackKingRookStart, BlackKingRookTarget);
                    break;
                case CastleType.BlackQueenSide:
                    board.MovePiece(BlackQueenRookStart, BlackQueenRookTarget);
                    break;
            }
        }

        internal static MoveList GetThreatenMoveList(Board board, Position position)
        {
            MoveList moveList = new MoveList();
            Piece king = board.GetPiece(position);
            int[,] directions = Constants.AllDirections;
            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int dx = directions[i, 0];
                int dy = directions[i, 1];
                Position targetPosition = position.GetDeltaPosition(dx, dy);
                Piece pieceAtTarget = board.GetPiece(targetPosition);
                if (pieceAtTarget.IsBorder)
                    continue; // Out of bounds
                if (!pieceAtTarget.IsEmpty)
                {
                    if (pieceAtTarget.IntColor == king.IntColor)
                        continue; // Can't capture own piece
                }

                moveList.Add(new Move(position, targetPosition, king));
            }
            return moveList;
        }
        internal static MoveList GetMoveList(Board board, Position position)
        {
            Piece king = board.GetPiece(position);
            MoveList moveList = new MoveList();

            // The king cannot move onto a field threatened by the opponent
            foreach (Move move in GetThreatenMoveList(board, position))
            {
                if (!board.IsThreatened(move.End))
                    moveList.Add(move);
            }

            if (!king.IsMoved())
            {
                if (king.IntColor == Constants.White)
                {
                    // Check for white king-side castling
                    if (board.WhiteCastleKingSidePossible()
                        && CastleFieldsFree(board, WhiteKingSideEmptyFields, WhiteKingSideKingFields))
                    {
                        moveList.Add(new Move(position, WhiteKingSideTarget, king, CastleType.WhiteKingSide));
                    }

                    // Check for white queen-side castling
                    if (board.WhiteCastleQueenSidePossible()
                        && CastleFieldsFree(board, WhiteQueenSideEmptyFields, WhiteQueenSideKingFields))
                    {
                        moveList.Add(new Move(position, WhiteQueenSideTarget, king, CastleType.WhiteQueenSide));
                    }
                }
                else
                {
                    // Check for black king-side castling
                    if (board.BlackCastleKingSidePossible()
                        && CastleFieldsFree(board, BlackKingSideEmptyFields, BlackKingSideKingFields))
                    {
                        moveList.Add(new Move(position, BlackKingSideTarget, king, CastleType.BlackKingSide));
                    }

                    // Check for black queen-side castling
                    if (board.BlackCastleQueenSidePossible()
                        && CastleFieldsFree(board, BlackQueenSideEmptyFields, BlackQueenSideKingFields))
                    {
                        moveList.Add(new Move(position, BlackQueenSideTarget, king, CastleType.BlackQueenSide));
                    }
                }
            }

            return moveList;
        }

        /// A castle requires the fields between king and rook to be empty and
        /// the fields the king stands on or passes over to be unthreatened.
        private static bool CastleFieldsFree(Board board, Position[] emptyFields, Position[] unthreatenedFields)
        {
            foreach (Position field in emptyFields)
            {
                if (!board.GetPiece(field).IsEmpty)
                    return false;
            }

            foreach (Position field in unthreatenedFields)
            {
                if (board.IsThreatened(field))
                    return false;
            }

            return true;
        }
    }
}