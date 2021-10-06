using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine.Pieces;
using MyChessEngineBase;
using MyChessEngineBase.Rating;
using MyChessEngineInteger;

namespace EngineUnitTests
{
    [TestClass]
    public class ChessEngineIntegerUnitTests
    {
        [TestMethod]
        public void CheckBoardRatingBlackMate()
        {
            ChessEngineInteger chessEngineInteger = new ChessEngineInteger
            {
                ["G6"] = new King(Color.White, MoveType.Normal),
                ["A8"] = new Rook(Color.White),
                ["H8"] = new King(Color.Black, MoveType.Normal)
            };

            BoardRating boardRating = chessEngineInteger.GetRating(Color.Black);
            Assert.AreEqual(boardRating.Situation, Situation.WhiteVictory);
        }

        [TestMethod]
        public void CheckBoardRatingBlackMateCalculateMove()
        {
            ChessEngineInteger chessEngineInteger = new ChessEngineInteger
            {
                ["G6"] = new King(Color.White, MoveType.Normal),
                ["A8"] = new Rook(Color.White),
                ["H8"] = new King(Color.Black, MoveType.Normal),
                ColorToMove = Color.Black
            };

            MyChessEngineBase.Move move = chessEngineInteger.CalculateMove();

        }

        [TestMethod]
        public void CalculateOneMoveMate1()
        {
            ChessEngineInteger chessEngineInteger = new ChessEngineInteger
            {
                ["G6"] = new King(Color.White, MoveType.Normal),
                ["H8"] = new King(Color.Black, MoveType.Normal),
                ["A1"] = new Rook(Color.White)
            };

            MyChessEngineBase.Move move = chessEngineInteger.CalculateMove();

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }
    }
}
