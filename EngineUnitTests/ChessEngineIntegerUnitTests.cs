using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine.Pieces;
using MyChessEngineBase;
using MyChessEngineInteger;

namespace EngineUnitTests
{
    [TestClass]
    public class ChessEngineIntegerUnitTests
    {
        [TestMethod]
        public void CalculateOneMoveMate1()
        {
            ChessEngineInteger chessEngine2 = new ChessEngineInteger
            {
                ["G6"] = new King(Color.White, MoveType.Normal),
                ["H8"] = new King(Color.Black, MoveType.Normal),
                ["A1"] = new Rook(Color.White)
            };

            MyChessEngineBase.Move move = chessEngine2.CalculateMove();

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }
    }
}
