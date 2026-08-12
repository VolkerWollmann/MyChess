using System.Text;

namespace MyBitboardChessEngine
{
    /// Iterative-deepening negamax engine built around the incrementally
    /// maintained Zobrist key: every node probes the shared transposition
    /// table, stored results are reusable at any smaller depth (the
    /// evaluation is a pure function of the position, mate scores are stored
    /// node-relative), and the stored best move is searched first - which is
    /// where a transposition table earns most of its cutoffs.
    public sealed class BitboardChessEngine
    {
        public readonly Board Board = new();
        public TranspositionTable Table { get; } = new();

        public long Nodes { get; private set; }

        #region Search state

        private readonly List<Move>[] MoveBuffers;
        private readonly int[][] ScoreBuffers;
        private readonly Move[,] PvTable = new Move[Constants.MaxPly, Constants.MaxPly];
        private readonly int[] PvLength = new int[Constants.MaxPly];
        private readonly ushort[,] Killers = new ushort[Constants.MaxPly, 2];
        private readonly int[,,] History = new int[2, 64, 64];

        #endregion

        public BitboardChessEngine()
        {
            MoveBuffers = new List<Move>[Constants.MaxPly];
            ScoreBuffers = new int[Constants.MaxPly][];
            for (int ply = 0; ply < Constants.MaxPly; ply++)
            {
                MoveBuffers[ply] = new List<Move>(64);
                ScoreBuffers[ply] = new int[256];
            }
        }

        #region Game surface

        public void New()
        {
            Board.New();
            Table.Clear();
        }

        public void Clear()
        {
            Board.Clear();
            Table.Clear();
        }

        public int ColorToMove
        {
            get => Board.SideToMove;
            set => Board.SetSideToMove(value);
        }

        public void SetPiece(int piece, string square) => Board.SetPiece(piece, square);

        public void SetCastleRights(CastleRights rights) => Board.SetCastleRights(rights);

        public List<Move> GetLegalMoves()
        {
            List<Move> pseudo = new(64);
            Board.GenerateMoves(pseudo);

            List<Move> legal = new(pseudo.Count);
            foreach (Move move in pseudo)
            {
                Board.MakeMove(move);
                if (!Board.InCheck(Board.SideToMove ^ 1))
                    legal.Add(move);
                Board.UnmakeMove();
            }
            return legal;
        }

        /// Plays a move permanently (game move, stays in the key history).
        /// The move is matched against the legal moves, so castling, en
        /// passant and promotion flags are attached automatically.
        public bool ExecuteMove(string from, string to)
        {
            return ExecuteMove(Constants.SquareOf(from), Constants.SquareOf(to));
        }

        public bool ExecuteMove(int from, int to)
        {
            foreach (Move move in GetLegalMoves())
            {
                if (move.From == from && move.To == to)
                {
                    Board.MakeMove(move);
                    return true;
                }
            }
            return false;
        }

        public Rating GetRating() => Board.GetRating();

        #endregion

        #region Search

        public EngineMove? CalculateMove(int depth = Constants.DefaultSearchDepth)
        {
            depth = Math.Clamp(depth, 1, 64);

            if (Board.KingSquare(Board.SideToMove) < 0 || Board.KingSquare(Board.SideToMove ^ 1) < 0)
                return null; // game is already over

            if (GetLegalMoves().Count == 0)
                return null; // checkmate or stalemate, no move to return

            Nodes = 0;
            Table.NewSearch();
            Array.Clear(Killers);
            Array.Clear(History);

            // Iterative deepening: each iteration seeds the next one's move
            // ordering through the table's best moves, so the deeper searches
            // run with near-optimal ordering - re-searching shallow depths is
            // far cheaper than a badly ordered deep search.
            int value = 0;
            for (int iteration = 1; iteration <= depth; iteration++)
                value = Negamax(iteration, 0, -Constants.Infinity, Constants.Infinity);

            Move bestMove = PvTable[0, 0];

            int side = Board.SideToMove;
            int whiteValue = side == Constants.White ? value : -value;

            GameState state = GameState.Normal;
            if (value >= Constants.MateThreshold)
                state = side == Constants.White ? GameState.BlackLoss : GameState.WhiteLoss;
            else if (value <= -Constants.MateThreshold)
                state = side == Constants.White ? GameState.WhiteLoss : GameState.BlackLoss;

            Rating rating = new(whiteValue, state) { MoveList = PrincipalVariation() };
            return new EngineMove(bestMove, rating);
        }

        /// Interface parity with the other engines: iterative deepening with a
        /// shared table makes the sequential search fast enough that splitting
        /// root moves over threads (with the ordering information lost between
        /// them) is not worth it here.
        public EngineMove? CalculateMoveParallel(int depth = Constants.DefaultSearchDepth)
        {
            return CalculateMove(depth);
        }

        private string PrincipalVariation()
        {
            StringBuilder line = new();
            for (int i = 0; i < PvLength[0]; i++)
            {
                if (i > 0)
                    line.Append(';');
                line.Append(PvTable[0, i]);
            }
            return line.ToString();
        }

        private int Negamax(int depth, int ply, int alpha, int beta)
        {
            Nodes++;
            PvLength[ply] = ply;

            if (ply >= Constants.MaxPly - 1)
                return Board.Evaluate();

            int side = Board.SideToMove;
            int enemy = side ^ 1;

            // King-capture tolerance: positions set up from outside can be
            // illegal (a king en prise, or already gone). A missing king is an
            // immediate, root-relative loss - faster kills rank higher.
            if (Board.KingSquare(side) < 0)
                return -(Constants.Mate - ply);
            if (Board.KingSquare(enemy) < 0)
                return Constants.Mate - ply;

            // Repetition is a draw. Checked before the table: a stored value
            // from a repetition-free path must not override the draw on this
            // path, and draw-by-path values are never stored.
            if (ply > 0 && Board.IsRepetition())
                return 0;

            ulong key = Board.Key;
            int alphaOriginal = alpha;

            ushort tableMove = 0;
            if (Table.Probe(key, out int tableValue, out tableMove, out int tableDepth, out Bound tableBound))
            {
                // Cross-depth cutoff: valid because every stored value is a
                // pure function of the position. The root never cuts - it must
                // deliver a move.
                if (ply > 0 && tableDepth >= depth)
                {
                    int v = ValueFromTable(tableValue, ply);
                    if (tableBound == Bound.Exact)
                        return v;
                    if (tableBound == Bound.Lower && v > alpha)
                        alpha = v;
                    else if (tableBound == Bound.Upper && v < beta)
                        beta = v;
                    if (alpha >= beta)
                        return v;
                }
            }

            if (depth <= 0)
            {
                int eval = Board.Evaluate();
                Table.Store(key, 0, eval, Bound.Exact, 0);
                return eval;
            }

            List<Move> moves = MoveBuffers[ply];
            Board.GenerateMoves(moves);
            int[] scores = ScoreBuffers[ply];
            ScoreMoves(moves, scores, tableMove, ply, side);

            Move bestMove = default;
            int best = -Constants.Infinity;
            int legalMoves = 0;

            for (int i = 0; i < moves.Count; i++)
            {
                PickBest(moves, scores, i);
                Move move = moves[i];

                Board.MakeMove(move);
                if (Board.InCheck(side))
                {
                    Board.UnmakeMove();
                    continue; // leaves the own king in check
                }

                legalMoves++;
                int value = -Negamax(depth - 1, ply + 1, -beta, -alpha);
                Board.UnmakeMove();

                if (value > best)
                {
                    best = value;
                    bestMove = move;

                    if (value > alpha)
                    {
                        alpha = value;
                        UpdatePrincipalVariation(ply, move);

                        if (alpha >= beta)
                        {
                            if (!move.IsCapture)
                            {
                                // quiet move refuted the line: remember it for
                                // this ply (killers) and globally (history)
                                if (Killers[ply, 0] != move.Packed)
                                {
                                    Killers[ply, 1] = Killers[ply, 0];
                                    Killers[ply, 0] = move.Packed;
                                }
                                History[side, move.From, move.To] += depth * depth;
                            }
                            break;
                        }
                    }
                }
            }

            if (legalMoves == 0)
            {
                // Checkmate (root-relative, faster mates rank higher) or
                // stalemate. True terminal results hold at every depth, so
                // they are stored with maximum draft.
                int terminal = Board.InCheck(side) ? -(Constants.Mate - ply) : 0;
                Table.Store(key, 64, ValueToTable(terminal, ply), Bound.Exact, 0);
                return terminal;
            }

            Bound bound = best <= alphaOriginal ? Bound.Upper
                : best >= beta ? Bound.Lower
                : Bound.Exact;
            Table.Store(key, depth, ValueToTable(best, ply), bound, bestMove.Packed);

            return best;
        }

        /// Mate scores are root-relative in the search but node-relative in
        /// the table, so an entry stored at one ply stays correct when the
        /// same position is reached at another ply.
        private static int ValueToTable(int value, int ply)
        {
            if (value >= Constants.MateThreshold) return value + ply;
            if (value <= -Constants.MateThreshold) return value - ply;
            return value;
        }

        private static int ValueFromTable(int value, int ply)
        {
            if (value >= Constants.MateThreshold) return value - ply;
            if (value <= -Constants.MateThreshold) return value + ply;
            return value;
        }

        private void UpdatePrincipalVariation(int ply, Move move)
        {
            PvTable[ply, ply] = move;
            for (int next = ply + 1; next < PvLength[ply + 1]; next++)
                PvTable[ply, next] = PvTable[ply + 1, next];
            PvLength[ply] = Math.Max(PvLength[ply + 1], ply + 1);
        }

        /// Move ordering: table move, then captures (most valuable victim,
        /// least valuable attacker), promotions, killers, then quiet moves by
        /// history score. Good ordering is what turns the table's stored best
        /// moves into early beta cutoffs.
        private void ScoreMoves(List<Move> moves, int[] scores, ushort tableMove, int ply, int side)
        {
            for (int i = 0; i < moves.Count; i++)
            {
                Move move = moves[i];
                int score;

                if (tableMove != 0 && move.Packed == tableMove)
                    score = int.MaxValue;
                else if (move.Flag == MoveFlag.EnPassant)
                    score = 1_000_000 + Constants.PieceValues[Constants.Pawn] * 16;
                else if (move.IsCapture)
                    score = 1_000_000
                            + Constants.PieceValues[Constants.TypeOf(move.Captured)] * 16
                            - Constants.TypeOf(move.Piece);
                else if (move.Flag == MoveFlag.Promotion)
                    score = 900_000;
                else if (move.Packed == Killers[ply, 0])
                    score = 800_000;
                else if (move.Packed == Killers[ply, 1])
                    score = 799_000;
                else
                    score = History[side, move.From, move.To];

                scores[i] = score;
            }
        }

        /// Selection step: swaps the highest-scored remaining move to
        /// position <paramref name="index"/> - cheaper than sorting when a
        /// cutoff usually ends the loop after a few moves.
        private static void PickBest(List<Move> moves, int[] scores, int index)
        {
            int best = index;
            for (int i = index + 1; i < moves.Count; i++)
            {
                if (scores[i] > scores[best])
                    best = i;
            }

            if (best != index)
            {
                (moves[index], moves[best]) = (moves[best], moves[index]);
                (scores[index], scores[best]) = (scores[best], scores[index]);
            }
        }

        #endregion
    }
}
