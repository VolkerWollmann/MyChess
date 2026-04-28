using System;
using System.Collections.Generic;
using System.Diagnostics;
using MyChessEngine.Pieces;
using MyChessEngineBase;
using MyChessEngineBase.Interfaces;
using MyChessEngineBase.Rating;

namespace MyChessEngine.Bitboard
{
    /// <summary>
    /// Secondary chess engine based on 64-bit bitboards.
    /// Supports standard movement and capture rules with promotion to queen.
    /// Castling and en passant are intentionally not implemented yet.
    /// </summary>
    public sealed class BitboardChessEngine : IChessEngine
    {
        private const int SearchDepth = 4;

        private readonly ulong[] _pieceBoards = new ulong[12];
        private Color _colorToMove;
        private string _message = string.Empty;

        private enum PieceIndex
        {
            WhitePawn = 0,
            WhiteKnight = 1,
            WhiteBishop = 2,
            WhiteRook = 3,
            WhiteQueen = 4,
            WhiteKing = 5,
            BlackPawn = 6,
            BlackKnight = 7,
            BlackBishop = 8,
            BlackRook = 9,
            BlackQueen = 10,
            BlackKing = 11
        }

        private readonly struct BitMove
        {
            public readonly int From;
            public readonly int To;
            public readonly PieceType PieceType;
            public readonly bool IsCapture;
            public readonly bool IsPromotion;

            public BitMove(int from, int to, PieceType pieceType, bool isCapture, bool isPromotion)
            {
                From = from;
                To = to;
                PieceType = pieceType;
                IsCapture = isCapture;
                IsPromotion = isPromotion;
            }
        }

        private struct BoardState
        {
            public readonly ulong[] PieceBoards;
            public readonly Color ColorToMove;

            public BoardState(ulong[] pieceBoards, Color colorToMove)
            {
                PieceBoards = pieceBoards;
                ColorToMove = colorToMove;
            }
        }

        public Color ColorToMove
        {
            get => _colorToMove;
            set => _colorToMove = value;
        }

        public string Message => _message;

        public BitboardChessEngine()
        {
            Clear();
        }

        public void New()
        {
            Clear();

            // White pieces
            _pieceBoards[(int)PieceIndex.WhitePawn] = 0x000000000000FF00UL;
            _pieceBoards[(int)PieceIndex.WhiteRook] = 0x0000000000000081UL;
            _pieceBoards[(int)PieceIndex.WhiteKnight] = 0x0000000000000042UL;
            _pieceBoards[(int)PieceIndex.WhiteBishop] = 0x0000000000000024UL;
            _pieceBoards[(int)PieceIndex.WhiteQueen] = 0x0000000000000008UL;
            _pieceBoards[(int)PieceIndex.WhiteKing] = 0x0000000000000010UL;

            // Black pieces
            _pieceBoards[(int)PieceIndex.BlackPawn] = 0x00FF000000000000UL;
            _pieceBoards[(int)PieceIndex.BlackRook] = 0x8100000000000000UL;
            _pieceBoards[(int)PieceIndex.BlackKnight] = 0x4200000000000000UL;
            _pieceBoards[(int)PieceIndex.BlackBishop] = 0x2400000000000000UL;
            _pieceBoards[(int)PieceIndex.BlackQueen] = 0x0800000000000000UL;
            _pieceBoards[(int)PieceIndex.BlackKing] = 0x1000000000000000UL;

            _colorToMove = Color.White;
            _message = "Bitboard engine initialized.";
        }

        public void Clear()
        {
            Array.Clear(_pieceBoards, 0, _pieceBoards.Length);
            _colorToMove = Color.White;
            _message = "Board cleared.";
        }

        public IPiece GetPiece(Position position)
        {
            int square = SquareIndex(position);
            if (square < 0)
                return null;

            ulong mask = 1UL << square;
            for (int i = 0; i < _pieceBoards.Length; i++)
            {
                if ((_pieceBoards[i] & mask) == 0)
                    continue;

                PieceType type = IndexToPieceType((PieceIndex)i);
                Color color = i <= (int)PieceIndex.WhiteKing ? Color.White : Color.Black;
                return CreateDisplayPiece(type, color, position);
            }

            return null;
        }

        public BoardRating GetRating(Color color)
        {
            return BuildRating(color, EvaluateMaterial());
        }

        public BoardRating GetBoardRating()
        {
            return BuildRating(_colorToMove, EvaluateMaterial());
        }

        public void Test()
        {
            List<BitMove> moves = GenerateLegalMoves(_colorToMove);
            _message = $"Bitboard legal moves: {moves.Count}{Environment.NewLine}";
            int max = Math.Min(40, moves.Count);
            for (int i = 0; i < max; i++)
            {
                _message += $"{SquareToPosition(moves[i].From)}-{SquareToPosition(moves[i].To)}{Environment.NewLine}";
            }
        }

        public bool ExecuteMove(Move move)
        {
            if (move == null)
                return false;

            int from = SquareIndex(move.Start);
            int to = SquareIndex(move.End);
            if (from < 0 || to < 0)
                return false;

            var legalMoves = GenerateLegalMoves(_colorToMove);
            for (int i = 0; i < legalMoves.Count; i++)
            {
                if (legalMoves[i].From != from || legalMoves[i].To != to)
                    continue;

                ApplyMove(legalMoves[i]);
                _colorToMove = ChessEngineConstants.NextColorToMove(_colorToMove);
                return true;
            }

            _message = $"Illegal move ignored: {move.Start}-{move.End}";
            return false;
        }

        public Move CalculateMove()
        {
            return CalculateMoveWithDepth(SearchDepth);
        }

        public Move CalculateMoveWithDepth(int depth)
        {
            var timer = Stopwatch.StartNew();
            int nodes = 0;

            List<BitMove> moves = GenerateLegalMoves(_colorToMove);
            if (moves.Count == 0)
            {
                BoardRating terminal = BuildTerminalRatingForNoMoves(_colorToMove);
                timer.Stop();
                _message = $"No legal moves. {terminal.Situation}";
                return Move.CreateNoMove(terminal);
            }

            int bestScore = _colorToMove == Color.White ? int.MinValue : int.MaxValue;
            BitMove bestMove = moves[0];

            for (int i = 0; i < moves.Count; i++)
            {
                BoardState backup = CaptureState();
                ApplyMove(moves[i]);
                _colorToMove = ChessEngineConstants.NextColorToMove(_colorToMove);

                int score = AlphaBeta(depth - 1, int.MinValue + 1, int.MaxValue - 1, ref nodes);

                RestoreState(backup);
                if (_colorToMove == Color.White)
                {
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = moves[i];
                    }
                }
                else if (score < bestScore)
                {
                    bestScore = score;
                    bestMove = moves[i];
                }
            }

            BoardRating rating = BuildRating(_colorToMove, bestScore);
            Move result = BuildExternalMove(bestMove, rating);

            timer.Stop();
            _message = $"{result} Time:{timer.Elapsed}{Environment.NewLine}" +
                       $"Situation:{rating.Situation} Evaluation:{rating.Evaluation}{Environment.NewLine}" +
                       $"Score:{rating.Weight} Nodes:{nodes} (bitboard)";
            return result;
        }

        private int AlphaBeta(int depth, int alpha, int beta, ref int nodes)
        {
            nodes++;
            List<BitMove> moves = GenerateLegalMoves(_colorToMove);
            if (depth <= 0 || moves.Count == 0)
            {
                if (moves.Count == 0)
                {
                    if (IsInCheck(_colorToMove))
                    {
                        return _colorToMove == Color.White ? -ChessEngineConstants.CheckMate : ChessEngineConstants.CheckMate;
                    }
                    return 0;
                }
                return EvaluateMaterial();
            }

            if (_colorToMove == Color.White)
            {
                int best = int.MinValue;
                for (int i = 0; i < moves.Count; i++)
                {
                    BoardState backup = CaptureState();
                    ApplyMove(moves[i]);
                    _colorToMove = Color.Black;

                    int score = AlphaBeta(depth - 1, alpha, beta, ref nodes);

                    RestoreState(backup);
                    if (score > best)
                        best = score;
                    if (best > alpha)
                        alpha = best;
                    if (alpha >= beta)
                        break;
                }
                return best;
            }

            int worst = int.MaxValue;
            for (int i = 0; i < moves.Count; i++)
            {
                BoardState backup = CaptureState();
                ApplyMove(moves[i]);
                _colorToMove = Color.White;

                int score = AlphaBeta(depth - 1, alpha, beta, ref nodes);

                RestoreState(backup);
                if (score < worst)
                    worst = score;
                if (worst < beta)
                    beta = worst;
                if (alpha >= beta)
                    break;
            }

            return worst;
        }

        private List<BitMove> GenerateLegalMoves(Color color)
        {
            List<BitMove> pseudoMoves = GeneratePseudoLegalMoves(color);
            List<BitMove> legal = new List<BitMove>(pseudoMoves.Count);

            for (int i = 0; i < pseudoMoves.Count; i++)
            {
                BoardState backup = CaptureState();
                ApplyMove(pseudoMoves[i]);
                Color next = ChessEngineConstants.NextColorToMove(color);
                _colorToMove = next;

                bool kingInCheck = IsInCheck(color);
                RestoreState(backup);
                if (!kingInCheck)
                    legal.Add(pseudoMoves[i]);
            }

            return legal;
        }

        private List<BitMove> GeneratePseudoLegalMoves(Color color)
        {
            var moves = new List<BitMove>(96);
            ulong own = Occupancy(color);
            ulong enemy = Occupancy(ChessEngineConstants.NextColorToMove(color));
            ulong occupied = own | enemy;

            AddPawnMoves(color, own, enemy, occupied, moves);
            AddKnightMoves(color, own, enemy, moves);
            AddSlidingMoves(color, PieceType.Bishop, own, enemy, occupied, moves);
            AddSlidingMoves(color, PieceType.Rook, own, enemy, occupied, moves);
            AddSlidingMoves(color, PieceType.Queen, own, enemy, occupied, moves);
            AddKingMoves(color, own, enemy, moves);

            return moves;
        }

        private void AddPawnMoves(Color color, ulong own, ulong enemy, ulong occupied, List<BitMove> moves)
        {
            ulong pawns = _pieceBoards[(int)(color == Color.White ? PieceIndex.WhitePawn : PieceIndex.BlackPawn)];
            int direction = color == Color.White ? 8 : -8;
            int startRank = color == Color.White ? 1 : 6;
            int promotionRank = color == Color.White ? 7 : 0;

            for (int from = 0; from < 64; from++)
            {
                ulong fromMask = 1UL << from;
                if ((pawns & fromMask) == 0)
                    continue;

                int rank = from / 8;
                int file = from % 8;

                int oneStep = from + direction;
                if (IsInBoard(oneStep) && ((occupied & (1UL << oneStep)) == 0))
                {
                    bool promotion = (oneStep / 8) == promotionRank;
                    moves.Add(new BitMove(from, oneStep, PieceType.Pawn, false, promotion));

                    if (rank == startRank)
                    {
                        int twoStep = from + 2 * direction;
                        if (IsInBoard(twoStep) && ((occupied & (1UL << twoStep)) == 0))
                            moves.Add(new BitMove(from, twoStep, PieceType.Pawn, false, false));
                    }
                }

                int captureLeft = from + direction - 1;
                if (file > 0 && IsInBoard(captureLeft) && ((enemy & (1UL << captureLeft)) != 0))
                {
                    bool promotion = (captureLeft / 8) == promotionRank;
                    moves.Add(new BitMove(from, captureLeft, PieceType.Pawn, true, promotion));
                }

                int captureRight = from + direction + 1;
                if (file < 7 && IsInBoard(captureRight) && ((enemy & (1UL << captureRight)) != 0))
                {
                    bool promotion = (captureRight / 8) == promotionRank;
                    moves.Add(new BitMove(from, captureRight, PieceType.Pawn, true, promotion));
                }
            }
        }

        private void AddKnightMoves(Color color, ulong own, ulong enemy, List<BitMove> moves)
        {
            ulong knights = _pieceBoards[(int)(color == Color.White ? PieceIndex.WhiteKnight : PieceIndex.BlackKnight)];
            int[] deltas = { -17, -15, -10, -6, 6, 10, 15, 17 };

            for (int from = 0; from < 64; from++)
            {
                ulong fromMask = 1UL << from;
                if ((knights & fromMask) == 0)
                    continue;

                int fromFile = from % 8;
                int fromRank = from / 8;
                for (int i = 0; i < deltas.Length; i++)
                {
                    int to = from + deltas[i];
                    if (!IsInBoard(to))
                        continue;
                    int toFile = to % 8;
                    int toRank = to / 8;
                    int fileDiff = Math.Abs(toFile - fromFile);
                    int rankDiff = Math.Abs(toRank - fromRank);
                    if (!((fileDiff == 1 && rankDiff == 2) || (fileDiff == 2 && rankDiff == 1)))
                        continue;

                    ulong toMask = 1UL << to;
                    if ((own & toMask) != 0)
                        continue;
                    moves.Add(new BitMove(from, to, PieceType.Knight, (enemy & toMask) != 0, false));
                }
            }
        }

        private void AddSlidingMoves(Color color, PieceType pieceType, ulong own, ulong enemy, ulong occupied, List<BitMove> moves)
        {
            PieceIndex index = PieceToIndex(pieceType, color);
            ulong pieces = _pieceBoards[(int)index];

            (int df, int dr)[] dirs = pieceType switch
            {
                PieceType.Bishop => new[] { (-1, -1), (-1, 1), (1, -1), (1, 1) },
                PieceType.Rook => new[] { (-1, 0), (1, 0), (0, -1), (0, 1) },
                PieceType.Queen => new[] { (-1, -1), (-1, 1), (1, -1), (1, 1), (-1, 0), (1, 0), (0, -1), (0, 1) },
                _ => Array.Empty<(int, int)>()
            };

            for (int from = 0; from < 64; from++)
            {
                ulong fromMask = 1UL << from;
                if ((pieces & fromMask) == 0)
                    continue;

                int file = from % 8;
                int rank = from / 8;
                for (int d = 0; d < dirs.Length; d++)
                {
                    int f = file + dirs[d].df;
                    int r = rank + dirs[d].dr;
                    while (f >= 0 && f < 8 && r >= 0 && r < 8)
                    {
                        int to = r * 8 + f;
                        ulong toMask = 1UL << to;
                        if ((own & toMask) != 0)
                            break;

                        bool capture = (enemy & toMask) != 0;
                        moves.Add(new BitMove(from, to, pieceType, capture, false));
                        if (capture || ((occupied & toMask) != 0))
                            break;

                        f += dirs[d].df;
                        r += dirs[d].dr;
                    }
                }
            }
        }

        private void AddKingMoves(Color color, ulong own, ulong enemy, List<BitMove> moves)
        {
            ulong king = _pieceBoards[(int)(color == Color.White ? PieceIndex.WhiteKing : PieceIndex.BlackKing)];
            if (king == 0UL)
                return;

            int from = BitScanForward(king);
            if (from < 0)
                return;

            int[] deltas = { -9, -8, -7, -1, 1, 7, 8, 9 };
            int fromFile = from % 8;
            int fromRank = from / 8;
            for (int i = 0; i < deltas.Length; i++)
            {
                int to = from + deltas[i];
                if (!IsInBoard(to))
                    continue;

                int toFile = to % 8;
                int toRank = to / 8;
                if (Math.Abs(fromFile - toFile) > 1 || Math.Abs(fromRank - toRank) > 1)
                    continue;

                ulong toMask = 1UL << to;
                if ((own & toMask) != 0)
                    continue;
                moves.Add(new BitMove(from, to, PieceType.King, (enemy & toMask) != 0, false));
            }
        }

        private bool IsInCheck(Color color)
        {
            ulong kingBoard = _pieceBoards[(int)(color == Color.White ? PieceIndex.WhiteKing : PieceIndex.BlackKing)];
            if (kingBoard == 0)
                return true;

            int kingSquare = BitScanForward(kingBoard);
            Color attacker = ChessEngineConstants.NextColorToMove(color);
            return IsSquareAttacked(kingSquare, attacker);
        }

        private bool IsSquareAttacked(int square, Color attacker)
        {
            int targetFile = square % 8;
            int targetRank = square / 8;

            // Pawns
            ulong attackerPawns = _pieceBoards[(int)(attacker == Color.White ? PieceIndex.WhitePawn : PieceIndex.BlackPawn)];
            int pawnDir = attacker == Color.White ? 1 : -1;
            int sourceRank = targetRank - pawnDir;
            if (sourceRank >= 0 && sourceRank < 8)
            {
                if (targetFile > 0)
                {
                    int pawnSquare = sourceRank * 8 + (targetFile - 1);
                    if ((attackerPawns & (1UL << pawnSquare)) != 0)
                        return true;
                }
                if (targetFile < 7)
                {
                    int pawnSquare = sourceRank * 8 + (targetFile + 1);
                    if ((attackerPawns & (1UL << pawnSquare)) != 0)
                        return true;
                }
            }

            // Knights
            ulong attackerKnights = _pieceBoards[(int)(attacker == Color.White ? PieceIndex.WhiteKnight : PieceIndex.BlackKnight)];
            int[] knightDeltas = { -17, -15, -10, -6, 6, 10, 15, 17 };
            for (int i = 0; i < knightDeltas.Length; i++)
            {
                int from = square + knightDeltas[i];
                if (!IsInBoard(from))
                    continue;
                int fileDiff = Math.Abs((from % 8) - targetFile);
                int rankDiff = Math.Abs((from / 8) - targetRank);
                if ((fileDiff == 1 && rankDiff == 2) || (fileDiff == 2 && rankDiff == 1))
                {
                    if ((attackerKnights & (1UL << from)) != 0)
                        return true;
                }
            }

            // King
            ulong attackerKing = _pieceBoards[(int)(attacker == Color.White ? PieceIndex.WhiteKing : PieceIndex.BlackKing)];
            if (attackerKing != 0)
            {
                int kingSquare = BitScanForward(attackerKing);
                if (Math.Abs((kingSquare % 8) - targetFile) <= 1 && Math.Abs((kingSquare / 8) - targetRank) <= 1)
                    return true;
            }

            // Sliding attacks
            if (IsAttackedBySliding(square, attacker, PieceType.Bishop) ||
                IsAttackedBySliding(square, attacker, PieceType.Rook) ||
                IsAttackedBySliding(square, attacker, PieceType.Queen))
            {
                return true;
            }

            return false;
        }

        private bool IsAttackedBySliding(int square, Color attacker, PieceType type)
        {
            (int df, int dr)[] dirs = type switch
            {
                PieceType.Bishop => new[] { (-1, -1), (-1, 1), (1, -1), (1, 1) },
                PieceType.Rook => new[] { (-1, 0), (1, 0), (0, -1), (0, 1) },
                PieceType.Queen => new[] { (-1, -1), (-1, 1), (1, -1), (1, 1), (-1, 0), (1, 0), (0, -1), (0, 1) },
                _ => Array.Empty<(int, int)>()
            };

            ulong occupied = Occupancy(Color.White) | Occupancy(Color.Black);
            ulong attackers = _pieceBoards[(int)PieceToIndex(type, attacker)];

            int file = square % 8;
            int rank = square / 8;
            for (int i = 0; i < dirs.Length; i++)
            {
                int f = file + dirs[i].df;
                int r = rank + dirs[i].dr;
                while (f >= 0 && f < 8 && r >= 0 && r < 8)
                {
                    int current = r * 8 + f;
                    ulong mask = 1UL << current;
                    if ((occupied & mask) == 0)
                    {
                        f += dirs[i].df;
                        r += dirs[i].dr;
                        continue;
                    }

                    if ((attackers & mask) != 0)
                        return true;
                    break;
                }
            }

            return false;
        }

        private void ApplyMove(BitMove move)
        {
            Color mover = _colorToMove;
            PieceIndex movingIndex = PieceToIndex(move.PieceType, mover);

            ulong fromMask = 1UL << move.From;
            ulong toMask = 1UL << move.To;

            _pieceBoards[(int)movingIndex] &= ~fromMask;

            Color enemyColor = ChessEngineConstants.NextColorToMove(mover);
            if (move.IsCapture)
            {
                for (int i = PieceStartIndex(enemyColor); i <= PieceEndIndex(enemyColor); i++)
                {
                    if ((_pieceBoards[i] & toMask) != 0)
                    {
                        _pieceBoards[i] &= ~toMask;
                        break;
                    }
                }
            }

            if (move.IsPromotion)
            {
                PieceIndex promoted = PieceToIndex(PieceType.Queen, mover);
                _pieceBoards[(int)promoted] |= toMask;
            }
            else
            {
                _pieceBoards[(int)movingIndex] |= toMask;
            }
        }

        private Move BuildExternalMove(BitMove move, BoardRating rating)
        {
            Position start = SquareToPosition(move.From);
            Position end = SquareToPosition(move.To);
            IPiece piece = CreateDisplayPiece(move.PieceType, _colorToMove, start);

            MoveType moveType = MoveType.Normal;
            if (move.IsCapture)
                moveType |= MoveType.Capture;
            if (move.IsPromotion)
                moveType |= MoveType.Promotion;

            Move external = new Move(start, end, piece, moveType)
            {
                Rating = rating
            };
            return external;
        }

        private int EvaluateMaterial()
        {
            int value = 0;
            value += MaterialValue(PieceType.Pawn) * (PopCount(_pieceBoards[(int)PieceIndex.WhitePawn]) - PopCount(_pieceBoards[(int)PieceIndex.BlackPawn]));
            value += MaterialValue(PieceType.Knight) * (PopCount(_pieceBoards[(int)PieceIndex.WhiteKnight]) - PopCount(_pieceBoards[(int)PieceIndex.BlackKnight]));
            value += MaterialValue(PieceType.Bishop) * (PopCount(_pieceBoards[(int)PieceIndex.WhiteBishop]) - PopCount(_pieceBoards[(int)PieceIndex.BlackBishop]));
            value += MaterialValue(PieceType.Rook) * (PopCount(_pieceBoards[(int)PieceIndex.WhiteRook]) - PopCount(_pieceBoards[(int)PieceIndex.BlackRook]));
            value += MaterialValue(PieceType.Queen) * (PopCount(_pieceBoards[(int)PieceIndex.WhiteQueen]) - PopCount(_pieceBoards[(int)PieceIndex.BlackQueen]));
            return value;
        }

        private static int MaterialValue(PieceType type)
        {
            return type switch
            {
                PieceType.Pawn => ChessEngineConstants.Pawn,
                PieceType.Knight => ChessEngineConstants.Knight,
                PieceType.Bishop => ChessEngineConstants.Bishop,
                PieceType.Rook => ChessEngineConstants.Rook,
                PieceType.Queen => ChessEngineConstants.Queen,
                PieceType.King => ChessEngineConstants.King,
                _ => 0
            };
        }

        private BoardRating BuildRating(Color perspective, int weight)
        {
            var rating = new BoardRating
            {
                Weight = weight,
                Evaluation = Evaluation.Normal,
                Situation = Situation.Normal,
                Depth = 0
            };

            if (_pieceBoards[(int)PieceIndex.WhiteKing] == 0)
            {
                rating.Situation = Situation.BlackVictory;
                rating.Evaluation = Evaluation.WhiteCheckMate;
                rating.Weight = -ChessEngineConstants.CheckMate;
                return rating;
            }

            if (_pieceBoards[(int)PieceIndex.BlackKing] == 0)
            {
                rating.Situation = Situation.WhiteVictory;
                rating.Evaluation = Evaluation.BlackCheckMate;
                rating.Weight = ChessEngineConstants.CheckMate;
                return rating;
            }

            if (IsInCheck(perspective))
            {
                rating.Situation = perspective == Color.White ? Situation.WhiteChecked : Situation.BlackChecked;
            }

            return rating;
        }

        private BoardRating BuildTerminalRatingForNoMoves(Color sideToMove)
        {
            if (IsInCheck(sideToMove))
            {
                return new BoardRating
                {
                    Situation = sideToMove == Color.White ? Situation.BlackVictory : Situation.WhiteVictory,
                    Evaluation = sideToMove == Color.White ? Evaluation.WhiteCheckMate : Evaluation.BlackCheckMate,
                    Weight = sideToMove == Color.White ? -ChessEngineConstants.CheckMate : ChessEngineConstants.CheckMate
                };
            }

            return new BoardRating
            {
                Situation = Situation.StaleMate,
                Evaluation = Evaluation.Remis,
                Weight = 0
            };
        }

        private BoardState CaptureState()
        {
            ulong[] copy = new ulong[12];
            Array.Copy(_pieceBoards, copy, 12);
            return new BoardState(copy, _colorToMove);
        }

        private void RestoreState(BoardState state)
        {
            Array.Copy(state.PieceBoards, _pieceBoards, 12);
            _colorToMove = state.ColorToMove;
        }

        private static bool IsInBoard(int square) => square >= 0 && square < 64;

        private static int SquareIndex(Position position)
        {
            if (!position.IsValidPosition())
                return -1;
            return position.Row * 8 + position.Column;
        }

        private static Position SquareToPosition(int square)
        {
            int row = square / 8;
            int column = square % 8;
            return new Position(column, row);
        }

        private ulong Occupancy(Color color)
        {
            ulong value = 0UL;
            for (int i = PieceStartIndex(color); i <= PieceEndIndex(color); i++)
                value |= _pieceBoards[i];
            return value;
        }

        private static int PieceStartIndex(Color color) => color == Color.White ? (int)PieceIndex.WhitePawn : (int)PieceIndex.BlackPawn;
        private static int PieceEndIndex(Color color) => color == Color.White ? (int)PieceIndex.WhiteKing : (int)PieceIndex.BlackKing;

        private static PieceIndex PieceToIndex(PieceType pieceType, Color color)
        {
            if (color == Color.White)
            {
                return pieceType switch
                {
                    PieceType.Pawn => PieceIndex.WhitePawn,
                    PieceType.Knight => PieceIndex.WhiteKnight,
                    PieceType.Bishop => PieceIndex.WhiteBishop,
                    PieceType.Rook => PieceIndex.WhiteRook,
                    PieceType.Queen => PieceIndex.WhiteQueen,
                    PieceType.King => PieceIndex.WhiteKing,
                    _ => PieceIndex.WhitePawn
                };
            }

            return pieceType switch
            {
                PieceType.Pawn => PieceIndex.BlackPawn,
                PieceType.Knight => PieceIndex.BlackKnight,
                PieceType.Bishop => PieceIndex.BlackBishop,
                PieceType.Rook => PieceIndex.BlackRook,
                PieceType.Queen => PieceIndex.BlackQueen,
                PieceType.King => PieceIndex.BlackKing,
                _ => PieceIndex.BlackPawn
            };
        }

        private static PieceType IndexToPieceType(PieceIndex index)
        {
            return index switch
            {
                PieceIndex.WhitePawn or PieceIndex.BlackPawn => PieceType.Pawn,
                PieceIndex.WhiteKnight or PieceIndex.BlackKnight => PieceType.Knight,
                PieceIndex.WhiteBishop or PieceIndex.BlackBishop => PieceType.Bishop,
                PieceIndex.WhiteRook or PieceIndex.BlackRook => PieceType.Rook,
                PieceIndex.WhiteQueen or PieceIndex.BlackQueen => PieceType.Queen,
                PieceIndex.WhiteKing or PieceIndex.BlackKing => PieceType.King,
                _ => PieceType.Pawn
            };
        }

        private static int PopCount(ulong value)
        {
            int count = 0;
            while (value != 0)
            {
                value &= value - 1;
                count++;
            }
            return count;
        }

        private static int BitScanForward(ulong bitboard)
        {
            if (bitboard == 0)
                return -1;

            int index = 0;
            while ((bitboard & 1UL) == 0)
            {
                bitboard >>= 1;
                index++;
            }
            return index;
        }

        private static IPiece CreateDisplayPiece(PieceType type, Color color, Position position)
        {
            return type switch
            {
                PieceType.Pawn => new Pawn(color, position),
                PieceType.Knight => new Knight(color, position),
                PieceType.Bishop => new Bishop(color, position),
                PieceType.Rook => new Rook(color, position),
                PieceType.Queen => new Queen(color, position),
                PieceType.King => new King(color, position),
                _ => null
            };
        }
    }
}
