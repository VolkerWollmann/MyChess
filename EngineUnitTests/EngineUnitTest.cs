using System;
using System.Collections.Generic;
using System.Linq;
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

        private MoveList CreateMoveList()
        {
            BoardRating whitMate1 = new BoardRating() { Evaluation = Evaluation.WhiteCheckMate };
            BoardRating whitMate2 = new BoardRating() { Evaluation = Evaluation.WhiteCheckMate };
            BoardRating blackMate1 = new BoardRating() { Evaluation = Evaluation.BlackCheckMate };
            BoardRating blackMate2 = new BoardRating() { Evaluation = Evaluation.BlackCheckMate };

            BoardRating whiteFavor3 = new BoardRating() { Evaluation = Evaluation.Normal, Value = 3 };
            BoardRating equal1 = new BoardRating() { Evaluation = Evaluation.Normal, Value = 0 };
            BoardRating equal2 = new BoardRating() { Evaluation = Evaluation.Normal, Value = 0 };
            BoardRating blackFavor3 = new BoardRating() { Evaluation = Evaluation.Normal, Value = -3 };

            List<BoardRating> ratings = new List<BoardRating>()
                {whitMate1, whitMate2, blackMate1, blackMate2, whiteFavor3, equal1, equal2, blackFavor3};

            ratings = RandomListAccess.GetShuffledList<BoardRating>(ratings);

            int i = 1;
            MoveList moveList = new MoveList();
            ratings.ForEach(rating =>
            {
                Move move = new Move("A"+i, "A1", new Pawn(Color.White), MoveType.Normal) {Rating = rating};
                moveList.Add(move);
                i++;
            });

            return moveList;
        }

        [TestMethod]
        public void TestRating()
        {
            MoveList moveList = CreateMoveList();
            
            var whiteBestMove = moveList.GetBestMove(Color.White);

            Assert.IsNotNull(whiteBestMove.Rating.Evaluation == Evaluation.BlackCheckMate);

            moveList.BubbleSort(Color.White);
            var ratingList = moveList.Moves.Select(move => move.Rating).ToList();

        }

        [TestMethod]
        public void TestPartialOrdering()
        {
            MoveList moveList = CreateMoveList();

            var initialList = moveList.Moves.Select(move => move.Rating).ToList();

            moveList.BubbleSort(Color.White);
            var orderedList = moveList.Moves.Select(move => move.Rating).ToList();
        }
    }
}
