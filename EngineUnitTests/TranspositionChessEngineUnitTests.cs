using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyTranspositionChessEngine;
using MyTranspositionChessEngine.Pieces;
using Move = MyTranspositionChessEngine.Move;
using MoveList = MyTranspositionChessEngine.MoveList;
using Position = MyTranspositionChessEngine.Position;

namespace EngineUnitTests
{
    /// The TranspositionChessEngine is a copy of the IntegerChessEngine plus a
    /// transposition table. The first part mirrors the IntegerChessEngine tests
    /// (the copied machinery must behave identically), the second part tests
    /// the transposition table itself.
    [TestClass]
    public class TranspositionChessEngineUnitTests
    {

        [TestMethod]
        public void CreateEngine()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            Assert.IsNotNull(chessEngine);
        }

        [TestMethod]
        public void CopyBoard()
        {
            Board board = new MyTranspositionChessEngine.Board();
            var copy = board.Copy();
        }

        [TestMethod]
        public void CompareBoard()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.New();
            Board board = chessEngine.Board;
            Board board2 = board.Copy();

            Assert.IsTrue(board.CompareBoard(board2));
        }

        [TestMethod]
        public void WhiteKnightInCornerStaysOnBoard()
        {
            Board board = new Board();
            Position a1 = new Position("A1");
            board.SetPiece(PieceFactory.WhiteKnight(), a1);

            MoveList moves = Knight.GetMoveList(board, a1);

            Assert.HasCount(2, moves); // only B3 and C2
        }

        [TestMethod]
        public void BlackKnightInCornerStaysOnBoard()
        {
            Board board = new Board();
            Position h8 = new Position("H8");
            board.SetPiece(PieceFactory.BlackKnight(), h8);

            MoveList moves = Knight.GetMoveList(board, h8);

            Assert.HasCount(2, moves); // only F7 and G6
        }

        [TestMethod]
        public void WhiteRookInCornerStaysOnBoard()
        {
            Board board = new Board();
            Position a1 = new Position("A1");
            board.SetPiece(PieceFactory.WhiteRook(), a1);

            MoveList moves = Rook.GetMoveList(board, a1);

            Assert.HasCount(14, moves); // A2-A8 and B1-H1
        }

        [TestMethod]
        public void RookStopsOnCapture()
        {
            Board board = new Board();
            Position a1 = new Position("A1");
            board.SetPiece(PieceFactory.BlackRook(), a1);
            board.SetPiece(PieceFactory.WhitePawn(), new Position("A5"));

            MoveList moves = Rook.GetMoveList(board, a1);

            Assert.HasCount(11, moves); // A2-A4, capture A5, B1-H1
        }

        [TestMethod]
        public void KingInCornerStaysOnBoard()
        {
            Board board = new Board();
            Position a1 = new Position("A1");
            board.SetPiece(PieceFactory.WhiteKing(), a1);

            MoveList moves = King.GetThreatenMoveList(board, a1);

            Assert.HasCount(3, moves); // A2, B1, B2
        }

        [TestMethod]
        public void TwoKingsNoCastlingWhiteToMove()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");

            chessEngine.ColorToMove = Constants.White;

            MoveList moves = chessEngine.GetMoveList();

            Assert.HasCount(5, moves); // D1, D2, E2, F2, F1
            Assert.IsTrue(moves.TrueForAll(move => move.CastleType == CastleType.None));
        }

        [TestMethod]
        public void PawnThreatensDiagonalsOnly()
        {
            Board board = new Board();
            Position e4 = new Position("E4");
            board.SetPiece(PieceFactory.WhitePawn(), e4);

            MoveList moves = Pawn.GetThreatenMoveList(board, e4);

            Assert.HasCount(2, moves); // D5 and F5, not E5
            Assert.IsTrue(moves.TrueForAll(move => move.End.Row == 4));
        }

        [TestMethod]
        public void RookThreatContinuesBehindEnemyKing()
        {
            Board board = new Board();
            Position a1 = new Position("A1");
            board.SetPiece(PieceFactory.WhiteRook(), a1);
            board.SetPiece(PieceFactory.BlackKing(CastleType.None), new Position("A5"));

            MoveList moves = Rook.GetThreatenMoveList(board, a1);

            // A2-A4, king on A5, A6-A8 behind the king, B1-H1
            Assert.HasCount(14, moves);
        }

        [TestMethod]
        public void GetThreatenMoveListCollectsAllPiecesOfColor()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "A1");
            chessEngine.SetPiece(PieceFactory.WhitePawn(), "E4");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");

            MoveList moves = chessEngine.GetThreatenMoveList(Constants.White);

            Assert.HasCount(5, moves); // king: A2, B1, B2 - pawn: D5, F5
        }

        [TestMethod]
        public void KingAvoidsThreatenedFields()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");
            chessEngine.SetPiece(PieceFactory.BlackRook(), "D8");

            chessEngine.ColorToMove = Constants.White;

            MoveList moves = chessEngine.GetMoveList();

            // D1 and D2 are threatened by the rook on D8
            Assert.HasCount(3, moves); // E2, F1, F2
            Assert.IsTrue(moves.TrueForAll(move => move.End.Column != 3));
        }

        [TestMethod]
        public void CastleBlockedOnThreatenedField()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(), "E1");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "A1");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "H1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");
            chessEngine.SetPiece(PieceFactory.BlackRook(), "F8");

            chessEngine.ColorToMove = Constants.White;

            MoveList moves = chessEngine.GetMoveList();

            // F1 is threatened, the king may not pass it on the king side
            Assert.IsFalse(moves.Exists(move => move.CastleType == CastleType.WhiteKingSide));
            Assert.IsTrue(moves.Exists(move => move.CastleType == CastleType.WhiteQueenSide));
        }

        [TestMethod]
        public void CastleBlockedByPieceInBetween()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(), "E1");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "A1");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "H1");
            chessEngine.SetPiece(PieceFactory.WhiteBishop(), "F1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");

            chessEngine.ColorToMove = Constants.White;

            MoveList moves = chessEngine.GetMoveList();

            Assert.IsFalse(moves.Exists(move => move.CastleType == CastleType.WhiteKingSide));
            Assert.IsTrue(moves.Exists(move => move.CastleType == CastleType.WhiteQueenSide));
        }

        [TestMethod]
        public void CastleBlockedWhenKingIsChecked()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(), "E1");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "A1");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "H1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "A8");
            chessEngine.SetPiece(PieceFactory.BlackRook(), "E8");

            chessEngine.ColorToMove = Constants.White;

            MoveList moves = chessEngine.GetMoveList();

            Assert.IsFalse(moves.Exists(move => move.CastleType != CastleType.None));
        }

        [TestMethod]
        public void CalculateMoveTakesHangingPawnAtDepthOne()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.WhiteQueen(), "C1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");
            chessEngine.SetPiece(PieceFactory.BlackPawn(), "C6");
            chessEngine.SetPiece(PieceFactory.BlackPawn(), "D7");

            chessEngine.ColorToMove = Constants.White;

            Move move = chessEngine.CalculateMove(1);

            // Depth 1 does not see the recapture by the pawn on D7
            Assert.AreEqual("C1", move.Start.ToString());
            Assert.AreEqual("C6", move.End.ToString());
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
        public void CalculateOpeningMove(int depth)
        {
            var chessEngine = new TranspositionChessEngine();
            chessEngine.New(); // Initialize the chess engine with the starting position.

            Move move = chessEngine.CalculateMove(depth);
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
        public void CalculateOpeningMoveParallel(int depth)
        {
            var chessEngine = new TranspositionChessEngine();
            chessEngine.New(); // Initialize the chess engine with the starting position.

            Move move = chessEngine.CalculateMoveParallel(depth);
        }

        [TestMethod]
        public void CalculateMoveAvoidsDefendedPawnAtDepthTwo()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.WhiteQueen(), "C1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");
            chessEngine.SetPiece(PieceFactory.BlackPawn(), "C6");
            chessEngine.SetPiece(PieceFactory.BlackPawn(), "D7");

            chessEngine.ColorToMove = Constants.White;

            Move move = chessEngine.CalculateMove(2);

            // Depth 2 sees D7xC6 and keeps the queen
            Assert.AreNotEqual("C6", move.End.ToString());
        }

        [TestMethod]
        public void EnPassantCaptureWinsMaterial()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");
            chessEngine.SetPiece(PieceFactory.WhitePawn(), "B2");
            chessEngine.SetPiece(PieceFactory.BlackPawn(), "C4");

            chessEngine.ColorToMove = Constants.White;

            // White double step B2-B4 passes the black pawn on C4
            Position b2 = new Position("B2");
            chessEngine.ExecuteMove(new Move(b2, new Position("B4"), chessEngine.Board.GetPiece(b2)));

            // The black pawn is marked for en passant on the immediate reply
            var blackPawn = chessEngine.Board.GetPiece(new Position("C4"));
            Assert.AreEqual(chessEngine.Board.CurrentPly, blackPawn.LastEnPassantPlyMarking + 1);

            // C4xB3 en passant is black's only move that wins material
            Move move = chessEngine.CalculateMove(2);

            Assert.AreEqual("C4", move.Start.ToString());
            Assert.AreEqual("B3", move.End.ToString());

            // The won pawn plus the threat-field difference, which stays below a pawn
            Assert.IsLessThan(Constants.PawnValue, Math.Abs(move.Rating.Value - (-Constants.PawnValue)),
                $"Rating {move.Rating.Value} must be -PawnValue plus a sub-pawn threat-field bonus");
        }

        [TestMethod]
        public void CalculateMovePrefersMoreThreatenedFieldsOnEqualMaterial()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.WhiteKnight(), "B1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");

            chessEngine.ColorToMove = Constants.White;

            // All moves keep the material at +KnightValue; the knight jump to the
            // central field C3 threatens the most fields and must win the
            // equal-material tie.
            Move move = chessEngine.CalculateMove(2);

            Assert.AreEqual("B1", move.Start.ToString());
            Assert.AreEqual("C3", move.End.ToString());

            // Material plus a positive sub-pawn threat-field bonus
            Assert.IsGreaterThan(Constants.KnightValue, move.Rating.Value);
            Assert.IsLessThan(Constants.KnightValue + Constants.PawnValue, move.Rating.Value);
        }

        [TestMethod]
        public void CalculateMovePrefersWinOverMaterial()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "H1");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "A1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "A8");
            chessEngine.SetPiece(PieceFactory.BlackQueen(), "B1");

            chessEngine.ColorToMove = Constants.White;

            Move move = chessEngine.CalculateMove(2);

            // Beating the king (BlackLoss) outweighs beating the queen
            Assert.AreEqual("A8", move.End.ToString());
            Assert.AreEqual(GameState.BlackLoss, move.Rating.State);
        }

        [TestMethod]
        public void CalculateTwoMoveMate()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "G6");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "G5");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "H8");

            chessEngine.ColorToMove = Constants.White;

            // 1. Re5 Kg8 (forced) 2. Re8#
            Move move = chessEngine.CalculateMove(5);

            Assert.AreEqual("G5", move.Start.ToString());
            Assert.AreEqual("E5", move.End.ToString());
            Assert.AreEqual(GameState.BlackLoss, move.Rating.State);
        }

        [TestMethod]
        public void CalculateTwoMoveMateParallel()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "G6");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "G5");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "H8");

            chessEngine.ColorToMove = Constants.White;

            // 1. Re5 Kg8 (forced) 2. Re8#
            Move move = chessEngine.CalculateMoveParallel(5);

            Assert.AreEqual("G5", move.Start.ToString());
            Assert.AreEqual("E5", move.End.ToString());
            Assert.AreEqual(GameState.BlackLoss, move.Rating.State);
        }

        [TestMethod]
        public void CalculateTwoMoveMatePart2()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "G6");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "D5");
            chessEngine.SetPiece(PieceFactory.WhiteKnight(), "E2");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "H8");
            chessEngine.SetPiece(PieceFactory.BlackRook(), "E4");

            chessEngine.ColorToMove = Constants.White;

            // 1. Rd8+ Re8 (forced) 2. Rxe8#
            Move move = chessEngine.CalculateMove(8);

            Assert.AreEqual("D5", move.Start.ToString());
            Assert.AreEqual("D8", move.End.ToString());
            Assert.AreEqual(GameState.BlackLoss, move.Rating.State);

            chessEngine.ExecuteMove(move);

            Move moveBlack = chessEngine.CalculateMove(8);

            // Defend the king -> black must block the check with the rook,
            // capturing the knight (E4xE2) would leave the king to be killed
            Assert.AreEqual("E4", moveBlack.Start.ToString());
            Assert.AreEqual("E8", moveBlack.End.ToString());
            Assert.AreEqual(GameState.BlackLoss, moveBlack.Rating.State);
        }

        /// The search executes and undoes moves on the live board; afterwards every
        /// plane except the transient threat plane must be exactly as before.
        [TestMethod]
        public void CalculateMoveLeavesBoardUnchanged()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.New();
            chessEngine.ColorToMove = Constants.White;

            Board pristine = chessEngine.Board.Copy();

            chessEngine.CalculateMove(4);

            for (int plane = 0; plane < Constants.ThreatPlane; plane++)
            for (int column = 0; column < Constants.GridSize; column++)
            for (int row = 0; row < Constants.GridSize; row++)
            {
                Assert.AreEqual(pristine.Field[plane, column, row],
                    chessEngine.Board.Field[plane, column, row],
                    $"Plane {plane} differs at [{column},{row}]");
            }

            Assert.AreEqual(pristine.CurrentPly, chessEngine.Board.CurrentPly);
        }

        [TestMethod]
        public void WhitePawnPromotesToQueenOnLastRow()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");
            chessEngine.SetPiece(PieceFactory.WhitePawn(), "B7");

            chessEngine.ColorToMove = Constants.White;

            Position b7 = new Position("B7");
            chessEngine.ExecuteMove(new Move(b7, new Position("B8"), chessEngine.Board.GetPiece(b7)));

            var promoted = chessEngine.Board.GetPiece(new Position("B8"));
            Assert.AreEqual(Constants.Queen, promoted.PieceType);
            Assert.AreEqual(Constants.White, promoted.IntColor);
            Assert.IsTrue(chessEngine.Board.GetPiece(b7).IsEmpty);
        }

        [TestMethod]
        public void BlackPawnPromotesToQueenOnCapture()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");
            chessEngine.SetPiece(PieceFactory.BlackPawn(), "C2");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "B1");

            chessEngine.ColorToMove = Constants.Black;

            Position c2 = new Position("C2");
            chessEngine.ExecuteMove(new Move(c2, new Position("B1"), chessEngine.Board.GetPiece(c2)));

            var promoted = chessEngine.Board.GetPiece(new Position("B1"));
            Assert.AreEqual(Constants.Queen, promoted.PieceType);
            Assert.AreEqual(Constants.Black, promoted.IntColor);
            Assert.IsTrue(chessEngine.Board.GetPiece(c2).IsEmpty);
        }

        [TestMethod]
        public void UndoRestoresPawnAfterPromotion()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");
            chessEngine.SetPiece(PieceFactory.WhitePawn(), "B7");

            chessEngine.ColorToMove = Constants.White;
            Board board = chessEngine.Board;
            Board pristine = board.Copy();

            Position b7 = new Position("B7");
            var undo = board.ExecuteMoveWithUndo(new Move(b7, new Position("B8"), board.GetPiece(b7)));
            board.UndoMove(undo);

            Assert.IsTrue(board.CompareBoard(pristine));
            Assert.AreEqual(pristine.CurrentPly, board.CurrentPly);

            var pawn = board.GetPiece(b7);
            Assert.AreEqual(Constants.Pawn, pawn.PieceType);
            Assert.IsTrue(pawn.Compare(pristine.GetPiece(b7)));
        }

        [TestMethod]
        public void PromotionOnB8DeliversCheckmate()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "G6");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "G8");
            chessEngine.SetPiece(PieceFactory.WhitePawn(), "B7");

            chessEngine.ColorToMove = Constants.White;

            // B7-B8=Q mates: the queen covers the 8th rank, the king G7/H7/F7
            Move move = chessEngine.CalculateMove(2);

            Assert.AreEqual("B7", move.Start.ToString());
            Assert.AreEqual("B8", move.End.ToString());
            Assert.AreEqual(GameState.BlackLoss, move.Rating.State);

            // After the promotion black has no legal move and is checked
            chessEngine.ExecuteMove(move);

            Assert.AreEqual(Constants.Queen, chessEngine.Board.GetPiece(new Position("B8")).PieceType);
            Assert.HasCount(0, chessEngine.GetMoveList());
            Assert.IsTrue(chessEngine.Board.IsKingThreatened(Constants.Black));
        }

        /// Position 2 – Central tension: d4/e5 pawns face each other,
        /// both sides fully developed with equal material.
        [TestMethod]
        public void CentralTensionCalculateMoveForWhiteDepthEight()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            SetUpCentralTension(chessEngine);

            Assert.AreEqual(0, chessEngine.GetRating().Value); // equal material

            Move move = chessEngine.CalculateMove(8);

            Assert.IsNotNull(move);
            Assert.AreEqual(Constants.White, move.Piece.IntColor);
            Console.WriteLine($"Best move: {move.Start}-{move.End} {move.Rating}");
            Console.WriteLine(TableStatistics(chessEngine));
        }

        /// Position 2 – Central tension: d4/e5 pawns face each other,
        /// both sides fully developed with equal material.
        [TestMethod]
        public void CentralTensionCalculateMoveForWhiteDepthEightParallel()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            SetUpCentralTension(chessEngine);

            Assert.AreEqual(0, chessEngine.GetRating().Value); // equal material

            Move move = chessEngine.CalculateMoveParallel(8);

            Assert.IsNotNull(move);
            Assert.AreEqual(Constants.White, move.Piece.IntColor);
            Console.WriteLine($"Best move: {move.Start}-{move.End} {move.Rating}");
            Console.WriteLine(TableStatistics(chessEngine));
        }

        [TestMethod]
        public void GetRatingStartPositionIsBalanced()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.New();

            Assert.AreEqual(0, chessEngine.GetRating().Value);
        }

        [TestMethod]
        public void GetRatingCountsMaterial()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");
            chessEngine.SetPiece(PieceFactory.WhiteQueen(), "D1");
            chessEngine.SetPiece(PieceFactory.BlackRook(), "A8");
            chessEngine.SetPiece(PieceFactory.BlackPawn(), "A7");

            Assert.AreEqual(300, chessEngine.GetRating().Value); // 900 - 500 - 100
        }

        #region Transposition table

        /// The Zobrist key must be identical whenever the same position with the
        /// same rights recurs, and different when rights differ.
        [TestMethod]
        public void ZobristKeyIsPositionNotPathDependent()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.New();
            ulong initialKey = chessEngine.Board.ComputeZobristKey();

            // Knight out and back: same position, same key
            Board board = chessEngine.Board;
            var undo1 = board.ExecuteMoveWithUndo(new Move(new Position("G1"), new Position("F3"), board.GetPiece(new Position("G1"))));
            var undo2 = board.ExecuteMoveWithUndo(new Move(new Position("F3"), new Position("G1"), board.GetPiece(new Position("F3"))));

            // The pieces stand as before, only the ply advanced; the key ignores the ply
            Assert.AreEqual(initialKey, board.ComputeZobristKey());

            board.UndoMove(undo2);
            board.UndoMove(undo1);
            Assert.AreEqual(initialKey, board.ComputeZobristKey());

            // A different side to move must change the key
            chessEngine.ColorToMove = Constants.Black;
            Assert.AreNotEqual(initialKey, board.ComputeZobristKey());
            chessEngine.ColorToMove = Constants.White;

            // A removed castle right must change the key
            board.DisableWhiteCastleKingSidePossible();
            Assert.AreNotEqual(initialKey, board.ComputeZobristKey());
        }

        [TestMethod]
        public void ZobristKeyDistinguishesEnPassantRight()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");
            chessEngine.SetPiece(PieceFactory.WhitePawn(), "B2");
            chessEngine.SetPiece(PieceFactory.BlackPawn(), "C4");
            chessEngine.ColorToMove = Constants.White;

            // Double step: black pawn may capture en passant, only right now
            Position b2 = new Position("B2");
            chessEngine.ExecuteMove(new Move(b2, new Position("B4"), chessEngine.Board.GetPiece(b2)));
            ulong keyWithEnPassant = chessEngine.Board.ComputeZobristKey();

            // One ply later (black king shuffle) the right is gone - same piece
            // placement, different key
            Position e8 = new Position("E8");
            var undo = chessEngine.Board.ExecuteMoveWithUndo(new Move(e8, new Position("D8"), chessEngine.Board.GetPiece(e8)));
            chessEngine.Board.UndoMove(undo);

            chessEngine.Board.CurrentPly++; // simulate the aged marking
            ulong keyWithoutEnPassant = chessEngine.Board.ComputeZobristKey();
            chessEngine.Board.CurrentPly--;

            Assert.AreNotEqual(keyWithEnPassant, keyWithoutEnPassant);
        }

        /// From depth 4 the first transpositions exist (two independent white
        /// moves swapped); at depth 5 the table must produce real hits.
        [TestMethod]
        public void TranspositionTableGetsHitsAtDepthFive()
        {
            TranspositionChessEngine chessEngine = new TranspositionChessEngine();
            chessEngine.New();

            chessEngine.CalculateMove(5);

            TranspositionTable table = chessEngine.Board.Table;
            Assert.IsNotNull(table);
            Console.WriteLine($"TT: {table.Hits} hits / {table.Probes} probes, {table.Stores} stores");
            Assert.IsGreaterThan(0L, table.Hits);
            Assert.IsGreaterThan(0L, table.Stores);
        }

        /// The transposition table must not change the result: same position,
        /// same depth, same best move, same value and state as the
        /// IntegerChessEngine it was copied from.
        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        public void OpeningMoveMatchesIntegerChessEngine(int depth)
        {
            var integerEngine = new MyIntegerChessEngine.IntegerChessEngine();
            integerEngine.New();
            MyIntegerChessEngine.Move expected = integerEngine.CalculateMove(depth);

            var transpositionEngine = new TranspositionChessEngine();
            transpositionEngine.New();
            Move actual = transpositionEngine.CalculateMove(depth);

            Assert.AreEqual(expected.Start.ToString(), actual.Start.ToString());
            Assert.AreEqual(expected.End.ToString(), actual.End.ToString());
            Assert.AreEqual(expected.Rating.Value, actual.Rating.Value);
            Assert.AreEqual(expected.Rating.State.ToString(), actual.Rating.State.ToString());
        }

        /// Cross-check on a full middlegame position where transpositions are
        /// frequent: the stored results must reproduce the plain search exactly.
        [TestMethod]
        public void CentralTensionMatchesIntegerChessEngineDepthFive()
        {
            var integerEngine = new MyIntegerChessEngine.IntegerChessEngine();
            SetUpCentralTensionInteger(integerEngine);
            MyIntegerChessEngine.Move expected = integerEngine.CalculateMove(5);

            var transpositionEngine = new TranspositionChessEngine();
            SetUpCentralTension(transpositionEngine);
            Move actual = transpositionEngine.CalculateMove(5);

            Console.WriteLine($"Integer:       {expected.Start}-{expected.End} {expected.Rating}");
            Console.WriteLine($"Transposition: {actual.Start}-{actual.End} {actual.Rating}");
            Console.WriteLine(TableStatistics(transpositionEngine));

            Assert.AreEqual(expected.Start.ToString(), actual.Start.ToString());
            Assert.AreEqual(expected.End.ToString(), actual.End.ToString());
            Assert.AreEqual(expected.Rating.Value, actual.Rating.Value);
            Assert.AreEqual(expected.Rating.State.ToString(), actual.Rating.State.ToString());
        }

        /// Mate search cross-check: the depth-sensitive win ratings (mate speed)
        /// must survive the table because entries are only reused at the exact
        /// remaining depth they were stored with.
        [TestMethod]
        public void TwoMoveMateMatchesIntegerChessEngine()
        {
            var integerEngine = new MyIntegerChessEngine.IntegerChessEngine();
            integerEngine.Clear();
            integerEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteKing(MyIntegerChessEngine.CastleType.None), "G6");
            integerEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteRook(), "D5");
            integerEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteKnight(), "E2");
            integerEngine.SetPiece(MyIntegerChessEngine.PieceFactory.BlackKing(MyIntegerChessEngine.CastleType.None), "H8");
            integerEngine.SetPiece(MyIntegerChessEngine.PieceFactory.BlackRook(), "E4");
            integerEngine.ColorToMove = MyIntegerChessEngine.Constants.White;
            MyIntegerChessEngine.Move expected = integerEngine.CalculateMove(6);

            var transpositionEngine = new TranspositionChessEngine();
            transpositionEngine.Clear();
            transpositionEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "G6");
            transpositionEngine.SetPiece(PieceFactory.WhiteRook(), "D5");
            transpositionEngine.SetPiece(PieceFactory.WhiteKnight(), "E2");
            transpositionEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "H8");
            transpositionEngine.SetPiece(PieceFactory.BlackRook(), "E4");
            transpositionEngine.ColorToMove = Constants.White;
            Move actual = transpositionEngine.CalculateMove(6);

            Assert.AreEqual(expected.Start.ToString(), actual.Start.ToString());
            Assert.AreEqual(expected.End.ToString(), actual.End.ToString());
            Assert.AreEqual(expected.Rating.Value, actual.Rating.Value);
            Assert.AreEqual(expected.Rating.State.ToString(), actual.Rating.State.ToString());
        }

        /// Not a hard assertion on time (machines differ), but the table must
        /// actually pay off in nodes: the search with table stores/probes are
        /// logged and a healthy hit rate shows up on the console.
        [TestMethod]
        public void OpeningDepthFiveTimingComparison()
        {
            var integerEngine = new MyIntegerChessEngine.IntegerChessEngine();
            integerEngine.New();
            Stopwatch integerWatch = Stopwatch.StartNew();
            MyIntegerChessEngine.Move expected = integerEngine.CalculateMove(5);
            integerWatch.Stop();

            var transpositionEngine = new TranspositionChessEngine();
            transpositionEngine.New();
            Stopwatch transpositionWatch = Stopwatch.StartNew();
            Move actual = transpositionEngine.CalculateMove(5);
            transpositionWatch.Stop();

            Console.WriteLine($"Integer:       {integerWatch.Elapsed.TotalMilliseconds:F0} ms -> {expected.Start}-{expected.End}");
            Console.WriteLine($"Transposition: {transpositionWatch.Elapsed.TotalMilliseconds:F0} ms -> {actual.Start}-{actual.End}");
            Console.WriteLine(TableStatistics(transpositionEngine));

            Assert.AreEqual(expected.Start.ToString(), actual.Start.ToString());
            Assert.AreEqual(expected.End.ToString(), actual.End.ToString());
        }

        #endregion

        #region Helpers

        private static string TableStatistics(TranspositionChessEngine chessEngine)
        {
            TranspositionTable table = chessEngine.Board.Table;
            return table == null
                ? "TT: no table"
                : $"TT: {table.Hits} hits / {table.Probes} probes, {table.Stores} stores";
        }

        private static void SetUpCentralTension(TranspositionChessEngine chessEngine)
        {
            chessEngine.Clear();

            // White
            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "G1");
            chessEngine.SetPiece(PieceFactory.WhiteQueen(), "E2");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "A1");
            chessEngine.SetPiece(PieceFactory.WhiteRook(), "D1");
            chessEngine.SetPiece(PieceFactory.WhiteBishop(), "C4");
            chessEngine.SetPiece(PieceFactory.WhiteBishop(), "G2");
            chessEngine.SetPiece(PieceFactory.WhiteKnight(), "F3");
            chessEngine.SetPiece(PieceFactory.WhiteKnight(), "C3");
            foreach (string position in new[] { "A2", "B2", "C2", "D4", "E4", "F2", "G3", "H2" })
                chessEngine.SetPiece(PieceFactory.WhitePawn(), position);

            // Black
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "G8");
            chessEngine.SetPiece(PieceFactory.BlackQueen(), "E7");
            chessEngine.SetPiece(PieceFactory.BlackRook(), "A8");
            chessEngine.SetPiece(PieceFactory.BlackRook(), "D8");
            chessEngine.SetPiece(PieceFactory.BlackBishop(), "C8");
            chessEngine.SetPiece(PieceFactory.BlackBishop(), "G7");
            chessEngine.SetPiece(PieceFactory.BlackKnight(), "C6");
            chessEngine.SetPiece(PieceFactory.BlackKnight(), "F6");
            foreach (string position in new[] { "A7", "B7", "C7", "D6", "E5", "F7", "G6", "H7" })
                chessEngine.SetPiece(PieceFactory.BlackPawn(), position);

            chessEngine.ColorToMove = Constants.White;
        }

        private static void SetUpCentralTensionInteger(MyIntegerChessEngine.IntegerChessEngine chessEngine)
        {
            chessEngine.Clear();

            // White
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteKing(MyIntegerChessEngine.CastleType.None), "G1");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteQueen(), "E2");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteRook(), "A1");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteRook(), "D1");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteBishop(), "C4");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteBishop(), "G2");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteKnight(), "F3");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhiteKnight(), "C3");
            foreach (string position in new[] { "A2", "B2", "C2", "D4", "E4", "F2", "G3", "H2" })
                chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.WhitePawn(), position);

            // Black
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.BlackKing(MyIntegerChessEngine.CastleType.None), "G8");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.BlackQueen(), "E7");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.BlackRook(), "A8");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.BlackRook(), "D8");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.BlackBishop(), "C8");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.BlackBishop(), "G7");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.BlackKnight(), "C6");
            chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.BlackKnight(), "F6");
            foreach (string position in new[] { "A7", "B7", "C7", "D6", "E5", "F7", "G6", "H7" })
                chessEngine.SetPiece(MyIntegerChessEngine.PieceFactory.BlackPawn(), position);

            chessEngine.ColorToMove = MyIntegerChessEngine.Constants.White;
        }

        #endregion
    }
}
