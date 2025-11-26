using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine;
using MyChessEngine.Pieces;
using MyChessEngineBase;
using MyChessEngineBase.Rating;

namespace EngineUnitTests
{
    [TestClass]
    public class ChessEngineUnitTests
    {
        [TestMethod]
        public void CreateEngine()
        {
            ChessEngine chessEngine = new ChessEngine();
            Assert.IsNotNull(chessEngine);
        }

        #region BoardRating

        [TestMethod]
        public void CheckStartBoardRating()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.New();

            foreach (Color color in ChessEngineConstants.BothColors)
            {
                BoardRating boardRating = chessEngine.GetRating(color);

                Assert.IsTrue(boardRating.Situation == Situation.Normal);
                Assert.IsTrue(boardRating.Evaluation == Evaluation.Normal);
                Assert.AreEqual(boardRating.Weight, 0);
            }

        }

        [TestMethod]
        public void CheckMoveList()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.New();

            var x = chessEngine.GetMoveList();
        }

        [TestMethod]
        public void CheckBoardRatingBlackMate()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, true));
            chessEngine.SetPiece(new King(Color.Black, "G8", MoveType.Normal, true));
            chessEngine.SetPiece(new Rook(Color.White, "A8", true, 0));
            chessEngine.ColorToMove = Color.Black;
            Move move = chessEngine.CalculateMove();

            BoardRating rating = move.Rating;
            Assert.AreEqual(rating.Situation, Situation.WhiteVictory);
            Assert.AreEqual(rating.Evaluation, Evaluation.BlackCheckMate);
        }

        #endregion

        #region MoveCalculation

        [TestMethod]
        public void CalculateOpeningMove()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.New();

            Move move = chessEngine.CalculateMove();
            Assert.IsNotNull(move);
        }

        [TestMethod]
        public void CalculateOpeningMoveBlack()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.New();

            chessEngine.ExecuteMove(new Move("E2", "E4", chessEngine["E2"].Piece, MoveType.PawnDoubleStep));
            Move move = chessEngine.CalculateMove();
            Assert.IsNotNull(move);
        }

        [TestMethod]
        public void CalculateOneMoveMate()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, true));
            chessEngine.SetPiece(new King(Color.Black, "G8", MoveType.Normal, true));
            chessEngine.SetPiece(new Rook(Color.White, "A1", false, 0));

            Move move = chessEngine.CalculateMove();

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }

        [TestMethod]
        public void CalculateTwoMoveMate()
        {
            ChessEngine chessEngine = new ChessEngine();

            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, true));
            chessEngine.SetPiece(new Pawn(Color.White, "C4"));
            chessEngine.SetPiece(new King(Color.Black, "H8", MoveType.Normal, true));
            chessEngine.SetPiece(new Pawn(Color.Black, "B5"));
            chessEngine.SetPiece(new Rook(Color.White, "G5", true, 0));
            

            Move move = chessEngine.CalculateMoveWithDepth(6);

            Assert.IsTrue(move.Rating.Evaluation == Evaluation.BlackCheckMate);
            Assert.IsTrue(move.Rating.Situation == Situation.WhiteVictory);
            Assert.IsTrue(move.Piece is Rook);
        }

        [TestMethod]
        public void CalculateTwoMoveMateParallel()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, true));
            chessEngine.SetPiece(new Pawn(Color.White, "E4"));
            chessEngine.SetPiece(new King(Color.Black, "G8", MoveType.Normal, true));
            chessEngine.SetPiece(new Pawn(Color.Black, "D5"));
            chessEngine.SetPiece(new Rook(Color.White, "A1", true, 0));


            Move move = chessEngine.CalculateMoveWithDepthParallel(6);
            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }

        [TestMethod]
        public void CalculateTwoMoveMateWithDepth8()
        {
            ChessEngine chessEngine = new ChessEngine();

            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, true));
            chessEngine.SetPiece(new Pawn(Color.White, "C4"));
            chessEngine.SetPiece(new King(Color.Black, "H8", MoveType.Normal, true));
            chessEngine.SetPiece(new Pawn(Color.Black, "B5"));
            chessEngine.SetPiece(new Rook(Color.White, "G5", true, 0));


            Move move = chessEngine.CalculateMoveWithDepth(8);

            Assert.IsTrue(move.Rating.Evaluation == Evaluation.BlackCheckMate);
            Assert.IsTrue(move.Rating.Situation == Situation.WhiteVictory);
            Assert.IsTrue(move.Piece is Rook);
        }

        [TestMethod]
        public void CalculateTwoMoveMateWithDepth8Parallel()
        {
            ChessEngine chessEngine = new ChessEngine();

            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, true));
            chessEngine.SetPiece(new Pawn(Color.White, "C4"));
            chessEngine.SetPiece(new King(Color.Black, "H8", MoveType.Normal, true));
            chessEngine.SetPiece(new Pawn(Color.Black, "B5"));
            chessEngine.SetPiece(new Rook(Color.White, "G5", true, 0));


            Move move = chessEngine.CalculateMoveWithDepthParallel(8);

            Assert.IsTrue(move.Rating.Evaluation == Evaluation.BlackCheckMate);
            Assert.IsTrue(move.Rating.Situation == Situation.WhiteVictory);
            Assert.IsTrue(move.Piece is Rook);
        }

        [TestMethod]
        public void CheckEnpassant()
        {
            ChessEngine chessEngine = new ChessEngine();

            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, true));
            chessEngine.SetPiece(new Pawn(Color.White, "B2"));
            chessEngine.SetPiece(new King(Color.Black, "H8", MoveType.Normal, true));
            chessEngine.SetPiece(new Pawn(Color.Black, "C4"));

            chessEngine.ColorToMove = Color.White;
            var x = chessEngine.GetMoveList();
            chessEngine.ExecuteMove(new Move("B2", "B4", chessEngine["B2"].Piece, MoveType.PawnDoubleStep));
            Move move = chessEngine.CalculateMoveWithDepth(4);
            //chessEngine.ExecuteMove(move);

            Assert.IsTrue(move.Type == MoveType.EnpassantBlackHighRow);

        }

        [TestMethod]
        public void CalculatePawnBeat()
        {
            ChessEngine chessEngine = new ChessEngine();

            chessEngine.SetPiece(new King(Color.White, "H1", MoveType.Normal, true));
            chessEngine.SetPiece(new Pawn(Color.White, "E4"));
            chessEngine.SetPiece(new King(Color.Black, "G8", MoveType.Normal, true));
            chessEngine.SetPiece(new Pawn(Color.Black, "D5"));

            Move move = chessEngine.CalculateMoveWithDepth(4);
            Assert.IsTrue(move.End.AreEqual(new Position("D5")));
        }

        #endregion
    }
}
