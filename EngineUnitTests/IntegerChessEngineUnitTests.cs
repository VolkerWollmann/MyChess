using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngineBase;
using MyIntegerChessEngine;
using Move = MyIntegerChessEngine.Move;
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
    }
}
