using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine;
using MyChessEngine.Bitboard;
using MyChessEngine.Pieces;
using MyChessEngineBase;
using System;

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

        [TestMethod]
        public void SetPiece_StringPosition_CanPlaceAndRemovePiece()
        {
            var engine = new BitboardChessEngine(BitboardChessEngine.StartPositionMode.Empty);
            engine.New();

            engine.SetPiece("E1", new King(Color.White, "E1"));
            engine.SetPiece("E8", new King(Color.Black, "E8"));
            engine.SetPiece("D5", new Queen(Color.White, "D5"));

            IPiece piece = engine.GetPiece(new Position("D5"));
            Assert.IsNotNull(piece);
            Assert.AreEqual(PieceType.Queen, piece.Type);
            Assert.AreEqual(Color.White, piece.Color);

            engine.SetPiece("D5", null);
            Assert.IsNull(engine.GetPiece(new Position("D5")));
        }

        [TestMethod]
        public void CalculateOneMoveMate()
        {
            var chessEngine = new BitboardChessEngine(BitboardChessEngine.StartPositionMode.Empty);
            chessEngine.SetPiece("G6",new King(Color.White, "G6"));

            chessEngine.SetPiece("G8",new King(Color.Black, "G8"));
            chessEngine.SetPiece("A1",new Rook(Color.White, "A1"));

            Move move = chessEngine.CalculateMoveWithDepth(6);

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }

        [TestMethod]
        public void CalculateTwoMoveMate()
        {
            var chessEngine = new BitboardChessEngine(BitboardChessEngine.StartPositionMode.Empty);

            chessEngine.SetPiece("G6",new King(Color.White, "G6", MoveType.Normal, 1));
            chessEngine.SetPiece("C4",new Pawn(Color.White, "C4"));
            chessEngine.SetPiece("H8",new King(Color.Black, "H8", MoveType.Normal, 1));
            chessEngine.SetPiece("B5",new Pawn(Color.Black, "B5"));
            chessEngine.SetPiece("G5",new Rook(Color.White, "G5", 1));


            Move move = chessEngine.CalculateMoveWithDepth(6);

            Assert.AreEqual(Evaluation.BlackCheckMate, move.Rating.Evaluation);
            Assert.AreEqual(Situation.WhiteVictory, move.Rating.Situation);
            Assert.IsTrue(move.Piece is Rook);

            Console.WriteLine(move.Rating.MoveList);
        }

        [TestMethod]
        public void CalculateTwoMoveMateWithDepth8()
        {
            var chessEngine = new BitboardChessEngine(BitboardChessEngine.StartPositionMode.Empty);

            chessEngine.SetPiece("G6", new King(Color.White, "G6", MoveType.Normal, 1));
            chessEngine.SetPiece("C4", new Pawn(Color.White, "C4"));
            chessEngine.SetPiece("H8", new King(Color.Black, "H8", MoveType.Normal, 1));
            chessEngine.SetPiece("B5", new Pawn(Color.Black, "B5"));
            chessEngine.SetPiece("G5", new Rook(Color.White, "G5", 1));


            Move move = chessEngine.CalculateMoveWithDepth(8);

            Assert.AreEqual(Evaluation.BlackCheckMate, move.Rating.Evaluation);
            Assert.AreEqual(Situation.WhiteVictory, move.Rating.Situation);
            Assert.IsTrue(move.Piece is Rook);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        public void CalculateOpeningMove(int depth)
        {
            var chessEngine = new BitboardChessEngine(BitboardChessEngine.StartPositionMode.Classic);

            Move move = chessEngine.CalculateMoveWithDepth(depth);
            Assert.IsNotNull(move);
        }

        [TestMethod]
        public void CopyTest()
        {
            var chessEngine = new BitboardChessEngine(BitboardChessEngine.StartPositionMode.Empty);
            var copy = chessEngine.CaptureState();
        }
    }
}
