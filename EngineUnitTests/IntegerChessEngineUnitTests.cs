using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngineBase;
using MyIntegerChessEngine;
using MyIntegerChessEngine.Pieces;
using Move = MyIntegerChessEngine.Move;
using MoveList = MyIntegerChessEngine.MoveList;
using Position = MyIntegerChessEngine.Position;


namespace EngineUnitTests
{
    [TestClass]
    public class IntegerChessEngineUnitTests
    {

        [TestMethod]
        public void CreateEngine()
        {
            IntegerChessEngine chessEngine = new IntegerChessEngine();
            Assert.IsNotNull(chessEngine);
        }

        [TestMethod]
        public void CopyBoard()
        {
            Board board = new MyIntegerChessEngine.Board();
            var copy = board.Copy();
        }

        [TestMethod]
        public void CompareBoard()
        {
            IntegerChessEngine chessEngine = new IntegerChessEngine();
            chessEngine.New();
            Board board = chessEngine.Board;
            Board board2 = board.Copy();

            Assert.IsTrue(board.CompareBoard(board2));

        }

        [TestMethod]
        public void TestToString()
        {
            Move move = new Move(new Position("A1"), new Position("A2"), PieceFactory.WhitePawn() );
            string result = move.ToString();
        }

        [TestMethod]
        public void WhiteKnightInCornerStaysOnBoard()
        {
            Board board = new Board();
            Position a1 = new Position("A1");
            board.SetPiece(PieceFactory.WhiteKnight(), a1);

            MoveList moves = new Knight().GetMoveList(board, a1);

            Assert.AreEqual(2, moves.Count); // only B3 and C2
        }

        [TestMethod]
        public void BlackKnightInCornerStaysOnBoard()
        {
            Board board = new Board();
            Position h8 = new Position("H8");
            board.SetPiece(PieceFactory.BlackKnight(), h8);

            MoveList moves = new Knight().GetMoveList(board, h8);

            Assert.AreEqual(2, moves.Count); // only F7 and G6
        }

        [TestMethod]
        public void WhiteRookInCornerStaysOnBoard()
        {
            Board board = new Board();
            Position a1 = new Position("A1");
            board.SetPiece(PieceFactory.WhiteRook(), a1);

            MoveList moves = new Rook().GetMoveList(board, a1);

            Assert.AreEqual(14, moves.Count); // A2-A8 and B1-H1
        }

        [TestMethod]
        public void RookStopsOnCapture()
        {
            Board board = new Board();
            Position a1 = new Position("A1");
            board.SetPiece(PieceFactory.BlackRook(), a1);
            board.SetPiece(PieceFactory.WhitePawn(), new Position("A5"));

            MoveList moves = new Rook().GetMoveList(board, a1);

            Assert.AreEqual(11, moves.Count); // A2-A4, capture A5, B1-H1
        }

        [TestMethod]
        public void KingInCornerStaysOnBoard()
        {
            Board board = new Board();
            Position a1 = new Position("A1");
            board.SetPiece(PieceFactory.WhiteKing(), a1);

            MoveList moves = new King().GetThreatenMoveList(board, a1);

            Assert.AreEqual(3, moves.Count); // A2, B1, B2
        }

        [TestMethod]
        public void TwoKingsNoCastlingWhiteToMove()
        {
            IntegerChessEngine chessEngine = new IntegerChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");

            chessEngine.ColorToMove = Constants.White;

            MoveList moves = chessEngine.GetMoveList();

            foreach (Move move in moves)
            {
                System.Console.WriteLine($"{move.Start} - {move.End}");
            }

            Assert.AreEqual(5, moves.Count); // D1, D2, E2, F2, F1
            Assert.IsTrue(moves.TrueForAll(move => move.CastleType == CastleType.None));
        }

        [TestMethod]
        public void PawnThreatensDiagonalsOnly()
        {
            Board board = new Board();
            Position e4 = new Position("E4");
            board.SetPiece(PieceFactory.WhitePawn(), e4);

            MoveList moves = new Pawn().GetThreatenMoveList(board, e4);

            Assert.AreEqual(2, moves.Count); // D5 and F5, not E5
            Assert.IsTrue(moves.TrueForAll(move => move.End.Row == 4));
        }

        [TestMethod]
        public void RookThreatContinuesBehindEnemyKing()
        {
            Board board = new Board();
            Position a1 = new Position("A1");
            board.SetPiece(PieceFactory.WhiteRook(), a1);
            board.SetPiece(PieceFactory.BlackKing(CastleType.None), new Position("A5"));

            MoveList moves = new Rook().GetThreatenMoveList(board, a1);

            // A2-A4, king on A5, A6-A8 behind the king, B1-H1
            Assert.AreEqual(14, moves.Count);
        }

        [TestMethod]
        public void GetThreatenMoveListCollectsAllPiecesOfColor()
        {
            IntegerChessEngine chessEngine = new IntegerChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "A1");
            chessEngine.SetPiece(PieceFactory.WhitePawn(), "E4");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");

            MoveList moves = chessEngine.GetThreatenMoveList(Constants.White);

            Assert.AreEqual(5, moves.Count); // king: A2, B1, B2 - pawn: D5, F5
        }

        [TestMethod]
        public void KingAvoidsThreatenedFields()
        {
            IntegerChessEngine chessEngine = new IntegerChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");
            chessEngine.SetPiece(PieceFactory.BlackRook(), "D8");

            chessEngine.ColorToMove = Constants.White;

            MoveList moves = chessEngine.GetMoveList();

            // D1 and D2 are threatened by the rook on D8
            Assert.AreEqual(3, moves.Count); // E2, F1, F2
            Assert.IsTrue(moves.TrueForAll(move => move.End.Column != 3));
        }

        [TestMethod]
        public void CastleBlockedOnThreatenedField()
        {
            IntegerChessEngine chessEngine = new IntegerChessEngine();
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
            IntegerChessEngine chessEngine = new IntegerChessEngine();
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
            IntegerChessEngine chessEngine = new IntegerChessEngine();
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
            IntegerChessEngine chessEngine = new IntegerChessEngine();
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
            var chessEngine = new IntegerChessEngine();
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
            var chessEngine = new IntegerChessEngine();
            chessEngine.New(); // Initialize the chess engine with the starting position.

            Move move = chessEngine.CalculateMoveParallel(depth);
        }

        [TestMethod]
        public void CalculateMoveAvoidsDefendedPawnAtDepthTwo()
        {
            IntegerChessEngine chessEngine = new IntegerChessEngine();
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
        public void CalculateMovePrefersWinOverMaterial()
        {
            IntegerChessEngine chessEngine = new IntegerChessEngine();
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
            IntegerChessEngine chessEngine = new IntegerChessEngine();
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
            IntegerChessEngine chessEngine = new IntegerChessEngine();
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
        public void GetRatingStartPositionIsBalanced()
        {
            IntegerChessEngine chessEngine = new IntegerChessEngine();
            chessEngine.New();

            Assert.AreEqual(0, chessEngine.GetRating().Value);
        }

        [TestMethod]
        public void GetRatingCountsMaterial()
        {
            IntegerChessEngine chessEngine = new IntegerChessEngine();
            chessEngine.Clear();

            chessEngine.SetPiece(PieceFactory.WhiteKing(CastleType.None), "E1");
            chessEngine.SetPiece(PieceFactory.BlackKing(CastleType.None), "E8");
            chessEngine.SetPiece(PieceFactory.WhiteQueen(), "D1");
            chessEngine.SetPiece(PieceFactory.BlackRook(), "A8");
            chessEngine.SetPiece(PieceFactory.BlackPawn(), "A7");

            Assert.AreEqual(300, chessEngine.GetRating().Value); // 900 - 500 - 100
        }
    }
}
