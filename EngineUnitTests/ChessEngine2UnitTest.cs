using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine;
using MyChessEngine.Pieces;

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
        public void CalculateOneMoveMate1()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2();

            chessEngine2["G6"] = new King(Color.White, MoveType.Normal);
            chessEngine2["H8"] = new King(Color.Black, MoveType.Normal);
            chessEngine2["A1"] = new Rook(Color.White);

            Move move = chessEngine2.CalculateMove();

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }


        [TestMethod]
        public void CalculateOneMoveMate2()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2();

            chessEngine2["G6"] = new King(Color.White, MoveType.Normal);
            chessEngine2["E4"] = new Pawn(Color.White);
            chessEngine2["G8"] = new King(Color.Black, MoveType.Normal);
            chessEngine2["D5"] = new Pawn(Color.Black);
            chessEngine2["A1"] = new Rook(Color.White);

            Move move = chessEngine2.CalculateMove();

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }
        
        [TestMethod]
        public void CheckBoardRatingBlackMate()
        {
            ChessEngine2 chessEngine2 = new ChessEngine2();

            chessEngine2["G6"] = new King(Color.White, MoveType.Normal);
            chessEngine2["A8"] = new Rook(Color.White);
            chessEngine2["G8"] = new King(Color.Black, MoveType.Normal);

            BoardRating boardRating = chessEngine2.GetRating(Color.Black);
            Assert.AreEqual(boardRating.Situation, Situation.WhiteVictory);
        }
    }
}
