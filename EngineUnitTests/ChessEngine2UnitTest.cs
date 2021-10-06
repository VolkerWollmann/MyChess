using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine;
using MyChessEngine.Pieces;
using MyChessEngineBase;
using MyChessEngineBase.Rating;

namespace EngineUnitTests
{
    [TestClass]
    public class ChessEngine2UnitTest
    {
        [TestMethod]
        public void CalculateOpeningMove()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2();
            chessEngine2.New();

            Move move = chessEngine2.CalculateMove();
            Assert.IsNotNull(move);
        }

        [TestMethod]
        public void CalculateOpeningMoveBlack()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2();
            chessEngine2.New();

            chessEngine2.ExecuteMove(new Move("E2", "E4", chessEngine2["E2"], MoveType.PawnDoubleStep));
            Move move = chessEngine2.CalculateMove();
            Assert.IsNotNull(move);
        }

        [TestMethod]
        public void CalculateOneMoveMate1()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2
            {
                ["G6"] = new King(Color.White, MoveType.Normal),
                ["H8"] = new King(Color.Black, MoveType.Normal),
                ["A1"] = new Rook(Color.White)
            };

            Move move = chessEngine2.CalculateMove();

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }


        [TestMethod]
        public void CalculateOneMoveMate2()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2
            {
                ["G6"] = new King(Color.White, MoveType.Normal),
                ["E4"] = new Pawn(Color.White),
                ["G8"] = new King(Color.Black, MoveType.Normal),
                ["D5"] = new Pawn(Color.Black),
                ["A1"] = new Rook(Color.White)
            };

            Move move = chessEngine2.CalculateMove();

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }
        
        [TestMethod]
        public void CalculatePawnBeat()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2
            {
                ["H1"] = new King(Color.White, MoveType.Normal),
                ["E4"] = new Pawn(Color.White),
                ["G8"] = new King(Color.Black, MoveType.Normal),
                ["D5"] = new Pawn(Color.Black)
            };


            Move move = chessEngine2.CalculateMove();
            Assert.IsTrue(move.End.AreEqual(new Position("D5")));
        }

        [TestMethod]
        public void CheckStaleMate()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2
            {
                ["H3"] = new King(Color.White, MoveType.Normal),
                ["H2"] = new Pawn(Color.White),
                ["H4"] = new Pawn(Color.White),
                ["H5"] = new Pawn(Color.Black),
                ["H8"] = new King(Color.Black, MoveType.Normal),
                ["G8"] = new Rook(Color.Black)
            };


            Move move = chessEngine2.CalculateMove();
            Assert.IsTrue(move.Rating.Evaluation == Evaluation.WhiteStaleMate);
        }

        [TestMethod]
        public void CalculateTwoMoveMate()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2
            {
                ["G6"] = new King(Color.White, MoveType.Normal),
                ["C4"] = new Pawn(Color.White),
                ["H8"] = new King(Color.Black, MoveType.Normal),
                ["B5"] = new Pawn(Color.Black),
                ["G5"] = new Rook(Color.White)
            };

            Move move = chessEngine2.CalculateMove();

            Assert.IsTrue(move.Rating.Evaluation == Evaluation.BlackCheckMate);
            Assert.IsTrue(move.Rating.Situation == Situation.WhiteVictory);
            Assert.IsTrue(move.Piece is Rook);
        }

        [TestMethod]
        public void CheckEnpassant()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2
            {
                ["G6"] = new King(Color.White, MoveType.Normal),
                ["C2"] = new Pawn(Color.White),
                ["H8"] = new King(Color.Black, MoveType.Normal),
                ["B4"] = new Pawn(Color.Black)
            };

            chessEngine2.ExecuteMove(new Move("C2", "C4", chessEngine2["C2"], MoveType.PawnDoubleStep));
            Move move = chessEngine2.CalculateMove();
            chessEngine2.ExecuteMove(move);

            Assert.IsTrue(move.Type == MoveType.EnpassantBlackLeft);

        }

        #region BoardRating

        [TestMethod]
        public void CheckBoardRatingBlackMate()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2
            {
                ["G6"] = new King(Color.White, MoveType.Normal),
                ["A8"] = new Rook(Color.White),
                ["G8"] = new King(Color.Black, MoveType.Normal)
            };

            BoardRating boardRating = chessEngine2.GetRating(Color.Black);
            Assert.AreEqual(boardRating.Situation, Situation.WhiteVictory);
        }

        [TestMethod]
        public void CheckStartBoardRating()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2();
            chessEngine2.New();

            foreach (Color color in ChessEngineConstants.BothColors)
            {
                BoardRating boardRating = chessEngine2.GetRating(color);

                Assert.IsTrue(boardRating.Situation == Situation.Normal);
                Assert.IsTrue(boardRating.Evaluation == Evaluation.Normal);
                Assert.AreEqual(boardRating.Weight, 0);
            }
        }

        #endregion
    }
}
