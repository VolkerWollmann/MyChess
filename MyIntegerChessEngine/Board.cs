using System;
using System.Collections.Generic;
using MyChessEngineBase;
using MyIntegerChessEngine.Pieces;

namespace MyIntegerChessEngine
{
    
    /// 12×12 padded board: A1 = [2,2], H8 = [9,9] 
    /// Each cell: [0] piece, [1] LastPly, [2] aux metadata.
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
            Field[Constants.BroadPlane, position.Column + 2, position.Row + 2] = piece.PieceAsInteger;
            Field[Constants.LastPlyPlane, position.Column + 2, position.Row + 2] = piece.LastPly;
            Field[Constants.PromotionPlane, position.Column + 2, position.Row + 2] = piece.PromotionPly;
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
            int promotionPly = Field[Constants.PromotionPlane, position.Column + 2, position.Row + 2];
            int lastEnPassantPlyMarking = Field[Constants.EnPassantPlane, position.Column + 2, position.Row + 2];
            return new Piece(pieceValue, lastPly, promotionPly, lastEnPassantPlyMarking);
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

            Field[Constants.BroadPlane, endColumn, endRow]
                = Field[Constants.BroadPlane, startColumn, startRow];
            Field[Constants.LastPlyPlane, endColumn, endRow] = CurrentPly;
            Field[Constants.PromotionPlane, endColumn, endRow]
                = Field[Constants.PromotionPlane, startColumn, startRow];
            Field[Constants.EnPassantPlane, endColumn, endRow]
                = Field[Constants.EnPassantPlane, startColumn, startRow];

            ClearSquare(start);
        }

        internal void ClearSquare(Position position)
        {
            int column = position.Column + 2;
            int row = position.Row + 2;

            Field[Constants.BroadPlane, column, row] = Constants.NoPiece;
            Field[Constants.LastPlyPlane, column, row] = 0;
            Field[Constants.PromotionPlane, column, row] = 0;
            Field[Constants.EnPassantPlane, column, row] = 0;
        }

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

        /// Marks all fields on the threat plane where an actual or possible beat
        /// by <paramref name="color"/> can happen.
        public void MarkThreatenedFields(int color)
        {
            for (int column = 0; column < ChessEngineConstants.Length; column++)
            for (int row = 0; row < ChessEngineConstants.Length; row++)
                Field[Constants.ThreatPlane, column + 2, row + 2] = 0;

            foreach (Move move in GetThreatenMoveList(color))
                Field[Constants.ThreatPlane, move.End.Column + 2, move.End.Row + 2] = 1;
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

        /// Material rating: white pieces count positive, black pieces negative.
        /// A missing king turns the state into WhiteLoss/BlackLoss.
        public Rating GetRating()
        {
            int ratingValue = 0;
            bool whiteKingOnBoard = false;
            bool blackKingOnBoard = false;

            for (int column = 0; column < ChessEngineConstants.Length; column++)
            for (int row = 0; row < ChessEngineConstants.Length; row++)
            {
                Piece piece = GetPiece(new Position(column, row));

                if (piece.IsEmpty)
                    continue;

                int value = piece.PieceType switch
                {
                    Constants.Pawn => Constants.PawnValue,
                    Constants.Knight => Constants.KnightValue,
                    Constants.Bishop => Constants.BishopValue,
                    Constants.Rook => Constants.RookValue,
                    Constants.Queen => Constants.QueenValue,
                    Constants.King => Constants.KingValue,
                    _ => 0
                };

                if (piece.PieceType == Constants.King)
                {
                    if (piece.IntColor == Constants.White)
                        whiteKingOnBoard = true;
                    else
                        blackKingOnBoard = true;
                }

                ratingValue += piece.IntColor == Constants.White ? value : -value;
            }

            GameState state = GameState.Normal;
            if (!whiteKingOnBoard)
                state = GameState.WhiteLoss;
            else if (!blackKingOnBoard)
                state = GameState.BlackLoss;

            return new Rating(ratingValue, state);
        }

        internal MoveList GetMoveList(Piece piece, Position position)
        {
            return piece.PieceType switch
            {
                Constants.Pawn => new Pawn().GetMoveList(this, position),
                Constants.Knight => new Knight().GetMoveList(this, position),
                Constants.Bishop => new Bishop().GetMoveList(this, position),
                Constants.Rook => new Rook().GetMoveList(this, position),
                Constants.Queen => new Queen().GetMoveList(this, position),
                Constants.King => new King().GetMoveList(this, position),
                _ => new MoveList()
            };
        }

        /// Depth search (minimax) for the best move of the side to move.
        /// White maximizes, black minimizes the rating value.
        /// Returns null if there is no legal move or the game is already over.
        public Move CalculateMove(int depth)
        {
            (Move move, Rating rating) = Search(depth);

            if (move != null)
                move.Rating = rating;

            return move;
        }

        private (Move Move, Rating Rating) Search(int depth)
        {
            Rating rating = GetRating();

            // A captured king ends the line; the depth bonus prefers the faster win
            // (and for the loser the later loss).
            if (rating.State == GameState.BlackLoss)
                return (null, new Rating(rating.Value + depth, rating.State));
            if (rating.State == GameState.WhiteLoss)
                return (null, new Rating(rating.Value - depth, rating.State));

            if (depth <= 0)
                return (null, rating);

            bool white = ColorToMove == Constants.White;
            Move bestMove = null;
            Rating bestRating = null;

            foreach (Move move in GetMoveList())
            {
                Board copy = Copy();
                copy.ExecuteMove(move);
                copy.ColorToMove = white ? Constants.Black : Constants.White;

                (_, Rating moveRating) = copy.Search(depth - 1);

                if (bestRating == null
                    || (white ? moveRating.Value > bestRating.Value : moveRating.Value < bestRating.Value))
                {
                    bestRating = moveRating;
                    bestMove = move;
                }
            }

            if (bestMove == null)
            {
                // No legal move: checkmate if the own king is in check, otherwise stalemate.
                // The threat plane is current, GetMoveList marked it for the opponent.
                if (IsKingThreatened(ColorToMove))
                {
                    return white
                        ? (null, new Rating(rating.Value - Constants.KingValue - depth, GameState.WhiteLoss))
                        : (null, new Rating(rating.Value + Constants.KingValue + depth, GameState.BlackLoss));
                }

                return (null, new Rating(0, GameState.Remis));
            }

            return (bestMove, bestRating);
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

        internal MoveList GetThreatenMoveList(Piece piece, Position position)
        {
            return piece.PieceType switch
            {
                Constants.Pawn => new Pawn().GetThreatenMoveList(this, position),
                Constants.Knight => new Knight().GetThreatenMoveList(this, position),
                Constants.Bishop => new Bishop().GetThreatenMoveList(this, position),
                Constants.Rook => new Rook().GetThreatenMoveList(this, position),
                Constants.Queen => new Queen().GetThreatenMoveList(this, position),
                Constants.King => new King().GetThreatenMoveList(this, position),
                _ => new MoveList()
            };
        }

        internal void SetEnPassantMarking(Position position, int ply)
        {
            Field[Constants.EnPassantPlane, position.Column + 2, position.Row + 2] = ply;
        }

        public void New()
        {
            Field = new int[Constants.Planes, Constants.GridSize, Constants.GridSize];
            CurrentPly = 0;
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


        public Board Copy()
        {
            Board newBoard = new();
            newBoard.Field = (int[,,])Field.Clone();
            newBoard.CurrentPly = CurrentPly;

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
