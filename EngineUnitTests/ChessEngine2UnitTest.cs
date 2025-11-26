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
            ChessEngine2 chessEngine2 = new ChessEngine2();

            chessEngine2.SetPiece(new King(Color.White, "G6", MoveType.Normal, true));
            chessEngine2.SetPiece(new King(Color.Black, "H8", MoveType.Normal, true));
            chessEngine2.SetPiece(new Rook(Color.White, "A1", true, 0));


            Move move = chessEngine2.CalculateMove();

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }


        [TestMethod]
        public void CalculateOneMoveMate2()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2();

            chessEngine2.SetPiece(new King(Color.White, "G6", MoveType.Normal, true));
            chessEngine2.SetPiece(new Pawn(Color.White, "E4"));
            chessEngine2.SetPiece(new King(Color.Black, "G8", MoveType.Normal, true));
            chessEngine2.SetPiece(new Pawn(Color.Black,"D5"));
            chessEngine2.SetPiece(new Rook(Color.White, "A1", true, 0));
           
            Move move = chessEngine2.CalculateMove();

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));
        }
        
        [TestMethod]
        public void CalculatePawnBeat()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2();

            chessEngine2.SetPiece( new King(Color.White, "H1", MoveType.Normal, true));
            chessEngine2.SetPiece(new Pawn(Color.White, "E4"));
            chessEngine2.SetPiece(new King(Color.Black, "G8", MoveType.Normal, true));
            chessEngine2.SetPiece(new Pawn(Color.Black, "D5"));

            Move move = chessEngine2.CalculateMove();
            Assert.IsTrue(move.End.AreEqual(new Position("D5")));
        }

        [TestMethod]
        public void CheckStaleMate()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2();

            chessEngine2.SetPiece( new King(Color.White, "H3",MoveType.Normal, true));
            chessEngine2.SetPiece(new Pawn(Color.White, "H2"));
            chessEngine2.SetPiece(new Pawn(Color.White, "H4"));
            chessEngine2.SetPiece(new Pawn(Color.Black, "H5"));
            chessEngine2.SetPiece(new King(Color.Black, "H8", MoveType.Normal, true));
            chessEngine2.SetPiece(new Rook(Color.Black, "G8", true, 0));

            Move move = chessEngine2.CalculateMove();
            Assert.IsTrue(move.Rating.Evaluation == Evaluation.WhiteStaleMate);
        }

        [TestMethod]
        public void CalculateTwoMoveMate()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2();

            chessEngine2.SetPiece(new King(Color.White, "G6", MoveType.Normal, true));
            chessEngine2.SetPiece(new Pawn(Color.White, "C4"));
            chessEngine2.SetPiece(new King(Color.Black, "H8", MoveType.Normal, true));
            chessEngine2.SetPiece(new Pawn(Color.Black, "B5"));
            chessEngine2.SetPiece(new Rook(Color.White, "G5", true, 0));
           

            Move move = chessEngine2.CalculateMove();

            Assert.IsTrue(move.Rating.Evaluation == Evaluation.BlackCheckMate);
            Assert.IsTrue(move.Rating.Situation == Situation.WhiteVictory);
            Assert.IsTrue(move.Piece is Rook);
        }

        [TestMethod]
        public void CheckEnpassant()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2();

            chessEngine2.SetPiece(new King(Color.White, "G6", MoveType.Normal, true));
            chessEngine2.SetPiece(new Pawn(Color.White, "C2"));
            chessEngine2.SetPiece(new King(Color.Black, "H8", MoveType.Normal, true));
            chessEngine2.SetPiece(new Pawn(Color.Black, "B4"));
            

            chessEngine2.ExecuteMove(new Move("C2", "C4", chessEngine2["C2"], MoveType.PawnDoubleStep));
            Move move = chessEngine2.CalculateMove();
            chessEngine2.ExecuteMove(move);

            Assert.IsTrue(move.Type == MoveType.EnpassantBlackLowRow);

        }

        #region BoardRating

        [TestMethod]
        public void CheckBoardRatingBlackMate()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2();

            chessEngine2.SetPiece(new King(Color.White, "G6", MoveType.Normal, true));
            chessEngine2.SetPiece(new Rook(Color.White, "A8", true, 0));
            chessEngine2.SetPiece(new King(Color.Black, "G8", MoveType.Normal, true));

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
