using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine;
using MyChessEngine.Pieces;

namespace EngineUnitTests
{
    [TestClass]
    public class EngineClassTests
    {
        [TestMethod]
        public void CreateEngine()
        {
            ChessEngine chessEngine = new ChessEngine();
            Assert.IsNotNull(chessEngine);
        }

        [TestMethod]
        public void TestRating()
        {
            BoardRating whitMate1 = new BoardRating() {Evaluation = Evaluation.WhiteCheckMate};
            BoardRating whitMate2 = new BoardRating() {Evaluation = Evaluation.WhiteCheckMate};
            BoardRating blackMate1 = new BoardRating() {Evaluation = Evaluation.BlackCheckMate};
            BoardRating blackMate2 = new BoardRating() {Evaluation = Evaluation.BlackCheckMate};

            BoardRating whiteFavor3 = new BoardRating()
                {Evaluation = Evaluation.Normal, Value = 3};
            BoardRating equal1 = new BoardRating() {Evaluation = Evaluation.Normal, Value = 0};
            BoardRating equal2 = new BoardRating() {Evaluation = Evaluation.Normal, Value = 0};
            BoardRating blackFavor3 = new BoardRating()
                {Evaluation = Evaluation.Normal, Value = 3};

            List<BoardRating> ratings = new List<BoardRating>()
                {whitMate1, whitMate2, blackMate1, blackMate2, whiteFavor3, equal1, equal2, blackFavor3};
            MoveList moveList = new MoveList();
            ratings.ForEach(rating =>
            {
                Move move = new Move("A1", "A1", new Pawn(Color.White),
                    MoveType.Normal);
                move.Rating = rating;
                moveList.Add(move);
            });


            var x = RandomListAccess.GetShuffledList<Move>(moveList.Moves);

            Assert.IsNotNull(x);

        }
    }
}
