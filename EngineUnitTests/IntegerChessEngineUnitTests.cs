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
        public void GetRatingStartPositionIsBalanced()
        {
            IntegerChessEngine chessEngine = new IntegerChessEngine();
            chessEngine.New();

            Assert.AreEqual(0, chessEngine.GetRating());
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

            Assert.AreEqual(300, chessEngine.GetRating()); // 900 - 500 - 100
        }
    }
}
