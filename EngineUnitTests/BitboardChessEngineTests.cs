using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine.Bitboard;
using MyChessEngineBase;

namespace EngineUnitTests
{
    [TestClass]
    public class BitboardChessEngineTests
    {
        [TestMethod]
        public void New_Position_HasStartingPieces()
        {
            var engine = new BitboardChessEngine();
            engine.New();

            Assert.IsNotNull(engine.GetPiece(new Position("E1")));
            Assert.IsNotNull(engine.GetPiece(new Position("E8")));
            Assert.IsNotNull(engine.GetPiece(new Position("A2")));
            Assert.IsNotNull(engine.GetPiece(new Position("H7")));
        }

        [TestMethod]
        public void CalculateMove_FromStart_ReturnsMove()
        {
            var engine = new BitboardChessEngine();
            engine.New();

            Move move = engine.CalculateMove();

            Assert.IsNotNull(move);
            Assert.IsTrue(move.Start.IsValidPosition());
            Assert.IsTrue(move.End.IsValidPosition());
        }

        [TestMethod]
        public void ExecuteMove_ValidMove_UpdatesBoard()
        {
            var engine = new BitboardChessEngine();
            engine.New();

            bool ok = engine.ExecuteMove(new Move(new Position("E2"), new Position("E4"), null));

            Assert.IsTrue(ok);
            Assert.IsNull(engine.GetPiece(new Position("E2")));
            Assert.IsNotNull(engine.GetPiece(new Position("E4")));
            Assert.AreEqual(Color.Black, engine.ColorToMove);
        }
    }
}
