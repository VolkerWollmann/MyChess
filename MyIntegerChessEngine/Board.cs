using System;
using System.Collections.Generic;
using MyChessEngineBase;
using MyIntegerChessEngine.Pieces;

namespace MyIntegerChessEngine
{
    
    /// 12×12 padded board: A1 = [2,2], H8 = [9,9]
    /// Each cell: [0] piece, [1] LastPly, [2] en passant marking, [3] threat.
    /// </summary>
    public class Board
    {
        public int[,,] Field = new int[Constants.Planes, Constants.GridSize, Constants.GridSize];

        public int CurrentPly;

        public Board()
        {
            InitBorder();
        }

        public void InitBorder()
        {
            for(int i=0; i<Constants.GridSize; i++)
            {
                Field[0,i,0] = Constants.BoardBorder;
                Field[0,i,1] = Constants.BoardBorder;
                Field[0,i,10] = Constants.BoardBorder;
                Field[0,i,11] =  Constants.BoardBorder;

                Field[0, 0, i] = Constants.BoardBorder;
                Field[0, 1, i] = Constants.BoardBorder;
                Field[0, 10, i] = Constants.BoardBorder;
                Field[0, 11, i] = Constants.BoardBorder;
            }
        }

        public void SetPiece(Piece piece, Position position)
        {
            UpdatePieceAccounting(Field[Constants.BroadPlane, position.Column + 2, position.Row + 2], -1);
            UpdatePieceAccounting(piece.PieceAsInteger, +1);

            Field[Constants.BroadPlane, position.Column + 2, position.Row + 2] = piece.PieceAsInteger;
            Field[Constants.LastPlyPlane, position.Column + 2, position.Row + 2] = piece.LastPly;
            Field[Constants.EnPassantPlane, position.Column + 2, position.Row + 2] = piece.LastEnPassantPlyMarking;

            if (piece.PieceType == Constants.King)
                ApplyPossibleCastles(piece);
        }

        /// Placing a king disables the castle rights its mask does not contain.
        private void ApplyPossibleCastles(Piece king)
        {
            if (king.IntColor == Constants.White)
            {
                if (!king.PossibleCastles.HasFlag(CastleType.WhiteKingSide))
                    DisableWhiteCastleKingSidePossible();
                if (!king.PossibleCastles.HasFlag(CastleType.WhiteQueenSide))
                    DisableWhiteCastleQueenSidePossible();
            }
            else
            {
                if (!king.PossibleCastles.HasFlag(CastleType.BlackKingSide))
                    DisableBlackCastleKingSidePossible();
                if (!king.PossibleCastles.HasFlag(CastleType.BlackQueenSide))
                    DisableBlackCastleQueenSidePossible();
            }
        }

        public Piece GetPiece(Position position)
        {
            int pieceValue = Field[Constants.BroadPlane, position.Column + 2, position.Row + 2];
            int lastPly = Field[Constants.LastPlyPlane, position.Column + 2, position.Row + 2];
            int lastEnPassantPlyMarking = Field[Constants.EnPassantPlane, position.Column + 2, position.Row + 2];
            return new Piece(pieceValue, lastPly, lastEnPassantPlyMarking);
        }   

        public void ExecuteMove(Move move)
        {
            CurrentPly++;

            // Piece-specific handling runs on the pre-move board:
            // King: castle rook move + castle right invalidation
            // Rook: castle right invalidation
            // Pawn: en passant capture and en passant marking
            switch (move.Piece.PieceType)
            {
                case Constants.King:
                    King.ExecuteMove(this, move);
                    break;
                case Constants.Rook:
                    Rook.ExecuteMove(this, move);
                    break;
                case Constants.Pawn:
                    Pawn.ExecuteMove(this, move);
                    break;
            }

            MovePiece(move.Start, move.End);
        }

        /// Transfers all planes from start to end, stamps the end square with CurrentPly
        /// and clears the start square. Does not increment the ply.
        internal void MovePiece(Position start, Position end)
        {
            int startColumn = start.Column + 2;
            int startRow = start.Row + 2;
            int endColumn = end.Column + 2;
            int endRow = end.Row + 2;

            // a capture removes the piece on the end square from the accounting
            UpdatePieceAccounting(Field[Constants.BroadPlane, endColumn, endRow], -1);

            Field[Constants.BroadPlane, endColumn, endRow]
                = Field[Constants.BroadPlane, startColumn, startRow];
            Field[Constants.LastPlyPlane, endColumn, endRow] = CurrentPly;
            Field[Constants.EnPassantPlane, endColumn, endRow]
                = Field[Constants.EnPassantPlane, startColumn, startRow];

            // raw clear: the moving piece only relocated, its value stays on the board
            Field[Constants.BroadPlane, startColumn, startRow] = Constants.NoPiece;
            Field[Constants.LastPlyPlane, startColumn, startRow] = 0;
            Field[Constants.EnPassantPlane, startColumn, startRow] = 0;
        }

        internal void ClearSquare(Position position)
        {
            int column = position.Column + 2;
            int row = position.Row + 2;

            UpdatePieceAccounting(Field[Constants.BroadPlane, column, row], -1);

            Field[Constants.BroadPlane, column, row] = Constants.NoPiece;
            Field[Constants.LastPlyPlane, column, row] = 0;
            Field[Constants.EnPassantPlane, column, row] = 0;
        }

        /// Adds (sign +1) or removes (sign -1) a piece to the incrementally
        /// maintained material sum and king counters read by GetRating.
        private void UpdatePieceAccounting(int pieceValue, int sign)
        {
            if (pieceValue == Constants.NoPiece || pieceValue == Constants.BoardBorder)
                return;

            int pieceType = pieceValue & Constants.PieceMask;
            bool white = (pieceValue & Constants.ColorMask) == Constants.White;

            MaterialValue += (white ? sign : -sign) * Constants.PieceValue(pieceType);

            if (pieceType == Constants.King)
            {
                if (white)
                    WhiteKings += sign;
                else
                    BlackKings += sign;
            }
        }

        #region Make/Unmake

        internal struct SavedSquare
        {
            public int Column;
            public int Row;
            public int PieceValue;
            public int LastPly;
            public int EnPassantMarking;
        }

        /// Snapshot of everything ExecuteMove can touch, created by ExecuteMoveWithUndo.
        internal struct UndoInfo
        {
            public int CurrentPly;
            public int ColorToMove;
            public int WhiteCastleKingSideFlag;
            public int WhiteCastleQueenSideFlag;
            public int BlackCastleKingSideFlag;
            public int BlackCastleQueenSideFlag;
            public int MaterialValue;
            public int WhiteKings;
            public int BlackKings;
            public SavedSquare[] Squares;
            public int SquareCount;
        }

        /// Executes the move like ExecuteMove and returns the snapshot UndoMove needs
        /// to take it back: the touched squares (start, end, en passant capture and
        /// marking fields, castle rook fields), castle flags, ColorToMove and the ply.
        internal UndoInfo ExecuteMoveWithUndo(Move move)
        {
            UndoInfo undo = new UndoInfo
            {
                CurrentPly = CurrentPly,
                ColorToMove = ColorToMove,
                WhiteCastleKingSideFlag = Field[Constants.LastPlyPlane, 0, 0],
                WhiteCastleQueenSideFlag = Field[Constants.LastPlyPlane, 0, 1],
                BlackCastleKingSideFlag = Field[Constants.LastPlyPlane, 0, 2],
                BlackCastleQueenSideFlag = Field[Constants.LastPlyPlane, 0, 3],
                MaterialValue = MaterialValue,
                WhiteKings = WhiteKings,
                BlackKings = BlackKings,
                Squares = new SavedSquare[5]
            };

            SaveSquare(ref undo, move.Start);
            SaveSquare(ref undo, move.End);

            switch (move.Piece.PieceType)
            {
                case Constants.Pawn:
                    // en passant capture field and the double step marking neighbours
                    SaveSquare(ref undo, new Position(move.End.Column, move.Start.Row));
                    SaveSquareOnBoard(ref undo, move.End.GetDeltaColumnPosition(-1));
                    SaveSquareOnBoard(ref undo, move.End.GetDeltaColumnPosition(1));
                    break;

                case Constants.King:
                    switch (move.CastleType)
                    {
                        case CastleType.WhiteKingSide:
                            SaveSquare(ref undo, King.WhiteKingRookStart);
                            SaveSquare(ref undo, King.WhiteKingRookTarget);
                            break;
                        case CastleType.WhiteQueenSide:
                            SaveSquare(ref undo, King.WhiteQueenRookStart);
                            SaveSquare(ref undo, King.WhiteQueenRookTarget);
                            break;
                        case CastleType.BlackKingSide:
                            SaveSquare(ref undo, King.BlackKingRookStart);
                            SaveSquare(ref undo, King.BlackKingRookTarget);
                            break;
                        case CastleType.BlackQueenSide:
                            SaveSquare(ref undo, King.BlackQueenRookStart);
                            SaveSquare(ref undo, King.BlackQueenRookTarget);
                            break;
                    }
                    break;
            }

            ExecuteMove(move);

            return undo;
        }

        internal void UndoMove(in UndoInfo undo)
        {
            for (int i = 0; i < undo.SquareCount; i++)
            {
                SavedSquare square = undo.Squares[i];
                int column = square.Column + 2;
                int row = square.Row + 2;

                Field[Constants.BroadPlane, column, row] = square.PieceValue;
                Field[Constants.LastPlyPlane, column, row] = square.LastPly;
                Field[Constants.EnPassantPlane, column, row] = square.EnPassantMarking;
            }

            Field[Constants.LastPlyPlane, 0, 0] = undo.WhiteCastleKingSideFlag;
            Field[Constants.LastPlyPlane, 0, 1] = undo.WhiteCastleQueenSideFlag;
            Field[Constants.LastPlyPlane, 0, 2] = undo.BlackCastleKingSideFlag;
            Field[Constants.LastPlyPlane, 0, 3] = undo.BlackCastleQueenSideFlag;

            MaterialValue = undo.MaterialValue;
            WhiteKings = undo.WhiteKings;
            BlackKings = undo.BlackKings;

            ColorToMove = undo.ColorToMove;
            CurrentPly = undo.CurrentPly;
        }

        private void SaveSquare(ref UndoInfo undo, Position position)
        {
            int column = position.Column + 2;
            int row = position.Row + 2;

            undo.Squares[undo.SquareCount++] = new SavedSquare
            {
                Column = position.Column,
                Row = position.Row,
                PieceValue = Field[Constants.BroadPlane, column, row],
                LastPly = Field[Constants.LastPlyPlane, column, row],
                EnPassantMarking = Field[Constants.EnPassantPlane, column, row]
            };
        }

        private void SaveSquareOnBoard(ref UndoInfo undo, Position position)
        {
            if (position.Column >= 0 && position.Column < ChessEngineConstants.Length)
                SaveSquare(ref undo, position);
        }

        #endregion

        /// Returns all possible moves of the side to move.
        /// The opponent's threats are marked first, so the king avoids threatened fields.
        public MoveList GetMoveList()
        {
            MarkThreatenedFields(ColorToMove == Constants.White ? Constants.Black : Constants.White);

            MoveList moveList = new MoveList();

            for (int column = 0; column < ChessEngineConstants.Length; column++)
            for (int row = 0; row < ChessEngineConstants.Length; row++)
            {
                Position position = new Position(column, row);
                Piece piece = GetPiece(position);

                if (piece.IsEmpty || piece.IntColor != ColorToMove)
                    continue;

                moveList.AddRange(GetMoveList(piece, position));
            }

            return moveList;
        }

        #region Threatened fields

        // Distinct fields marked by the last MarkThreatenedFields call and the
        // color they were marked for. At a leaf the last marking is always the
        // parent's GetMoveList - for the opponent of the parent, i.e. exactly
        // the side to move at the leaf - so the leaf evaluation reads the count
        // for free instead of sweeping the board again (one ply stale: the
        // parent's own move is not reflected in it).
        private int LastThreatMarkColor = -1;
        private int LastThreatMarkCount;

        /// Marks all fields on the threat plane where an actual or possible beat
        /// by <paramref name="color"/> can happen and remembers the distinct
        /// field count for the threat-field evaluation.
        public void MarkThreatenedFields(int color)
        {
            for (int column = 0; column < ChessEngineConstants.Length; column++)
            for (int row = 0; row < ChessEngineConstants.Length; row++)
                Field[Constants.ThreatPlane, column + 2, row + 2] = 0;

            int count = 0;
            foreach (Move move in GetThreatenMoveList(color))
            {
                if (Field[Constants.ThreatPlane, move.End.Column + 2, move.End.Row + 2] == 0)
                {
                    Field[Constants.ThreatPlane, move.End.Column + 2, move.End.Row + 2] = 1;
                    count++;
                }
            }

            LastThreatMarkColor = color;
            LastThreatMarkCount = count;
        }

        /// Threat-field term of the side to move, weighted with ThreatFieldValue:
        /// positive white threat count or negative black threat count. Bounded
        /// below one pawn, so with equal material the move threatening more own
        /// fields wins, but material always rules. The count remembered by the
        /// parent's MarkThreatenedFields is reused when it fits (free, one ply
        /// stale); otherwise the board is counted directly.
        internal int GetThreatFieldRating()
        {
            if (Constants.ThreatFieldValue == 0)
                return 0;

            int count = LastThreatMarkColor == ColorToMove
                ? LastThreatMarkCount
                : CountThreatenedFields(ColorToMove);

            return ColorToMove == Constants.White
                ? Constants.ThreatFieldValue * count
                : -Constants.ThreatFieldValue * count;
        }

        /// Number of distinct fields threatened by <paramref name="color"/>.
        /// Marks the transient threat plane with raw integer math instead of
        /// building move lists, so it is cheap enough for every leaf evaluation.
        /// Same semantics as the piece threaten lists: own fields excluded,
        /// enemy fields included, slider rays pass through the enemy king.
        private int CountThreatenedFields(int color)
        {
            for (int column = 0; column < ChessEngineConstants.Length; column++)
            for (int row = 0; row < ChessEngineConstants.Length; row++)
                Field[Constants.ThreatPlane, column + 2, row + 2] = 0;

            for (int column = 2; column < 2 + ChessEngineConstants.Length; column++)
            for (int row = 2; row < 2 + ChessEngineConstants.Length; row++)
            {
                int pieceValue = Field[Constants.BroadPlane, column, row];
                if (pieceValue == Constants.NoPiece || (pieceValue & Constants.ColorMask) != color)
                    continue;

                switch (pieceValue & Constants.PieceMask)
                {
                    case Constants.Pawn:
                        int pawnDirection = color == Constants.White ? 1 : -1;
                        MarkThreat(color, column - 1, row + pawnDirection);
                        MarkThreat(color, column + 1, row + pawnDirection);
                        break;

                    case Constants.Knight:
                        for (int i = 0; i < Constants.KnightDeltas.GetLength(0); i++)
                            MarkThreat(color, column + Constants.KnightDeltas[i, 0], row + Constants.KnightDeltas[i, 1]);
                        break;

                    case Constants.Bishop:
                        for (int i = 0; i < Constants.DiagonalDirections.GetLength(0); i++)
                            MarkThreatRay(color, column, row, Constants.DiagonalDirections[i, 0], Constants.DiagonalDirections[i, 1]);
                        break;

                    case Constants.Rook:
                        for (int i = 0; i < Constants.StraightDirections.GetLength(0); i++)
                            MarkThreatRay(color, column, row, Constants.StraightDirections[i, 0], Constants.StraightDirections[i, 1]);
                        break;

                    case Constants.Queen:
                        for (int i = 0; i < Constants.AllDirections.GetLength(0); i++)
                            MarkThreatRay(color, column, row, Constants.AllDirections[i, 0], Constants.AllDirections[i, 1]);
                        break;

                    case Constants.King:
                        for (int i = 0; i < Constants.AllDirections.GetLength(0); i++)
                            MarkThreat(color, column + Constants.AllDirections[i, 0], row + Constants.AllDirections[i, 1]);
                        break;
                }
            }

            int count = 0;
            for (int column = 0; column < ChessEngineConstants.Length; column++)
            for (int row = 0; row < ChessEngineConstants.Length; row++)
                count += Field[Constants.ThreatPlane, column + 2, row + 2];

            LastThreatMarkColor = color;
            LastThreatMarkCount = count;

            return count;
        }

        /// Marks a single threatened field (padded grid coordinates)
        /// unless it is a border field or occupied by an own piece.
        private void MarkThreat(int color, int column, int row)
        {
            int target = Field[Constants.BroadPlane, column, row];
            if (target == Constants.BoardBorder)
                return;
            if (target != Constants.NoPiece && (target & Constants.ColorMask) == color)
                return;

            Field[Constants.ThreatPlane, column, row] = 1;
        }

        /// Marks a slider ray (padded grid coordinates): stops at the border or
        /// an own piece, includes an enemy piece and passes through the enemy king.
        private void MarkThreatRay(int color, int column, int row, int deltaColumn, int deltaRow)
        {
            int targetColumn = column + deltaColumn;
            int targetRow = row + deltaRow;

            while (true)
            {
                int target = Field[Constants.BroadPlane, targetColumn, targetRow];
                if (target == Constants.BoardBorder)
                    return;
                if (target != Constants.NoPiece && (target & Constants.ColorMask) == color)
                    return;

                Field[Constants.ThreatPlane, targetColumn, targetRow] = 1;

                if (target != Constants.NoPiece && (target & Constants.PieceMask) != Constants.King)
                    return;

                targetColumn += deltaColumn;
                targetRow += deltaRow;
            }
        }

        public bool IsThreatened(Position position)
        {
            return Field[Constants.ThreatPlane, position.Column + 2, position.Row + 2] != 0;
        }

        /// Returns all fields where an actual or possible beat by <paramref name="color"/> can happen.
        public MoveList GetThreatenMoveList(int color)
        {
            MoveList moveList = new MoveList();

            for (int column = 0; column < ChessEngineConstants.Length; column++)
            for (int row = 0; row < ChessEngineConstants.Length; row++)
            {
                Position position = new Position(column, row);
                Piece piece = GetPiece(position);

                if (piece.IsEmpty || piece.IntColor != color)
                    continue;

                moveList.AddRange(GetThreatenMoveList(piece, position));
            }

            return moveList;
        }

        internal MoveList GetThreatenMoveList(Piece piece, Position position)
        {
            return piece.PieceType switch
            {
                Constants.Pawn => Pawn.GetThreatenMoveList(this, position),
                Constants.Knight => Knight.GetThreatenMoveList(this, position),
                Constants.Bishop => Bishop.GetThreatenMoveList(this, position),
                Constants.Rook => Rook.GetThreatenMoveList(this, position),
                Constants.Queen => Queen.GetThreatenMoveList(this, position),
                Constants.King => King.GetThreatenMoveList(this, position),
                _ => new MoveList()
            };
        }

        /// True if the king of <paramref name="color"/> stands on a threatened field.
        /// Reads the threat plane as marked by the last MarkThreatenedFields call.
        public bool IsKingThreatened(int color)
        {
            for (int column = 0; column < ChessEngineConstants.Length; column++)
            for (int row = 0; row < ChessEngineConstants.Length; row++)
            {
                Position position = new Position(column, row);
                Piece piece = GetPiece(position);

                if (piece.PieceType == Constants.King && !piece.IsEmpty && piece.IntColor == color)
                    return IsThreatened(position);
            }

            return false;
        }

        /// Number of fields the piece would threaten from <paramref name="position"/>,
        /// counted on the current board without executing the move. Cheap ordering
        /// heuristic: the vacated start square and the captured victim are ignored.
        private int CountPieceThreatsFrom(Piece piece, Position position)
        {
            int column = position.Column + 2;
            int row = position.Row + 2;
            int color = piece.IntColor;

            switch (piece.PieceType)
            {
                case Constants.Pawn:
                    int pawnDirection = color == Constants.White ? 1 : -1;
                    return CountThreat(color, column - 1, row + pawnDirection)
                           + CountThreat(color, column + 1, row + pawnDirection);

                case Constants.Knight:
                {
                    int count = 0;
                    for (int i = 0; i < Constants.KnightDeltas.GetLength(0); i++)
                        count += CountThreat(color, column + Constants.KnightDeltas[i, 0], row + Constants.KnightDeltas[i, 1]);
                    return count;
                }

                case Constants.Bishop:
                {
                    int count = 0;
                    for (int i = 0; i < Constants.DiagonalDirections.GetLength(0); i++)
                        count += CountThreatRay(color, column, row, Constants.DiagonalDirections[i, 0], Constants.DiagonalDirections[i, 1]);
                    return count;
                }

                case Constants.Rook:
                {
                    int count = 0;
                    for (int i = 0; i < Constants.StraightDirections.GetLength(0); i++)
                        count += CountThreatRay(color, column, row, Constants.StraightDirections[i, 0], Constants.StraightDirections[i, 1]);
                    return count;
                }

                case Constants.Queen:
                {
                    int count = 0;
                    for (int i = 0; i < Constants.AllDirections.GetLength(0); i++)
                        count += CountThreatRay(color, column, row, Constants.AllDirections[i, 0], Constants.AllDirections[i, 1]);
                    return count;
                }

                case Constants.King:
                {
                    int count = 0;
                    for (int i = 0; i < Constants.AllDirections.GetLength(0); i++)
                        count += CountThreat(color, column + Constants.AllDirections[i, 0], row + Constants.AllDirections[i, 1]);
                    return count;
                }
            }

            return 0;
        }

        /// 1 if the field (padded grid coordinates) is threatenable
        /// (no border, no own piece), otherwise 0.
        private int CountThreat(int color, int column, int row)
        {
            int target = Field[Constants.BroadPlane, column, row];
            if (target == Constants.BoardBorder)
                return 0;
            if (target != Constants.NoPiece && (target & Constants.ColorMask) == color)
                return 0;

            return 1;
        }

        /// Counts a slider ray (padded grid coordinates) like MarkThreatRay:
        /// stops at the border or an own piece, includes an enemy piece and
        /// passes through the enemy king.
        private int CountThreatRay(int color, int column, int row, int deltaColumn, int deltaRow)
        {
            int count = 0;
            int targetColumn = column + deltaColumn;
            int targetRow = row + deltaRow;

            while (true)
            {
                int target = Field[Constants.BroadPlane, targetColumn, targetRow];
                if (target == Constants.BoardBorder)
                    return count;
                if (target != Constants.NoPiece && (target & Constants.ColorMask) == color)
                    return count;

                count++;

                if (target != Constants.NoPiece && (target & Constants.PieceMask) != Constants.King)
                    return count;

                targetColumn += deltaColumn;
                targetRow += deltaRow;
            }
        }

        #endregion

        /// Material rating: white pieces count positive, black pieces negative.
        /// A missing king turns the state into WhiteLoss/BlackLoss.
        /// Reads the incrementally maintained accounting instead of scanning the board.
        public Rating GetRating()
        {
            GameState state = GameState.Normal;
            if (WhiteKings == 0)
                state = GameState.WhiteLoss;
            else if (BlackKings == 0)
                state = GameState.BlackLoss;

            return new Rating(MaterialValue, state);
        }

        internal MoveList GetMoveList(Piece piece, Position position)
        {
            return piece.PieceType switch
            {
                Constants.Pawn => Pawn.GetMoveList(this, position),
                Constants.Knight => Knight.GetMoveList(this, position),
                Constants.Bishop => Bishop.GetMoveList(this, position),
                Constants.Rook => Rook.GetMoveList(this, position),
                Constants.Queen => Queen.GetMoveList(this, position),
                Constants.King => King.GetMoveList(this, position),
                _ => new MoveList()
            };
        }

        /// Depth search (minimax) for the best move of the side to move.
        /// White maximizes, black minimizes the rating value.
        /// Returns null if there is no legal move or the game is already over.
        public Move? CalculateMove(int depth)
        {
            (Move? move, Rating rating) = Search(depth, int.MinValue, int.MaxValue);

            if (move != null)
                move.Rating = rating;

            return move;
        }

        /// Depth search like CalculateMove, the root moves are searched in parallel
        /// ("young brothers wait"): the first ordered move is searched with the full
        /// window to establish the bound, the remaining moves run in parallel and
        /// only take bounds from finished siblings with a lower index. That window
        /// is never tighter than the one the sequential search would use, so moves
        /// that beat their bound have exact values, refuted moves could not have
        /// been chosen by CalculateMove either, and the reduce in move order picks
        /// the same move deterministically.
        public Move? CalculateMoveParallel(int depth)
        {
            Rating rating = GetRating();

            if (rating.State != GameState.Normal || depth <= 0)
                return null;

            MoveList moves = GetMoveList();

            if (moves.Count == 0)
                return null; // checkmate or stalemate, no move to return

            bool white = ColorToMove == Constants.White;
            Move[] ordered = OrderMoves(moves);
            Rating?[] ratings = new Rating[ordered.Length];

            ratings[0] = SearchRootMove(ordered[0], depth, int.MinValue, int.MaxValue);

            Parallel.For(1, ordered.Length, i =>
            {
                int bound = ratings[0]!.Value;
                for (int j = 1; j < i; j++)
                {
                    Rating? sibling = Volatile.Read(ref ratings[j]);
                    if (sibling != null)
                        bound = white ? Math.Max(bound, sibling.Value) : Math.Min(bound, sibling.Value);
                }

                Rating result = white
                    ? SearchRootMove(ordered[i], depth, bound, int.MaxValue)
                    : SearchRootMove(ordered[i], depth, int.MinValue, bound);

                // Inside the half-open window the value is exact; otherwise the
                // move cannot beat an earlier sibling and is dropped, like a move
                // in the sequential loop that does not raise alpha.
                if (white ? result.Value > bound : result.Value < bound)
                    Volatile.Write(ref ratings[i], result);
            });

            Move bestMove = ordered[0];
            Rating bestRating = ratings[0]!;

            for (int i = 1; i < ordered.Length; i++)
            {
                if (ratings[i] == null)
                    continue;

                if (white ? ratings[i]!.Value > bestRating.Value : ratings[i]!.Value < bestRating.Value)
                {
                    bestRating = ratings[i]!;
                    bestMove = ordered[i];
                }
            }

            // SearchRootMove only searched the reply position, so the root move
            // itself still has to be prepended to the line.
            bestRating.AddMove($"{bestMove.Start}-{bestMove.End}");

            bestMove.Rating = bestRating;
            return bestMove;
        }

        /// Executes the root move on a board copy and searches the reply position.
        private Rating SearchRootMove(Move move, int depth, int alpha, int beta)
        {
            Board copy = Copy();
            copy.ExecuteMove(move);
            copy.ColorToMove = ColorToMove == Constants.White ? Constants.Black : Constants.White;

            (_, Rating result) = copy.Search(depth - 1, alpha, beta);
            return result;
        }

        /// Alpha-beta search: alpha/beta bound the values white/black can already
        /// force elsewhere in the tree; branches outside the window are cut off.
        private (Move? Move, Rating Rating) Search(int depth, int alpha, int beta)
        {
            Rating rating = GetRating();

            // A captured king ends the line. Win/loss ratings are depth-dominated
            // and material-free: a faster win (for the loser a later loss) always
            // outranks any material gain, so the loser defends the king instead of
            // grabbing pieces. The +2 charges the capture to the illegal move two
            // plies further up: a direct king kill outranks the checkmate that a
            // legal defense would only postpone to the same node.
            if (rating.State == GameState.BlackLoss)
                return (null, new Rating(Constants.KingValue + (depth + 2) * Constants.WinDepthValue, rating.State));
            if (rating.State == GameState.WhiteLoss)
                return (null, new Rating(-Constants.KingValue - (depth + 2) * Constants.WinDepthValue, rating.State));

            // Only the side to move at the leaf is counted: at fixed search depth
            // every leaf has the same side to move, so the values stay comparable
            // across the tree - an even root depth counts the own threats ("for
            // me"), an odd one the opponent's ("against me"). The count itself is
            // free: GetThreatFieldRating reuses the marking the parent's
            // GetMoveList already made for exactly this color.
            if (depth <= 0)
                return (null, new Rating(rating.Value + GetThreatFieldRating(), rating.State));

            bool white = ColorToMove == Constants.White;
            Move? bestMove = null;
            Rating? bestRating = null;

            foreach (Move move in OrderMoves(GetMoveList()))
            {
                UndoInfo undo = ExecuteMoveWithUndo(move);
                ColorToMove = white ? Constants.Black : Constants.White;

                (_, Rating moveRating) = Search(depth - 1, alpha, beta);

                UndoMove(undo);

                if (bestRating == null
                    || (white ? moveRating.Value > bestRating.Value : moveRating.Value < bestRating.Value))
                {
                    bestRating = moveRating;
                    bestMove = move;
                }

                if (white)
                    alpha = Math.Max(alpha, bestRating.Value);
                else
                    beta = Math.Min(beta, bestRating.Value);

                if (alpha >= beta)
                    break; // opponent avoids this line, no need to search the rest
            }

            if (bestMove == null)
            {
                // No legal move: checkmate if the own king is in check, otherwise stalemate.
                // The threat plane is current, GetMoveList marked it for the opponent.
                if (IsKingThreatened(ColorToMove))
                {
                    return white
                        ? (null, new Rating(-Constants.KingValue - depth * Constants.WinDepthValue, GameState.WhiteLoss))
                        : (null, new Rating(Constants.KingValue + depth * Constants.WinDepthValue, GameState.BlackLoss));
                }

                return (null, new Rating(0, GameState.Remis));
            }

            // Every rating object flows up exactly one search path, so prepending
            // here builds the strongest line in root-first order while unwinding.
            bestRating!.AddMove($"{bestMove.Start}-{bestMove.End}");

            return (bestMove, bestRating)!;
        }

        /// Captures first, most valuable victim first; equal captures and quiet
        /// moves are ordered by the fields the moved piece threatens from its
        /// target square, so lines that raise the threat-field evaluation are
        /// searched first and alpha-beta cuts earlier. Piece values are multiples
        /// of 100 and a single piece threatens at most 27 fields, so the capture
        /// priority can never collide with the threat count.
        /// OrderByDescending is stable, ties keep the move generation order.
        private Move[] OrderMoves(MoveList moves)
        {
            return moves.OrderByDescending(move =>
            {
                Piece victim = GetPiece(move.End);
                int victimValue = victim.IsEmpty ? 0 : Constants.PieceValue(victim.PieceType);
                return victimValue + CountPieceThreatsFrom(move.Piece, move.End);
            }).ToArray();
        }

        internal void SetEnPassantMarking(Position position, int ply)
        {
            Field[Constants.EnPassantPlane, position.Column + 2, position.Row + 2] = ply;
        }

        /// Replaces the piece on <paramref name="position"/> by a queen of the same color.
        internal void PromoteToQueen(Position position)
        {
            int column = position.Column + 2;
            int row = position.Row + 2;

            int color = Field[Constants.BroadPlane, column, row] & Constants.ColorMask;
            Field[Constants.BroadPlane, column, row] = Constants.Queen | color;

            int gain = Constants.QueenValue - Constants.PawnValue;
            MaterialValue += color == Constants.White ? gain : -gain;
        }

        public void New()
        {
            Field = new int[Constants.Planes, Constants.GridSize, Constants.GridSize];
            CurrentPly = 0;
            MaterialValue = 0;
            WhiteKings = 0;
            BlackKings = 0;
            LastThreatMarkColor = -1;
            LastThreatMarkCount = 0;
            InitBorder();
        }

        int GetPieceValue(Position position)
        {
            return Field[Constants.LastPlyPlane, position.Column+2, position.Row+2];
        }

        #region Castling
        public bool WhiteCastleKingSidePossible()
        {
            return Field[Constants.LastPlyPlane, 0, 0] == 0;
        }

        public bool WhiteCastleQueenSidePossible()
        {
            return Field[Constants.LastPlyPlane, 0, 1] == 0;
        }


        public bool BlackCastleKingSidePossible()
        {
            return Field[Constants.LastPlyPlane, 0, 2] == 0;
        }

        public bool BlackCastleQueenSidePossible()
        {
            return Field[Constants.LastPlyPlane, 0, 3] == 0;
        }

        public void DisableWhiteCastleKingSidePossible()
        {
            Field[Constants.LastPlyPlane, 0, 0] = 1;
        }

        public void DisableWhiteCastleQueenSidePossible()
        {
            Field[Constants.LastPlyPlane, 0, 1] = 1;
        }


        public void DisableBlackCastleKingSidePossible()
        {
            Field[Constants.LastPlyPlane, 0, 2] = 1;
        }

        public void DisableBlackCastleQueenSidePossible()
        {
            Field[Constants.LastPlyPlane, 0, 3] = 1;
        }
        #endregion

        // Index 4: indices 0-3 hold the castling right flags
        public int ColorToMove
        {
            get => Field[Constants.LastPlyPlane, 0, 4];
            set => Field[Constants.LastPlyPlane, 0, 4] = value;
        }

        // Incrementally maintained accounting, updated by UpdatePieceAccounting
        // and read by GetRating. Kept outside the Field array: border cells are
        // read by GetPiece for out-of-board positions, so board data must not
        // alias with counters. Copy and New handle these explicitly.
        public int MaterialValue { get; private set; }

        private int WhiteKings;

        private int BlackKings;


        public Board Copy()
        {
            Board newBoard = new();
            newBoard.Field = (int[,,])Field.Clone();
            newBoard.CurrentPly = CurrentPly;
            newBoard.MaterialValue = MaterialValue;
            newBoard.WhiteKings = WhiteKings;
            newBoard.BlackKings = BlackKings;
            newBoard.LastThreatMarkColor = LastThreatMarkColor;
            newBoard.LastThreatMarkCount = LastThreatMarkCount;

            return newBoard;
        }

        public bool CompareBoard(Board other)
        {

            for (int i = 2; i < 2+ ChessEngineConstants.Length; i++)
            for (int j = 2; j < 2+ ChessEngineConstants.Length; j++)
            {
                if (this.Field[Constants.BroadPlane,i, j] != other.Field[Constants.BroadPlane,i, j]) 
                    return false;
            }

            return true;
        }
    }
}
