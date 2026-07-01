namespace MyIntegerChessEngine.Pieces
{
    internal class Pawn : Piece
    {
        internal MoveList GetThreatenMoveList(Board board, Position position)
        {
            var result = new MoveList();
            Piece pawn = board.GetPiece(position);

            if (pawn.Color == Constants.White)
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

        internal MoveList GetMoveList(Board board, Position position)
        {
            var result = new MoveList();
            Piece pawn = board.GetPiece(position);

            if (pawn.Color == Constants.White)
                AddWhitePawnMoves(board, position, pawn, result);
            else
                AddBlackPawnMoves(board, position, pawn, result);

            return result;
        }

        private static void TryAddThreatMove(Board board, Position from, Piece pawn, MoveList list, int deltaColumn, int deltaRow)
        {
            Position target = from.GetDeltaPosition(deltaColumn, deltaRow);
            Piece atTarget = board.GetPiece(target);
            if (atTarget.PieceType == Constants.BoardBorder)
                return;
            if (atTarget.Color == pawn.Color)
                return;

            list.Add(new Move(from, target, pawn));
        }

        private static void AddWhitePawnMoves(Board board, Position position, Piece pawn, MoveList list)
        {
            // capture left (from white view: column -1, row +1)
            TryAddCapture(board, position, pawn, list, -1, 1);

            // forward one
            Position oneForward = position.GetDeltaRowPosition(1);
            if (IsEmpty(board, oneForward))
            {
                list.Add(new Move(position, oneForward, pawn));

                // double step from rank 2 (row index 1)
                if (position.Row == 1)
                {
                    Position twoForward = position.GetDeltaRowPosition(2);
                    if (IsEmpty(board, twoForward))
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
            if (IsEmpty(board, oneForward))
            {
                list.Add(new Move(position, oneForward, pawn));

                if (position.Row == 6)
                {
                    Position twoForward = position.GetDeltaRowPosition(-2);
                    if (IsEmpty(board, twoForward))
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
            if (atTarget.PieceType == Constants.BoardBorder)
                return;
            if (atTarget.Color == Constants.NoPiece)
                return;
            if (atTarget.Color == pawn.Color)
                return;

            list.Add(new Move(from, target, pawn));
        }

        private static bool IsEmpty(Board board, Position position)
        {
            Piece at = board.GetPiece(position);
            return at.PieceType != Constants.BoardBorder && at.Color == Constants.NoPiece;
        }

        private static bool IsEnemyPawn(Board board, Position position, int enemyColor)
        {
            Piece at = board.GetPiece(position);
            return at.PieceType == Constants.Pawn && at.Color == enemyColor;
        }

        private static void TryAddWhiteEnPassant(Board board, Position position, Piece pawn, MoveList list)
        {
            Position left = position.GetDeltaColumnPosition(-1);
            if (IsEnemyPawn(board, left, Constants.Black))
            {
                Position captureTo = position.GetDeltaPosition(-1, 1);
                if (IsEmpty(board, captureTo))
                    list.Add(new Move(position, captureTo, pawn));
            }

            Position right = position.GetDeltaColumnPosition(1);
            if (IsEnemyPawn(board, right, Constants.Black))
            {
                Position captureTo = position.GetDeltaPosition(1, 1);
                if (IsEmpty(board, captureTo))
                    list.Add(new Move(position, captureTo, pawn));
            }
        }

        private static void TryAddBlackEnPassant(Board board, Position position, Piece pawn, MoveList list)
        {
            Position left = position.GetDeltaColumnPosition(-1);
            if (IsEnemyPawn(board, left, Constants.White))
            {
                Position captureTo = position.GetDeltaPosition(-1, -1);
                if (IsEmpty(board, captureTo))
                    list.Add(new Move(position, captureTo, pawn));
            }

            Position right = position.GetDeltaColumnPosition(1);
            if (IsEnemyPawn(board, right, Constants.White))
            {
                Position captureTo = position.GetDeltaPosition(1, -1);
                if (IsEmpty(board, captureTo))
                    list.Add(new Move(position, captureTo, pawn));
            }
        }
    }
}
