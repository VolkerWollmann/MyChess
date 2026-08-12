using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyBitboardChessEngine;

namespace EngineUnitTests
{
    /// The BitboardChessEngine is a fresh design (inspired by, not copied
    /// from, the IntegerChessEngine): bitboards, fully legal move generation,
    /// incremental Zobrist keys, iterative deepening and a cross-depth
    /// transposition table. Move generation is verified against published
    /// perft numbers, the incremental key against a from-scratch computation,
    /// and the search against the IntegerChessEngine on forced positions.
    [TestClass]
    public class BitboardChessEngineUnitTests
    {
        #region Move generation (perft against published reference values)

        [TestMethod]
        [DataRow(1, 20L)]
        [DataRow(2, 400L)]
        [DataRow(3, 8_902L)]
        [DataRow(4, 197_281L)]
        [DataRow(5, 4_865_609L)]
        public void PerftStartPosition(int depth, long expected)
        {
            BitboardChessEngine engine = new();
            engine.New();

            Assert.AreEqual(expected, engine.Board.Perft(depth));
        }

        /// "Kiwipete": the classic perft position exercising castling, en
        /// passant, pins and checks. Depths 1-3 contain no promotions, so the
        /// queen-only promotion policy does not affect the counts.
        [TestMethod]
        [DataRow(1, 48L)]
        [DataRow(2, 2_039L)]
        [DataRow(3, 97_862L)]
        public void PerftKiwipete(int depth, long expected)
        {
            BitboardChessEngine engine = new();
            engine.Board.SetFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -");

            Assert.AreEqual(expected, engine.Board.Perft(depth));
        }

        [TestMethod]
        public void KnightInCornerHasTwoMoves()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKnight, "A1");
            engine.SetPiece(Constants.WhiteKing, "E4");
            engine.SetPiece(Constants.BlackKing, "E8");

            int knightMoves = engine.GetLegalMoves().Count(move => move.Piece == Constants.WhiteKnight);

            Assert.AreEqual(2, knightMoves); // only B3 and C2
        }

        [TestMethod]
        public void RookInCornerHasFourteenMoves()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteRook, "A1");
            engine.SetPiece(Constants.WhiteKing, "E4");
            engine.SetPiece(Constants.BlackKing, "E8");

            int rookMoves = engine.GetLegalMoves().Count(move => move.Piece == Constants.WhiteRook);

            Assert.AreEqual(14, rookMoves); // A2-A8 and B1-H1
        }

        [TestMethod]
        public void LoneKingInCornerHasThreeMoves()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "A1");
            engine.SetPiece(Constants.BlackKing, "E8");

            Assert.HasCount(3, engine.GetLegalMoves()); // A2, B1, B2
        }

        [TestMethod]
        public void TwoKingsNoCastlingWhiteToMove()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "E1");
            engine.SetPiece(Constants.BlackKing, "E8");
            engine.ColorToMove = Constants.White;

            var moves = engine.GetLegalMoves();

            Assert.HasCount(5, moves); // D1, D2, E2, F2, F1
            Assert.IsTrue(moves.TrueForAll(move => move.Flag == MoveFlag.Normal));
        }

        [TestMethod]
        public void KingAvoidsThreatenedFields()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "E1");
            engine.SetPiece(Constants.BlackKing, "E8");
            engine.SetPiece(Constants.BlackRook, "D8");
            engine.ColorToMove = Constants.White;

            var moves = engine.GetLegalMoves();

            // D1 and D2 are covered by the rook on D8
            Assert.HasCount(3, moves); // E2, F1, F2
            Assert.IsTrue(moves.TrueForAll(move => (move.To & 7) != 3));
        }

        [TestMethod]
        public void PinnedPieceMayNotMove()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "E1");
            engine.SetPiece(Constants.WhiteKnight, "E2"); // pinned by the rook on E8
            engine.SetPiece(Constants.BlackKing, "A8");
            engine.SetPiece(Constants.BlackRook, "E8");
            engine.ColorToMove = Constants.White;

            // The fully legal move generation must not offer any knight move
            Assert.IsFalse(engine.GetLegalMoves().Any(move => move.Piece == Constants.WhiteKnight));
        }

        [TestMethod]
        public void CastleBlockedOnThreatenedField()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "E1");
            engine.SetPiece(Constants.WhiteRook, "A1");
            engine.SetPiece(Constants.WhiteRook, "H1");
            engine.SetPiece(Constants.BlackKing, "E8");
            engine.SetPiece(Constants.BlackRook, "F8");
            engine.SetCastleRights(CastleRights.WhiteKingSide | CastleRights.WhiteQueenSide);
            engine.ColorToMove = Constants.White;

            var moves = engine.GetLegalMoves();

            // F1 is covered, the king may not pass it on the king side
            Assert.IsFalse(moves.Any(move => move.Flag == MoveFlag.CastleKingSide));
            Assert.IsTrue(moves.Any(move => move.Flag == MoveFlag.CastleQueenSide));
        }

        [TestMethod]
        public void CastleBlockedByPieceInBetween()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "E1");
            engine.SetPiece(Constants.WhiteRook, "A1");
            engine.SetPiece(Constants.WhiteRook, "H1");
            engine.SetPiece(Constants.WhiteBishop, "F1");
            engine.SetPiece(Constants.BlackKing, "E8");
            engine.SetCastleRights(CastleRights.WhiteKingSide | CastleRights.WhiteQueenSide);
            engine.ColorToMove = Constants.White;

            var moves = engine.GetLegalMoves();

            Assert.IsFalse(moves.Any(move => move.Flag == MoveFlag.CastleKingSide));
            Assert.IsTrue(moves.Any(move => move.Flag == MoveFlag.CastleQueenSide));
        }

        [TestMethod]
        public void CastleBlockedWhenKingIsChecked()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "E1");
            engine.SetPiece(Constants.WhiteRook, "A1");
            engine.SetPiece(Constants.WhiteRook, "H1");
            engine.SetPiece(Constants.BlackKing, "A8");
            engine.SetPiece(Constants.BlackRook, "E8");
            engine.SetCastleRights(CastleRights.WhiteKingSide | CastleRights.WhiteQueenSide);
            engine.ColorToMove = Constants.White;

            Assert.IsFalse(engine.GetLegalMoves().Any(
                move => move.Flag == MoveFlag.CastleKingSide || move.Flag == MoveFlag.CastleQueenSide));
        }

        [TestMethod]
        public void MovingTheRookRemovesTheCastleRight()
        {
            BitboardChessEngine engine = new();
            engine.New();

            engine.ExecuteMove("A2", "A4");
            engine.ExecuteMove("A7", "A5");
            engine.ExecuteMove("A1", "A3"); // white queen side rook moves out
            engine.ExecuteMove("A8", "A6"); // black queen side rook moves out

            Assert.AreEqual(CastleRights.WhiteKingSide | CastleRights.BlackKingSide, engine.Board.Rights);
        }

        #endregion

        #region Zobrist key invariants

        /// The incrementally maintained key must equal the from-scratch
        /// computation after every kind of move. A short game covering
        /// double steps, en passant, castling, captures and promotion.
        [TestMethod]
        public void IncrementalKeyMatchesScratchComputation()
        {
            BitboardChessEngine engine = new();
            engine.New();

            (string from, string to)[] game =
            {
                ("E2", "E4"), ("D7", "D5"),
                ("E4", "E5"), ("F7", "F5"),
                ("E5", "F6"), // en passant
                ("G8", "F6"),
                ("G1", "F3"), ("B8", "C6"),
                ("F1", "B5"), ("C8", "G4"),
                ("E1", "G1"), // white castles king side
                ("D8", "D6"),
                ("F3", "E5"), ("E8", "C8") // black castles queen side
            };

            foreach ((string from, string to) in game)
            {
                Assert.IsTrue(engine.ExecuteMove(from, to), $"move {from}-{to} must be legal");
                Assert.AreEqual(engine.Board.ComputeKeyFromScratch(), engine.Board.Key,
                    $"key mismatch after {from}-{to}");
            }
        }

        [TestMethod]
        public void UnmakeRestoresPositionAndKey()
        {
            BitboardChessEngine engine = new();
            engine.Board.SetFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -");

            ulong initialKey = engine.Board.Key;
            int initialMaterial = engine.Board.Material;

            // run a perft: thousands of make/unmake cycles over castles,
            // en passant and captures must restore the position exactly
            engine.Board.Perft(3);

            Assert.AreEqual(initialKey, engine.Board.Key);
            Assert.AreEqual(initialKey, engine.Board.ComputeKeyFromScratch());
            Assert.AreEqual(initialMaterial, engine.Board.Material);
        }

        [TestMethod]
        public void KeyIsPositionNotPathDependent()
        {
            BitboardChessEngine engine = new();
            engine.New();

            // 1. Nf3 Nf6 2. Ng1 Ng8 returns to the start position
            engine.ExecuteMove("G1", "F3");
            engine.ExecuteMove("G8", "F6");
            ulong afterKnights = engine.Board.Key;

            engine.ExecuteMove("F3", "G1");
            engine.ExecuteMove("F6", "G8");

            BitboardChessEngine fresh = new();
            fresh.New();

            Assert.AreEqual(fresh.Board.Key, engine.Board.Key);
            Assert.AreNotEqual(afterKnights, engine.Board.Key);
        }

        [TestMethod]
        public void RepetitionIsDetected()
        {
            BitboardChessEngine engine = new();
            engine.New();

            engine.ExecuteMove("G1", "F3");
            engine.ExecuteMove("G8", "F6");
            Assert.IsFalse(engine.Board.IsRepetition());

            engine.ExecuteMove("F3", "G1");
            engine.ExecuteMove("F6", "G8"); // start position for the second time

            Assert.IsTrue(engine.Board.IsRepetition());
        }

        /// En passant only makes a key difference when the capture is really
        /// possible; the same piece placement without a capturing pawn hashes
        /// identically whether or not a double step just happened.
        [TestMethod]
        public void EnPassantOnlyHashedWhenCapturePossible()
        {
            // with black pawn on D4: after E2-E4 the capture D4xE3 exists
            BitboardChessEngine withCapture = new();
            withCapture.Clear();
            withCapture.SetPiece(Constants.WhiteKing, "E1");
            withCapture.SetPiece(Constants.BlackKing, "E8");
            withCapture.SetPiece(Constants.WhitePawn, "E2");
            withCapture.SetPiece(Constants.BlackPawn, "D4");
            withCapture.ColorToMove = Constants.White;
            withCapture.ExecuteMove("E2", "E4");

            BitboardChessEngine samePlacement = new();
            samePlacement.Clear();
            samePlacement.SetPiece(Constants.WhiteKing, "E1");
            samePlacement.SetPiece(Constants.BlackKing, "E8");
            samePlacement.SetPiece(Constants.WhitePawn, "E4");
            samePlacement.SetPiece(Constants.BlackPawn, "D4");
            samePlacement.ColorToMove = Constants.Black;

            Assert.AreNotEqual(samePlacement.Board.Key, withCapture.Board.Key,
                "a real en passant right must change the key");

            // without a black pawn beside E4 the double step leaves no trace
            BitboardChessEngine noCapturer = new();
            noCapturer.Clear();
            noCapturer.SetPiece(Constants.WhiteKing, "E1");
            noCapturer.SetPiece(Constants.BlackKing, "E8");
            noCapturer.SetPiece(Constants.WhitePawn, "E2");
            noCapturer.SetPiece(Constants.BlackPawn, "A7");
            noCapturer.ColorToMove = Constants.White;
            noCapturer.ExecuteMove("E2", "E4");

            BitboardChessEngine noCapturerDirect = new();
            noCapturerDirect.Clear();
            noCapturerDirect.SetPiece(Constants.WhiteKing, "E1");
            noCapturerDirect.SetPiece(Constants.BlackKing, "E8");
            noCapturerDirect.SetPiece(Constants.WhitePawn, "E4");
            noCapturerDirect.SetPiece(Constants.BlackPawn, "A7");
            noCapturerDirect.ColorToMove = Constants.Black;

            Assert.AreEqual(noCapturerDirect.Board.Key, noCapturer.Board.Key,
                "an unusable en passant marker must not change the key");
        }

        #endregion

        #region Search

        [TestMethod]
        public void CalculateMoveTakesHangingPawnAtDepthOne()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "E1");
            engine.SetPiece(Constants.WhiteQueen, "C1");
            engine.SetPiece(Constants.BlackKing, "E8");
            engine.SetPiece(Constants.BlackPawn, "C6");
            engine.SetPiece(Constants.BlackPawn, "D7");
            engine.ColorToMove = Constants.White;

            EngineMove move = engine.CalculateMove(1);

            // depth 1 does not see the recapture by the pawn on D7
            Assert.AreEqual("C1", move.Start);
            Assert.AreEqual("C6", move.End);
        }

        [TestMethod]
        public void CalculateMoveAvoidsDefendedPawnAtDepthTwo()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "E1");
            engine.SetPiece(Constants.WhiteQueen, "C1");
            engine.SetPiece(Constants.BlackKing, "E8");
            engine.SetPiece(Constants.BlackPawn, "C6");
            engine.SetPiece(Constants.BlackPawn, "D7");
            engine.ColorToMove = Constants.White;

            EngineMove move = engine.CalculateMove(2);

            // depth 2 sees D7xC6 and keeps the queen
            Assert.AreNotEqual("C6", move.End);
        }

        [TestMethod]
        public void EnPassantCaptureWinsMaterial()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "E1");
            engine.SetPiece(Constants.BlackKing, "E8");
            engine.SetPiece(Constants.WhitePawn, "B2");
            engine.SetPiece(Constants.BlackPawn, "C4");
            engine.ColorToMove = Constants.White;

            engine.ExecuteMove("B2", "B4"); // double step past the black pawn

            EngineMove move = engine.CalculateMove(2);

            // C4xB3 en passant is black's only move that wins material
            Assert.AreEqual("C4", move.Start);
            Assert.AreEqual("B3", move.End);

            // a pawn up for black, plus a sub-pawn mobility term
            Assert.IsLessThan(Constants.PieceValues[Constants.Pawn],
                Math.Abs(move.Rating.Value - (-Constants.PieceValues[Constants.Pawn])));
        }

        [TestMethod]
        public void CalculateMovePrefersKingCaptureOverQueen()
        {
            // Illegal position (black king en prise), set up on purpose:
            // taking the king must outrank taking the queen.
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "H2");
            engine.SetPiece(Constants.WhiteRook, "B1");
            engine.SetPiece(Constants.BlackKing, "B8");
            engine.SetPiece(Constants.BlackQueen, "C1");
            engine.ColorToMove = Constants.White;

            EngineMove move = engine.CalculateMove(2);

            Assert.AreEqual("B1", move.Start);
            Assert.AreEqual("B8", move.End);
            Assert.AreEqual(GameState.BlackLoss, move.Rating.State);
        }

        /// Unlike the pseudo-legal reference engines, the legal move
        /// generation never lets a check stand: with the queen on B1 checking
        /// along the first rank, hunting the black king with RxA8 is illegal -
        /// capturing the checking queen is the only good move.
        [TestMethod]
        public void CheckMustBeResolvedBeforeAnyKingHunt()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "H1");
            engine.SetPiece(Constants.WhiteRook, "A1");
            engine.SetPiece(Constants.BlackKing, "A8");
            engine.SetPiece(Constants.BlackQueen, "B1");
            engine.ColorToMove = Constants.White;

            Assert.IsTrue(engine.Board.InCheck(Constants.White));

            EngineMove move = engine.CalculateMove(2);

            Assert.AreEqual("A1", move.Start);
            Assert.AreEqual("B1", move.End); // capture the checker, win the queen
        }

        [TestMethod]
        public void CalculateTwoMoveMate()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "G6");
            engine.SetPiece(Constants.WhiteRook, "G5");
            engine.SetPiece(Constants.BlackKing, "H8");
            engine.ColorToMove = Constants.White;

            // Any safe rook lift on the 5th rank mates in 2 (1. Re5 Kg8
            // 2. Re8# and its siblings) - the engine must find one of them:
            // a rook move announcing mate on the 3rd ply.
            EngineMove move = engine.CalculateMove(5);

            Assert.AreEqual("G5", move.Start);
            Assert.AreEqual(GameState.BlackLoss, move.Rating.State);
            Assert.AreEqual(Constants.Mate - 3, move.Rating.Value); // mate on the 3rd ply

            // play the announced line out: after white's 2nd move black is mated
            foreach (string pvMove in move.Rating.MoveList.Split(';'))
            {
                string[] squares = pvMove.Split('-');
                Assert.IsTrue(engine.ExecuteMove(squares[0], squares[1]), $"PV move {pvMove} must be legal");
            }

            Assert.HasCount(0, engine.GetLegalMoves());
            Assert.IsTrue(engine.Board.InCheck(Constants.Black));
        }

        [TestMethod]
        public void CalculateTwoMoveMatePart2()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "G6");
            engine.SetPiece(Constants.WhiteRook, "D5");
            engine.SetPiece(Constants.WhiteKnight, "E2");
            engine.SetPiece(Constants.BlackKing, "H8");
            engine.SetPiece(Constants.BlackRook, "E4");
            engine.ColorToMove = Constants.White;

            // 1. Rd8+ Re8 (forced) 2. Rxe8#
            EngineMove move = engine.CalculateMove(6);

            Assert.AreEqual("D5", move.Start);
            Assert.AreEqual("D8", move.End);
            Assert.AreEqual(GameState.BlackLoss, move.Rating.State);

            engine.ExecuteMove(move.Start, move.End);

            // black must block the check with the rook; taking the knight
            // instead would run into the mate
            EngineMove moveBlack = engine.CalculateMove(6);

            Assert.AreEqual("E4", moveBlack.Start);
            Assert.AreEqual("E8", moveBlack.End);
            Assert.AreEqual(GameState.BlackLoss, moveBlack.Rating.State);
        }

        [TestMethod]
        public void PromotionOnB8DeliversCheckmate()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "G6");
            engine.SetPiece(Constants.BlackKing, "G8");
            engine.SetPiece(Constants.WhitePawn, "B7");
            engine.ColorToMove = Constants.White;

            EngineMove move = engine.CalculateMove(3);

            Assert.AreEqual("B7", move.Start);
            Assert.AreEqual("B8", move.End);
            Assert.AreEqual(GameState.BlackLoss, move.Rating.State);

            engine.ExecuteMove("B7", "B8");

            Assert.AreEqual(Constants.WhiteQueen, engine.Board.PieceAt(Constants.SquareOf("B8")));
            Assert.HasCount(0, engine.GetLegalMoves());
            Assert.IsTrue(engine.Board.InCheck(Constants.Black));
        }

        [TestMethod]
        public void StalemateHasNoMoveAndNoCheck()
        {
            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "E1");
            engine.SetPiece(Constants.WhiteQueen, "C7");
            engine.SetPiece(Constants.BlackKing, "A8");
            engine.ColorToMove = Constants.Black;

            Assert.HasCount(0, engine.GetLegalMoves());
            Assert.IsFalse(engine.Board.InCheck(Constants.Black));
            Assert.IsNull(engine.CalculateMove(4));
        }

        [TestMethod]
        public void SearchLeavesBoardUnchanged()
        {
            BitboardChessEngine engine = new();
            engine.New();

            ulong key = engine.Board.Key;
            int material = engine.Board.Material;
            int[] mailbox = (int[])engine.Board.Mailbox.Clone();

            engine.CalculateMove(5);

            Assert.AreEqual(key, engine.Board.Key);
            Assert.AreEqual(material, engine.Board.Material);
            CollectionAssert.AreEqual(mailbox, engine.Board.Mailbox);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(10)]
        public void CalculateOpeningMove(int depth)
        {
            BitboardChessEngine engine = new();
            engine.New();

            EngineMove move = engine.CalculateMove(depth);

            Assert.IsNotNull(move);
            Console.WriteLine($"Depth {depth}: {move} Nodes:{engine.Nodes} "
                              + $"TT: {engine.Table.Hits}/{engine.Table.Probes} hits");
        }

        /// Position 2 – Central tension: d4/e5 pawns face each other,
        /// both sides fully developed with equal material - the same
        /// middlegame position the other engines are tested on at depth 8.
        [TestMethod]
        [DataRow(0_08L)]
        [DataRow(0_09L)]
        [DataRow(0_10L)]
        public void CentralTensionCalculateMoveForWhiteDepth(long depth)
        {
            BitboardChessEngine engine = new();
            SetUpCentralTension(engine);

            Assert.AreEqual(0, engine.Board.Material); // equal material

            Stopwatch watch = Stopwatch.StartNew();
            EngineMove move = engine.CalculateMove((int)depth);
            watch.Stop();

            Assert.IsNotNull(move);
            Assert.AreEqual(Constants.White, Constants.ColorOf(move.Move.Piece));
            Assert.AreEqual(GameState.Normal, move.Rating.State);

            Console.WriteLine($"Best move: {move} in {watch.Elapsed.TotalMilliseconds:F0} ms");
            Console.WriteLine($"Nodes: {engine.Nodes} TT: {engine.Table.Hits}/{engine.Table.Probes} hits");
            Console.WriteLine($"Line: {move.Rating.MoveList}");
        }

        private static void SetUpCentralTension(BitboardChessEngine engine)
        {
            engine.Clear();

            // White
            engine.SetPiece(Constants.WhiteKing, "G1");
            engine.SetPiece(Constants.WhiteQueen, "E2");
            engine.SetPiece(Constants.WhiteRook, "A1");
            engine.SetPiece(Constants.WhiteRook, "D1");
            engine.SetPiece(Constants.WhiteBishop, "C4");
            engine.SetPiece(Constants.WhiteBishop, "G2");
            engine.SetPiece(Constants.WhiteKnight, "F3");
            engine.SetPiece(Constants.WhiteKnight, "C3");
            foreach (string position in new[] { "A2", "B2", "C2", "D4", "E4", "F2", "G3", "H2" })
                engine.SetPiece(Constants.WhitePawn, position);

            // Black
            engine.SetPiece(Constants.BlackKing, "G8");
            engine.SetPiece(Constants.BlackQueen, "E7");
            engine.SetPiece(Constants.BlackRook, "A8");
            engine.SetPiece(Constants.BlackRook, "D8");
            engine.SetPiece(Constants.BlackBishop, "C8");
            engine.SetPiece(Constants.BlackBishop, "G7");
            engine.SetPiece(Constants.BlackKnight, "C6");
            engine.SetPiece(Constants.BlackKnight, "F6");
            foreach (string position in new[] { "A7", "B7", "C7", "D6", "E5", "F7", "G6", "H7" })
                engine.SetPiece(Constants.BlackPawn, position);

            engine.ColorToMove = Constants.White;
        }

        #endregion

        #region Cross-checks against the IntegerChessEngine

        /// Both engines must find a mate in two here. The exact rook target
        /// may differ (several rook lifts on the 5th rank mate equally fast,
        /// the engines break the tie by their own move order), so the check
        /// is: same moving piece, and both announce the win.
        [TestMethod]
        public void TwoMoveMateMatchesIntegerChessEngine()
        {
            var integerEngine = new MyIntegerChessEngine.IntegerChessEngine();
            integerEngine.Clear();
            integerEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteKing(MyIntegerChessEngine.CastleType.None), "G6");
            integerEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteRook(), "G5");
            integerEngine.SetPiece(MyIntegerChessEngine.PieceFactory.BlackKing(MyIntegerChessEngine.CastleType.None), "H8");
            integerEngine.ColorToMove = MyIntegerChessEngine.Constants.White;
            MyIntegerChessEngine.Move expected = integerEngine.CalculateMove(5);

            BitboardChessEngine engine = new();
            engine.Clear();
            engine.SetPiece(Constants.WhiteKing, "G6");
            engine.SetPiece(Constants.WhiteRook, "G5");
            engine.SetPiece(Constants.BlackKing, "H8");
            engine.ColorToMove = Constants.White;
            EngineMove actual = engine.CalculateMove(5);

            Assert.AreEqual(MyIntegerChessEngine.GameState.BlackLoss, expected.Rating.State);
            Assert.AreEqual(GameState.BlackLoss, actual.Rating.State);
            Assert.AreEqual(Constants.Mate - 3, actual.Rating.Value); // mate in 2 moves
            Assert.AreEqual(expected.Start.ToString(), actual.Start); // both move the rook
        }

        /// Node-count and time comparison on the opening position - no hard
        /// assertion on the ratio, but the numbers land in the test output.
        [TestMethod]
        public void OpeningDepthSixComparisonWithIntegerEngine()
        {
            var integerEngine = new MyIntegerChessEngine.IntegerChessEngine();
            integerEngine.New();
            Stopwatch integerWatch = Stopwatch.StartNew();
            MyIntegerChessEngine.Move integerMove = integerEngine.CalculateMove(6);
            integerWatch.Stop();

            BitboardChessEngine engine = new();
            engine.New();
            Stopwatch bitboardWatch = Stopwatch.StartNew();
            EngineMove bitboardMove = engine.CalculateMove(6);
            bitboardWatch.Stop();

            Console.WriteLine($"Integer:  {integerWatch.Elapsed.TotalMilliseconds,8:F0} ms -> {integerMove.Start}-{integerMove.End}");
            Console.WriteLine($"Bitboard: {bitboardWatch.Elapsed.TotalMilliseconds,8:F0} ms -> {bitboardMove.Start}-{bitboardMove.End} "
                              + $"Nodes:{engine.Nodes} TT: {engine.Table.Hits}/{engine.Table.Probes} hits");

            Assert.IsNotNull(bitboardMove);
        }

        #endregion
    }
}
