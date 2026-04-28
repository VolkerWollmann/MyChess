using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine.Bitboard;
using MyChessEngine.Pieces;
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

        [TestMethod]
        public void New_EmptyStartMode_IsEmptyBoard()
        {
            var engine = new BitboardChessEngine(BitboardChessEngine.StartPositionMode.Empty);
            engine.New();

            Assert.IsNull(engine.GetPiece(new Position("E1")));
            Assert.IsNull(engine.GetPiece(new Position("E8")));
            Assert.IsNull(engine.GetPiece(new Position("A2")));
            Assert.IsNull(engine.GetPiece(new Position("H7")));
        }

        [TestMethod]
        public void SetPiece_CanPlaceAndRemovePiece()
        {
            var engine = new BitboardChessEngine(BitboardChessEngine.StartPositionMode.Empty);
            engine.New();

            engine.SetPiece(new Position("E1"), new King(Color.White, "E1"));
            engine.SetPiece(new Position("E8"), new King(Color.Black, "E8"));
            engine.SetPiece(new Position("D4"), new Queen(Color.White, "D4"));

            IPiece piece = engine.GetPiece(new Position("D4"));
            Assert.IsNotNull(piece);
            Assert.AreEqual(PieceType.Queen, piece.Type);
            Assert.AreEqual(Color.White, piece.Color);

            engine.SetPiece(new Position("D4"), null);
            Assert.IsNull(engine.GetPiece(new Position("D4")));
        }
    }
}
