namespace MyTranspositionChessEngine.Pieces
{
    internal static class Pawn
    {
        /// Called by Board.ExecuteMove before the pawn itself is moved:
        /// removes the captured pawn on en passant and marks adjacent
        /// enemy pawns after a double step.
        internal static void ExecuteMove(Board board, Move move)
        {
            // En passant capture: diagonal move onto an empty square,
            // the captured pawn stands beside the start square
            if (move.Start.Column != move.End.Column
                && board.GetPiece(move.End).PieceType == Constants.NoPiece)
            {
                board.ClearSquare(new Position(move.End.Column, move.Start.Row));
            }

            // Double step: allow adjacent enemy pawns to capture en passant.
            // Pawn.GetMoveList offers the capture while marking + 1 == CurrentPly,
            // i.e. only for the opponent's immediate reply (ply was already incremented).
            if (Math.Abs(move.End.Row - move.Start.Row) == 2)
            {
                MarkEnPassant(board, move, move.End.GetDeltaColumnPosition(-1));
                MarkEnPassant(board, move, move.End.GetDeltaColumnPosition(1));
            }

            // Promotion (queen only): rewrite the pawn on its start square,
            // MovePiece then carries the queen to the last row.
            if (move.End.Row == (move.Piece.IntColor == Constants.White ? 7 : 0))
                board.PromoteToQueen(move.Start);
        }

        private static void MarkEnPassant(Board board, Move move, Position neighbour)
        {
            Piece piece = board.GetPiece(neighbour);
            if (piece.PieceType != Constants.Pawn || piece.IntColor == move.Piece.IntColor)
                return;

            board.SetEnPassantMarking(neighbour, board.CurrentPly - 1);
        }

        internal static MoveList GetThreatenMoveList(Board board, Position position)
        {
            var result = new MoveList();
            Piece pawn = board.GetPiece(position);

            if (pawn.IntColor == Constants.White)
            {
                TryAddThreatMove(board, position, pawn, result, -1, 1);
                TryAddThreatMove(board, position, pawn, result, 1, 1);
            }
            else
            {
                TryAddThreatMove(board, position, pawn, result, -1, -1);
                TryAddThreatMove(board, position, pawn, result, 1, -1);
            }

            return result;
        }

        internal static MoveList GetMoveList(Board board, Position position)
        {
            var result = new MoveList();
            Piece pawn = board.GetPiece(position);

            if (pawn.IntColor == Constants.White)
                AddWhitePawnMoves(board, position, pawn, result);
            else
                AddBlackPawnMoves(board, position, pawn, result);

            return result;
        }

        private static void TryAddThreatMove(Board board, Position from, Piece pawn, MoveList list, int deltaColumn, int deltaRow)
        {
            Position target = from.GetDeltaPosition(deltaColumn, deltaRow);
            Piece atTarget = board.GetPiece(target);
            if (atTarget.IsBorder)
                return;
            if (!atTarget.IsEmpty && atTarget.IntColor == pawn.IntColor)
                return;

            list.Add(new Move(from, target, pawn));
        }

        private static void AddWhitePawnMoves(Board board, Position position, Piece pawn, MoveList list)
        {
            // capture left (from white view: column -1, row +1)
            TryAddCapture(board, position, pawn, list, -1, 1);

            // forward one
            Position oneForward = position.GetDeltaRowPosition(1);
            if (IsEmptySquare(board, oneForward))
            {
                list.Add(new Move(position, oneForward, pawn));

                // double step from rank 2 (row index 1)
                if (position.Row == 1)
                {
                    Position twoForward = position.GetDeltaRowPosition(2);
                    if (IsEmptySquare(board, twoForward))
                        list.Add(new Move(position, twoForward, pawn));
                }
            }

            // capture right
            TryAddCapture(board, position, pawn, list, 1, 1);

            // en passant
            if (pawn.LastEnPassantPlyMarking + 1 == board.CurrentPly)
            {
                TryAddWhiteEnPassant(board, position, pawn, list);
            }
        }

        private static void AddBlackPawnMoves(Board board, Position position, Piece pawn, MoveList list)
        {
            TryAddCapture(board, position, pawn, list, -1, -1);

            Position oneForward = position.GetDeltaRowPosition(-1);
            if (IsEmptySquare(board, oneForward))
            {
                list.Add(new Move(position, oneForward, pawn));

                if (position.Row == 6)
                {
                    Position twoForward = position.GetDeltaRowPosition(-2);
                    if (IsEmptySquare(board, twoForward))
                        list.Add(new Move(position, twoForward, pawn));
                }
            }

            TryAddCapture(board, position, pawn, list, 1, -1);

            if (pawn.LastEnPassantPlyMarking + 1 == board.CurrentPly)
            {
                TryAddBlackEnPassant(board, position, pawn, list);
            }
        }

        private static void TryAddCapture(Board board, Position from, Piece pawn, MoveList list, int deltaColumn, int deltaRow)
        {
            Position target = from.GetDeltaPosition(deltaColumn, deltaRow);
            Piece atTarget = board.GetPiece(target);
            if (atTarget.IsBorder || atTarget.IsEmpty)
                return;
            if (atTarget.IntColor == pawn.IntColor)
                return;

            list.Add(new Move(from, target, pawn));
        }

        private static bool IsEmptySquare(Board board, Position position)
        {
            return board.GetPiece(position).IsEmpty;
        }

        private static bool IsEnemyPawn(Board board, Position position, int enemyColor)
        {
            Piece at = board.GetPiece(position);
            return at.PieceType == Constants.Pawn && at.IntColor == enemyColor;
        }

        private static void TryAddWhiteEnPassant(Board board, Position position, Piece pawn, MoveList list)
        {
            Position left = position.GetDeltaColumnPosition(-1);
            if (IsEnemyPawn(board, left, Constants.Black))
            {
                Position captureTo = position.GetDeltaPosition(-1, 1);
                if (IsEmptySquare(board, captureTo))
                    list.Add(new Move(position, captureTo, pawn));
            }

            Position right = position.GetDeltaColumnPosition(1);
            if (IsEnemyPawn(board, right, Constants.Black))
            {
                Position captureTo = position.GetDeltaPosition(1, 1);
                if (IsEmptySquare(board, captureTo))
                    list.Add(new Move(position, captureTo, pawn));
            }
        }

        private static void TryAddBlackEnPassant(Board board, Position position, Piece pawn, MoveList list)
        {
            Position left = position.GetDeltaColumnPosition(-1);
            if (IsEnemyPawn(board, left, Constants.White))
            {
                Position captureTo = position.GetDeltaPosition(-1, -1);
                if (IsEmptySquare(board, captureTo))
                    list.Add(new Move(position, captureTo, pawn));
            }

            Position right = position.GetDeltaColumnPosition(1);
            if (IsEnemyPawn(board, right, Constants.White))
            {
                Position captureTo = position.GetDeltaPosition(1, -1);
                if (IsEmptySquare(board, captureTo))
                    list.Add(new Move(position, captureTo, pawn));
            }
        }
    }
}
