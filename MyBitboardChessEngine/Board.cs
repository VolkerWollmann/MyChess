using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace MyBitboardChessEngine
{
    /// Bitboard position: one 64-bit mask per piece code plus a mailbox for
    /// square lookups. MakeMove/UnmakeMove maintain material, occupancy and
    /// the Zobrist key incrementally; the key history provides repetition
    /// detection for free.
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class Board
    {
        internal readonly ulong[] Pieces = new ulong[12];
        internal readonly ulong[] Occupied = new ulong[2];
        internal readonly int[] Mailbox = new int[64];

        public int SideToMove;
        public CastleRights Rights;

        /// Square behind a pawn that just double-stepped, -1 if none.
        public int EnPassantSquare = -1;

        /// Plies since the last pawn move or capture; bounds the repetition scan.
        public int HalfmoveClock;

        public int GamePly;

        /// Incrementally maintained Zobrist key of the current position.
        public ulong Key;

        /// White-positive material sum, incrementally maintained.
        public int Material;

        /// Key after every position since the last reset, current key last.
        /// Game moves and search make/unmake share this list, so repetitions
        /// against the game history are found inside the search as well.
        internal readonly List<ulong> KeyHistory = new(512);

        public ulong AllOccupied => Occupied[0] | Occupied[1];

        #region Undo stack

        private struct Undo
        {
            public Move Move;
            public CastleRights Rights;
            public int EnPassantSquare;
            public int HalfmoveClock;
            public ulong Key;
        }

        private readonly Undo[] UndoStack = new Undo[1024];
        private int UndoCount;

        #endregion

        // Which castle rights survive a move touching this square (king or
        // rook squares clear rights; capturing a rook on its start square too).
        private static readonly CastleRights[] RightsMask = BuildRightsMask();

        private static CastleRights[] BuildRightsMask()
        {
            CastleRights[] mask = new CastleRights[64];
            Array.Fill(mask, CastleRights.All);
            mask[Constants.SquareOf("E1")] = CastleRights.All & ~(CastleRights.WhiteKingSide | CastleRights.WhiteQueenSide);
            mask[Constants.SquareOf("A1")] = CastleRights.All & ~CastleRights.WhiteQueenSide;
            mask[Constants.SquareOf("H1")] = CastleRights.All & ~CastleRights.WhiteKingSide;
            mask[Constants.SquareOf("E8")] = CastleRights.All & ~(CastleRights.BlackKingSide | CastleRights.BlackQueenSide);
            mask[Constants.SquareOf("A8")] = CastleRights.All & ~CastleRights.BlackQueenSide;
            mask[Constants.SquareOf("H8")] = CastleRights.All & ~CastleRights.BlackKingSide;
            return mask;
        }

        public Board()
        {
            Clear();
        }

        #region Setup

        public void Clear()
        {
            Array.Clear(Pieces);
            Array.Clear(Occupied);
            Array.Fill(Mailbox, Constants.NoPiece);
            SideToMove = Constants.White;
            Rights = CastleRights.None;
            EnPassantSquare = -1;
            HalfmoveClock = 0;
            GamePly = 0;
            Material = 0;
            UndoCount = 0;
            ResetKey();
        }

        public void New()
        {
            Clear();

            int[] backRank = { Constants.Rook, Constants.Knight, Constants.Bishop, Constants.Queen,
                               Constants.King, Constants.Bishop, Constants.Knight, Constants.Rook };
            for (int file = 0; file < 8; file++)
            {
                AddPiece(Constants.MakePiece(backRank[file], Constants.White), file);
                AddPiece(Constants.MakePiece(Constants.Pawn, Constants.White), 8 + file);
                AddPiece(Constants.MakePiece(Constants.Pawn, Constants.Black), 48 + file);
                AddPiece(Constants.MakePiece(backRank[file], Constants.Black), 56 + file);
            }

            Rights = CastleRights.All;
            ResetKey();
        }

        /// Places (or with NoPiece removes) a piece for position setup.
        /// Not for playing moves - use MakeMove/ExecuteMove for that.
        public void SetPiece(int piece, int square)
        {
            if (Mailbox[square] != Constants.NoPiece)
                RemovePiece(square);
            if (piece != Constants.NoPiece)
                AddPiece(piece, square);
            ResetKey();
        }

        public void SetPiece(int piece, string square)
        {
            SetPiece(piece, Constants.SquareOf(square));
        }

        public void SetCastleRights(CastleRights rights)
        {
            Rights = rights;
            ResetKey();
        }

        public void SetSideToMove(int side)
        {
            SideToMove = side;
            ResetKey();
        }

        /// Recomputes the key from scratch and restarts the key history -
        /// called after every setup change; during play the key is incremental.
        internal void ResetKey()
        {
            Key = ComputeKeyFromScratch();
            KeyHistory.Clear();
            KeyHistory.Add(Key);
        }

        internal ulong ComputeKeyFromScratch()
        {
            ulong key = 0;

            for (int square = 0; square < 64; square++)
            {
                if (Mailbox[square] != Constants.NoPiece)
                    key ^= Zobrist.PieceKeys[Mailbox[square], square];
            }

            key ^= Zobrist.RightsKey(Rights);
            key ^= EnPassantKeyTerm();

            if (SideToMove == Constants.Black)
                key ^= Zobrist.SideKey;

            return key;
        }

        /// En passant is hashed only when a capture is actually possible - a
        /// pawn of the side to move stands beside the double-stepped pawn.
        /// Positions in which no en passant move exists either way must not
        /// get different keys just because the marker differs.
        private ulong EnPassantKeyTerm()
        {
            if (EnPassantSquare < 0)
                return 0;

            int pushedSquare = EnPassantSquare + (SideToMove == Constants.White ? -8 : 8);
            ulong pushed = 1UL << pushedSquare;
            ulong adjacent = ((pushed & ~Attacks.FileA) >> 1) | ((pushed & ~Attacks.FileH) << 1);

            if ((adjacent & Pieces[Constants.MakePiece(Constants.Pawn, SideToMove)]) == 0)
                return 0;

            return Zobrist.EnPassantFileKeys[EnPassantSquare & 7];
        }

        #endregion

        #region Piece accounting

        // AddPiece/RemovePiece maintain bitboards, mailbox and material but
        // never the key: MakeMove does its own incremental key updates and
        // UnmakeMove restores the saved key, so unmaking stays key-free.
        private void AddPiece(int piece, int square)
        {
            ulong bit = 1UL << square;
            Pieces[piece] |= bit;
            Occupied[Constants.ColorOf(piece)] |= bit;
            Mailbox[square] = piece;

            int value = Constants.PieceValues[Constants.TypeOf(piece)];
            Material += Constants.ColorOf(piece) == Constants.White ? value : -value;
        }

        private void RemovePiece(int square)
        {
            int piece = Mailbox[square];
            ulong bit = 1UL << square;
            Pieces[piece] &= ~bit;
            Occupied[Constants.ColorOf(piece)] &= ~bit;
            Mailbox[square] = Constants.NoPiece;

            int value = Constants.PieceValues[Constants.TypeOf(piece)];
            Material -= Constants.ColorOf(piece) == Constants.White ? value : -value;
        }

        public int PieceAt(int square) => Mailbox[square];

        public int KingSquare(int color)
        {
            ulong king = Pieces[Constants.MakePiece(Constants.King, color)];
            return king == 0 ? -1 : BitOperations.TrailingZeroCount(king);
        }

        #endregion

        #region Make / Unmake

        public void MakeMove(Move move)
        {
            UndoStack[UndoCount++] = new Undo
            {
                Move = move,
                Rights = Rights,
                EnPassantSquare = EnPassantSquare,
                HalfmoveClock = HalfmoveClock,
                Key = Key
            };

            int side = SideToMove;
            int enemy = side ^ 1;

            // XOR out the state terms that are about to change
            ulong key = Key ^ EnPassantKeyTerm() ^ Zobrist.RightsKey(Rights);

            // captures (en passant captures beside the target square)
            if (move.Flag == MoveFlag.EnPassant)
            {
                int captureSquare = move.To + (side == Constants.White ? -8 : 8);
                key ^= Zobrist.PieceKeys[Mailbox[captureSquare], captureSquare];
                RemovePiece(captureSquare);
            }
            else if (move.Captured != Constants.NoPiece)
            {
                key ^= Zobrist.PieceKeys[move.Captured, move.To];
                RemovePiece(move.To);
            }

            // the moving piece (a promotion arrives as a queen)
            int arriving = move.Flag == MoveFlag.Promotion
                ? Constants.MakePiece(Constants.Queen, side)
                : move.Piece;

            key ^= Zobrist.PieceKeys[move.Piece, move.From];
            key ^= Zobrist.PieceKeys[arriving, move.To];
            RemovePiece(move.From);
            AddPiece(arriving, move.To);

            // castling moves the rook as well
            if (move.Flag == MoveFlag.CastleKingSide)
            {
                int rookFrom = move.To + 1;
                int rookTo = move.To - 1;
                key ^= Zobrist.PieceKeys[Mailbox[rookFrom], rookFrom] ^ Zobrist.PieceKeys[Mailbox[rookFrom], rookTo];
                int rook = Mailbox[rookFrom];
                RemovePiece(rookFrom);
                AddPiece(rook, rookTo);
            }
            else if (move.Flag == MoveFlag.CastleQueenSide)
            {
                int rookFrom = move.To - 2;
                int rookTo = move.To + 1;
                key ^= Zobrist.PieceKeys[Mailbox[rookFrom], rookFrom] ^ Zobrist.PieceKeys[Mailbox[rookFrom], rookTo];
                int rook = Mailbox[rookFrom];
                RemovePiece(rookFrom);
                AddPiece(rook, rookTo);
            }

            Rights &= RightsMask[move.From] & RightsMask[move.To];
            EnPassantSquare = move.Flag == MoveFlag.DoublePush
                ? move.From + (side == Constants.White ? 8 : -8)
                : -1;

            HalfmoveClock = move.IsCapture || Constants.TypeOf(move.Piece) == Constants.Pawn
                ? 0
                : HalfmoveClock + 1;

            SideToMove = enemy;
            GamePly++;

            // XOR in the new state terms
            key ^= Zobrist.RightsKey(Rights) ^ Zobrist.SideKey;
            Key = key;
            Key ^= EnPassantKeyTerm();

            KeyHistory.Add(Key);
        }

        public void UnmakeMove()
        {
            Undo undo = UndoStack[--UndoCount];
            Move move = undo.Move;

            SideToMove ^= 1;
            GamePly--;
            int side = SideToMove;
            int enemy = side ^ 1;

            // take the moving piece back (a promotion turns back into a pawn)
            RemovePiece(move.To);
            AddPiece(move.Flag == MoveFlag.Promotion
                ? Constants.MakePiece(Constants.Pawn, side)
                : move.Piece, move.From);

            if (move.Flag == MoveFlag.EnPassant)
                AddPiece(Constants.MakePiece(Constants.Pawn, enemy), move.To + (side == Constants.White ? -8 : 8));
            else if (move.Captured != Constants.NoPiece)
                AddPiece(move.Captured, move.To);

            if (move.Flag == MoveFlag.CastleKingSide)
            {
                int rook = Mailbox[move.To - 1];
                RemovePiece(move.To - 1);
                AddPiece(rook, move.To + 1);
            }
            else if (move.Flag == MoveFlag.CastleQueenSide)
            {
                int rook = Mailbox[move.To + 1];
                RemovePiece(move.To + 1);
                AddPiece(rook, move.To - 2);
            }

            Rights = undo.Rights;
            EnPassantSquare = undo.EnPassantSquare;
            HalfmoveClock = undo.HalfmoveClock;
            Key = undo.Key;
            KeyHistory.RemoveAt(KeyHistory.Count - 1);
        }

        /// True if the current position occurred before on the path/in the
        /// game. One prior occurrence counts: heading for a repetition is
        /// already a draw the opponent can force.
        public bool IsRepetition()
        {
            int last = KeyHistory.Count - 1;
            int lowest = Math.Max(0, last - HalfmoveClock);
            for (int i = last - 2; i >= lowest; i -= 2)
            {
                if (KeyHistory[i] == Key)
                    return true;
            }
            return false;
        }

        #endregion

        #region Attacks and checks

        public bool IsSquareAttacked(int square, int byColor)
        {
            if ((Attacks.KnightAttacks[square] & Pieces[Constants.MakePiece(Constants.Knight, byColor)]) != 0)
                return true;
            if ((Attacks.KingAttacks[square] & Pieces[Constants.MakePiece(Constants.King, byColor)]) != 0)
                return true;
            if ((Attacks.PawnAttacks[byColor ^ 1, square] & Pieces[Constants.MakePiece(Constants.Pawn, byColor)]) != 0)
                return true;

            ulong occupied = AllOccupied;
            ulong rookLike = Pieces[Constants.MakePiece(Constants.Rook, byColor)]
                             | Pieces[Constants.MakePiece(Constants.Queen, byColor)];
            if ((Attacks.RookAttacks(square, occupied) & rookLike) != 0)
                return true;

            ulong bishopLike = Pieces[Constants.MakePiece(Constants.Bishop, byColor)]
                               | Pieces[Constants.MakePiece(Constants.Queen, byColor)];
            return (Attacks.BishopAttacks(square, occupied) & bishopLike) != 0;
        }

        public bool InCheck(int color)
        {
            int king = KingSquare(color);
            return king >= 0 && IsSquareAttacked(king, color ^ 1);
        }

        /// Union of all squares attacked by <paramref name="color"/>.
        internal ulong AttackSet(int color)
        {
            ulong occupied = AllOccupied;
            ulong attacks = 0;

            ulong pawns = Pieces[Constants.MakePiece(Constants.Pawn, color)];
            attacks |= color == Constants.White
                ? ((pawns & ~Attacks.FileA) << 7) | ((pawns & ~Attacks.FileH) << 9)
                : ((pawns & ~Attacks.FileH) >> 7) | ((pawns & ~Attacks.FileA) >> 9);

            ulong knights = Pieces[Constants.MakePiece(Constants.Knight, color)];
            while (knights != 0)
                attacks |= Attacks.KnightAttacks[Attacks.PopLsb(ref knights)];

            ulong king = Pieces[Constants.MakePiece(Constants.King, color)];
            while (king != 0)
                attacks |= Attacks.KingAttacks[Attacks.PopLsb(ref king)];

            ulong bishops = Pieces[Constants.MakePiece(Constants.Bishop, color)]
                            | Pieces[Constants.MakePiece(Constants.Queen, color)];
            while (bishops != 0)
                attacks |= Attacks.BishopAttacks(Attacks.PopLsb(ref bishops), occupied);

            ulong rooks = Pieces[Constants.MakePiece(Constants.Rook, color)]
                          | Pieces[Constants.MakePiece(Constants.Queen, color)];
            while (rooks != 0)
                attacks |= Attacks.RookAttacks(Attacks.PopLsb(ref rooks), occupied);

            return attacks;
        }

        #endregion

        #region Evaluation

        /// Static evaluation from the view of the side to move: material plus
        /// a sub-pawn mobility term counted for BOTH sides. Everything is a
        /// pure function of the position - no path or parity dependence - so
        /// transposition entries are reusable across depths.
        public int Evaluate()
        {
            int mobility = BitOperations.PopCount(AttackSet(Constants.White) & ~Occupied[Constants.White])
                           - BitOperations.PopCount(AttackSet(Constants.Black) & ~Occupied[Constants.Black]);

            int score = Material + Constants.MobilityValue * mobility;
            return SideToMove == Constants.White ? score : -score;
        }

        public Rating GetRating()
        {
            GameState state = GameState.Normal;
            if (KingSquare(Constants.White) < 0)
                state = GameState.WhiteLoss;
            else if (KingSquare(Constants.Black) < 0)
                state = GameState.BlackLoss;

            return new Rating(Material, state);
        }

        #endregion

        #region Move generation

        /// Pseudo-legal moves of the side to move: piece rules, castling
        /// through-check rules and en passant are enforced here; leaving the
        /// own king in check is filtered by the caller after MakeMove. Kings
        /// are capturable like any piece, so illegal positions set up from
        /// outside resolve by king capture instead of crashing.
        internal void GenerateMoves(List<Move> list)
        {
            list.Clear();

            int side = SideToMove;
            ulong own = Occupied[side];
            ulong enemyOcc = Occupied[side ^ 1];
            ulong occupied = own | enemyOcc;

            GeneratePawnMoves(list, side, occupied, enemyOcc);

            int knight = Constants.MakePiece(Constants.Knight, side);
            ulong knights = Pieces[knight];
            while (knights != 0)
            {
                int from = Attacks.PopLsb(ref knights);
                AddTargets(list, from, knight, Attacks.KnightAttacks[from] & ~own);
            }

            int bishop = Constants.MakePiece(Constants.Bishop, side);
            ulong bishops = Pieces[bishop];
            while (bishops != 0)
            {
                int from = Attacks.PopLsb(ref bishops);
                AddTargets(list, from, bishop, Attacks.BishopAttacks(from, occupied) & ~own);
            }

            int rook = Constants.MakePiece(Constants.Rook, side);
            ulong rooks = Pieces[rook];
            while (rooks != 0)
            {
                int from = Attacks.PopLsb(ref rooks);
                AddTargets(list, from, rook, Attacks.RookAttacks(from, occupied) & ~own);
            }

            int queen = Constants.MakePiece(Constants.Queen, side);
            ulong queens = Pieces[queen];
            while (queens != 0)
            {
                int from = Attacks.PopLsb(ref queens);
                AddTargets(list, from, queen, Attacks.QueenAttacks(from, occupied) & ~own);
            }

            int king = Constants.MakePiece(Constants.King, side);
            ulong kings = Pieces[king];
            while (kings != 0)
            {
                int from = Attacks.PopLsb(ref kings);
                AddTargets(list, from, king, Attacks.KingAttacks[from] & ~own);
            }

            GenerateCastles(list, side, occupied);
        }

        private void AddTargets(List<Move> list, int from, int piece, ulong targets)
        {
            while (targets != 0)
            {
                int to = Attacks.PopLsb(ref targets);
                list.Add(new Move(from, to, piece, Mailbox[to]));
            }
        }

        private void GeneratePawnMoves(List<Move> list, int side, ulong occupied, ulong enemyOcc)
        {
            int pawn = Constants.MakePiece(Constants.Pawn, side);
            int up = side == Constants.White ? 8 : -8;
            int startRank = side == Constants.White ? 1 : 6;
            int promotionRank = side == Constants.White ? 7 : 0;

            ulong pawns = Pieces[pawn];
            while (pawns != 0)
            {
                int from = Attacks.PopLsb(ref pawns);

                int to = from + up;
                if (((occupied >> to) & 1) == 0)
                {
                    list.Add(new Move(from, to, pawn,
                        flag: (to >> 3) == promotionRank ? MoveFlag.Promotion : MoveFlag.Normal));

                    if ((from >> 3) == startRank && ((occupied >> (to + up)) & 1) == 0)
                        list.Add(new Move(from, to + up, pawn, flag: MoveFlag.DoublePush));
                }

                ulong captures = Attacks.PawnAttacks[side, from] & enemyOcc;
                while (captures != 0)
                {
                    int captureTo = Attacks.PopLsb(ref captures);
                    list.Add(new Move(from, captureTo, pawn, Mailbox[captureTo],
                        (captureTo >> 3) == promotionRank ? MoveFlag.Promotion : MoveFlag.Normal));
                }

                if (EnPassantSquare >= 0
                    && (Attacks.PawnAttacks[side, from] & (1UL << EnPassantSquare)) != 0)
                {
                    list.Add(new Move(from, EnPassantSquare, pawn,
                        Constants.MakePiece(Constants.Pawn, side ^ 1), MoveFlag.EnPassant));
                }
            }
        }

        /// Castling: right present, king and rook on their start squares, the
        /// squares between empty, and the king neither in check nor crossing
        /// or reaching an attacked square.
        private void GenerateCastles(List<Move> list, int side, ulong occupied)
        {
            int king = Constants.MakePiece(Constants.King, side);
            int rook = Constants.MakePiece(Constants.Rook, side);
            int enemy = side ^ 1;
            int home = side == Constants.White ? 0 : 56;

            if (side == Constants.White
                    ? (Rights & CastleRights.WhiteKingSide) != 0
                    : (Rights & CastleRights.BlackKingSide) != 0)
            {
                if (Mailbox[home + 4] == king && Mailbox[home + 7] == rook
                    && ((occupied >> (home + 5)) & 1) == 0 && ((occupied >> (home + 6)) & 1) == 0
                    && !IsSquareAttacked(home + 4, enemy)
                    && !IsSquareAttacked(home + 5, enemy)
                    && !IsSquareAttacked(home + 6, enemy))
                {
                    list.Add(new Move(home + 4, home + 6, king, flag: MoveFlag.CastleKingSide));
                }
            }

            if (side == Constants.White
                    ? (Rights & CastleRights.WhiteQueenSide) != 0
                    : (Rights & CastleRights.BlackQueenSide) != 0)
            {
                if (Mailbox[home + 4] == king && Mailbox[home] == rook
                    && ((occupied >> (home + 1)) & 1) == 0 && ((occupied >> (home + 2)) & 1) == 0
                    && ((occupied >> (home + 3)) & 1) == 0
                    && !IsSquareAttacked(home + 4, enemy)
                    && !IsSquareAttacked(home + 3, enemy)
                    && !IsSquareAttacked(home + 2, enemy))
                {
                    list.Add(new Move(home + 4, home + 2, king, flag: MoveFlag.CastleQueenSide));
                }
            }
        }

        #endregion

        #region Debugger support

        /// One-line summary for the debugger: side to move, the Zobrist key in
        /// hex, and whether the incrementally maintained key still equals the
        /// from-scratch computation ("key ok" / "KEY DRIFT"). While stepping
        /// INSIDE MakeMove the state is intentionally half-updated, so "KEY
        /// DRIFT" is normal there - at every statement outside make/unmake it
        /// signals a real incremental-update bug.
        private string DebuggerDisplay
        {
            get
            {
                string side = SideToMove == Constants.White ? "White" : "Black";
                string drift = Key == ComputeKeyFromScratch() ? "key ok" : "KEY DRIFT";
                string enPassant = EnPassantSquare >= 0 ? Constants.NameOf(EnPassantSquare) : "-";
                return $"{side} to move Key=0x{Key:X16} ({drift}) Material={Material} "
                       + $"Rights={Rights} ep={enPassant} ply={GamePly}";
            }
        }

        /// Pin this in a watch window for a live board picture: ranks 8..1,
        /// white pieces upper case, black lower case, '.' empty.
        public string Ascii
        {
            get
            {
                const string codes = "PNBRQKpnbrqk";
                StringBuilder text = new();
                for (int rank = 7; rank >= 0; rank--)
                {
                    text.Append(rank + 1).Append("  ");
                    for (int file = 0; file < 8; file++)
                    {
                        int piece = Mailbox[rank * 8 + file];
                        text.Append(piece == Constants.NoPiece ? '.' : codes[piece]).Append(' ');
                    }
                    text.AppendLine();
                }
                text.AppendLine("   A B C D E F G H");
                text.Append(DebuggerDisplay);
                return text.ToString();
            }
        }

        #endregion

        /// Move-path enumeration to the given depth over strictly legal moves -
        /// the standard correctness check for move generation and make/unmake.
        internal long Perft(int depth)
        {
            if (depth == 0)
                return 1;

            List<Move> moves = new(64);
            GenerateMoves(moves);

            long nodes = 0;
            foreach (Move move in moves)
            {
                MakeMove(move);
                if (!InCheck(SideToMove ^ 1))
                    nodes += Perft(depth - 1);
                UnmakeMove();
            }

            return nodes;
        }

        #region FEN

        /// Minimal FEN loader for tests and position setup:
        /// "pieces side castling ep [halfmove fullmove]".
        internal void SetFen(string fen)
        {
            Clear();
            string[] parts = fen.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            const string codes = "PNBRQKpnbrqk";
            int rank = 7, file = 0;
            foreach (char c in parts[0])
            {
                if (c == '/')
                {
                    rank--;
                    file = 0;
                }
                else if (char.IsDigit(c))
                {
                    file += c - '0';
                }
                else
                {
                    AddPiece(codes.IndexOf(c), rank * 8 + file);
                    file++;
                }
            }

            SideToMove = parts.Length > 1 && parts[1] == "b" ? Constants.Black : Constants.White;

            Rights = CastleRights.None;
            if (parts.Length > 2)
            {
                if (parts[2].Contains('K')) Rights |= CastleRights.WhiteKingSide;
                if (parts[2].Contains('Q')) Rights |= CastleRights.WhiteQueenSide;
                if (parts[2].Contains('k')) Rights |= CastleRights.BlackKingSide;
                if (parts[2].Contains('q')) Rights |= CastleRights.BlackQueenSide;
            }

            if (parts.Length > 3 && parts[3] != "-")
                EnPassantSquare = Constants.SquareOf(parts[3].ToUpperInvariant());

            ResetKey();
        }

        #endregion
    }
}
