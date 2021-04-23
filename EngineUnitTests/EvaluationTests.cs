using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine;
using MyChessEngine.Rating;
using MyChessEngine.Pieces;


namespace EngineUnitTests
{
    [TestClass]
    public class EvaluationTests
    {

        #region Move list order

        private MoveList CreateMoveList()
        {
            BoardRating whitMate1 = new BoardRating() { Situation = Situation.BlackVictory, Evaluation = Evaluation.WhiteCheckMate };
            BoardRating whitMate2 = new BoardRating() { Situation = Situation.StaleMate, Evaluation = Evaluation.BlackStaleMate };
            BoardRating blackMate1 = new BoardRating() { Situation = Situation.WhiteVictory, Evaluation = Evaluation.BlackCheckMate };
            BoardRating blackMate2 = new BoardRating() { Situation = Situation.WhiteVictory, Evaluation = Evaluation.BlackCheckMate };

            BoardRating whiteFavor3 = new BoardRating() { Evaluation = Evaluation.Normal, Weight = 3 };
            BoardRating equal1 = new BoardRating() { Evaluation = Evaluation.Normal, Weight = 0 };
            BoardRating equal2 = new BoardRating() { Evaluation = Evaluation.Normal, Weight = 0 };
            BoardRating blackFavor3 = new BoardRating() { Evaluation = Evaluation.Normal, Weight = -3 };

            List<BoardRating> ratings = new List<BoardRating>()
                {whitMate1, whitMate2, blackMate1, blackMate2, whiteFavor3, equal1, equal2, blackFavor3};

            ratings = RandomListAccess.GetShuffledList(ratings);

            int i = 1;
            MoveList moveList = new MoveList();
            ratings.ForEach(rating =>
            {
                Move move = new Move("A" + i, "A1", new Pawn(Color.White), MoveType.Normal) { Rating = rating };
                moveList.Add(move);
                i++;
            });

            return moveList;
        }

        [TestMethod]
        public void MoveListPreOrder()
        {
            MoveList moveList = CreateMoveList();

            var whiteBestMove = moveList.GetBestMove(Color.White);

            Assert.IsNotNull(whiteBestMove.Rating.Evaluation == Evaluation.BlackCheckMate);

            moveList.Sort(Color.White);
            var ratingList = moveList.Moves.Select(move => move.Rating).ToList();

            IBoardRatingComparer boardRatingComparer = BoardRatingComparerFactory.GetComparer(Color.White);
            for (int i = 0; i < ratingList.Count - 1; i++)
                Assert.IsTrue(boardRatingComparer.Compare(ratingList[i], ratingList[i + 1]) <= 0);

        }

        private List<BoardRating> CreateBoardRatingList()
        {
            List<BoardRating> result = new List<BoardRating>();
            Random random = new Random();

            for (int i = 0; i < 40; i++)
            {
                int split = random.Next(0, 100);
                BoardRating boardRating;
                if (split <= 5)
                    boardRating = new BoardRating() { Evaluation = Evaluation.WhiteCheckMate, Depth = random.Next(1,5) };
                else if (split <= 10)
                    boardRating = new BoardRating() { Evaluation = Evaluation.BlackCheckMate, Depth = random.Next(1, 5) };
                else if (split <= 15)
                    boardRating = new BoardRating() { Evaluation = Evaluation.WhiteStaleMate, Depth = random.Next(1, 5) };
                else if (split <= 20)
                    boardRating = new BoardRating() { Evaluation = Evaluation.BlackStaleMate, Depth = random.Next(1, 5) };
                else if (split <= 25)
                    boardRating = new BoardRating() { Evaluation = Evaluation.Normal, Weight = 0, Depth = random.Next(1, 5) };
                else
                    boardRating = new BoardRating() { Evaluation = Evaluation.Normal, Weight = random.Next(-50, 50), Depth = random.Next(1, 5) };

                result.Add(boardRating);
            }

            return result;
        }

        [TestMethod]
        public void RatingPreOrder()
        {
            List<BoardRating> ratingList = CreateBoardRatingList();

            IBoardRatingComparer boardRatingWhiteComparer = BoardRatingComparerFactory.GetComparer(Color.White);

            var orderedListWhite = ratingList.Select(rating => rating).ToList();
            orderedListWhite.Sort(boardRatingWhiteComparer);

            LinkedList<BoardRating> lw = new LinkedList<BoardRating>(orderedListWhite);
            var pairsWhite = lw.Select(e => new Tuple<BoardRating, BoardRating>(e, lw.Find(e)?.Next?.Value)).ToList();
            Assert.IsTrue(pairsWhite.All(t =>
                (t.Item2 == null) || boardRatingWhiteComparer.Compare(t.Item1, t.Item2) <= 0));

            IBoardRatingComparer boardRatingBlackComparer = BoardRatingComparerFactory.GetComparer(Color.Black);

            var orderedListBlack = ratingList.Select(rating => rating).ToList();
            orderedListBlack.Sort(boardRatingBlackComparer);

            LinkedList<BoardRating> lb = new LinkedList<BoardRating>(orderedListWhite);
            var pairsBlack = lb.Select(e => new Tuple<BoardRating, BoardRating>(e, lb.Find(e)?.Next?.Value)).ToList();
            Assert.IsTrue(pairsBlack.All(t =>
                (t.Item2 == null) || boardRatingWhiteComparer.Compare(t.Item1, t.Item2) <= 0));
        }

        #endregion
    }
}
