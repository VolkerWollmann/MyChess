using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyIntegerChessEngine;


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

            Assert.IsTrue(board.Compare(board2));

        }
    }
}
