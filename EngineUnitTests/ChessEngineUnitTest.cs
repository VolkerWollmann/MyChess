using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine;
using MyChessEngine.Pieces;

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

        #region Move list order

        private MoveList CreateMoveList()
        {
            BoardRating whitMate1 = new BoardRating() {Situation= Situation.BlackVictory, Evaluation = Evaluation.WhiteCheckMate};
            BoardRating whitMate2 = new BoardRating() { Situation = Situation.StaleMate, Evaluation = Evaluation.BlackStaleMate};
            BoardRating blackMate1 = new BoardRating() { Situation = Situation.WhiteVictory, Evaluation = Evaluation.BlackCheckMate};
            BoardRating blackMate2 = new BoardRating() { Situation = Situation.WhiteVictory, Evaluation = Evaluation.BlackCheckMate};

            BoardRating whiteFavor3 = new BoardRating() {Evaluation = Evaluation.Normal, Weight = 3};
            BoardRating equal1 = new BoardRating() {Evaluation = Evaluation.Normal, Weight = 0};
            BoardRating equal2 = new BoardRating() {Evaluation = Evaluation.Normal, Weight = 0};
            BoardRating blackFavor3 = new BoardRating() {Evaluation = Evaluation.Normal, Weight = -3};

            List<BoardRating> ratings = new List<BoardRating>()
                {whitMate1, whitMate2, blackMate1, blackMate2, whiteFavor3, equal1, equal2, blackFavor3};

            ratings = RandomListAccess.GetShuffledList(ratings);

            int i = 1;
            MoveList moveList = new MoveList();
            ratings.ForEach(rating =>
            {
                Move move = new Move("A" + i, "A1", new Pawn(Color.White), MoveType.Normal) {Rating = rating};
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

            BoardRatingComparer boardRatingComparer = new BoardRatingComparer(Color.White);
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
                    boardRating = new BoardRating() {Evaluation = Evaluation.WhiteCheckMate};
                else if (split <= 10)
                    boardRating = new BoardRating() {Evaluation = Evaluation.BlackCheckMate};
                else if (split <= 15)
                    boardRating = new BoardRating() {Evaluation = Evaluation.WhiteStaleMate};
                else if (split <= 20)
                    boardRating = new BoardRating() {Evaluation = Evaluation.BlackStaleMate};
                else if (split <= 25)
                    boardRating = new BoardRating() {Evaluation = Evaluation.Normal, Weight = 0};
                else
                    boardRating = new BoardRating() {Evaluation = Evaluation.Normal, Weight = random.Next(-50, 50)};

                result.Add(boardRating);
            }

            return result;
        }

        [TestMethod]
        public void RatingPreOrder()
        {
            List<BoardRating> ratingList = CreateBoardRatingList();

            BoardRatingComparer boardRatingWhiteComparer = new BoardRatingComparer(Color.White);

            var orderedListWhite = ratingList.Select(rating => rating).ToList();
            orderedListWhite.Sort(boardRatingWhiteComparer);

            LinkedList<BoardRating> lw = new LinkedList<BoardRating>(orderedListWhite);
            var pairsWhite = lw.Select(e => new Tuple<BoardRating, BoardRating>(e, lw.Find(e)?.Next?.Value)).ToList();
            Assert.IsTrue(pairsWhite.All(t =>
                (t.Item2 == null) || boardRatingWhiteComparer.Compare(t.Item1, t.Item2) <= 0));



            BoardRatingComparer boardRatingBlackComparer = new BoardRatingComparer(Color.Black);

            var orderedListBlack = ratingList.Select(rating => rating).ToList();
            orderedListBlack.Sort(boardRatingBlackComparer);

            LinkedList<BoardRating> lb = new LinkedList<BoardRating>(orderedListWhite);
            var pairsBlack = lb.Select(e => new Tuple<BoardRating, BoardRating>(e, lb.Find(e)?.Next?.Value)).ToList();
            Assert.IsTrue(pairsBlack.All(t =>
                (t.Item2 == null) || boardRatingWhiteComparer.Compare(t.Item1, t.Item2) <= 0));
        }

        #endregion

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
        public void CheckBoardRatingBlackMate()
        {
            ChessEngine chessEngine = new ChessEngine();


            chessEngine["G6"] = new King(Color.White);
            chessEngine["A8"] = new Rook(Color.White);
            chessEngine["G8"] = new King(Color.Black);

            BoardRating boardRating = chessEngine.GetRating(Color.Black);
            Assert.AreEqual(boardRating.Situation, Situation.WhiteVictory);
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
        public void CalculatePawnBeat()
        {
            ChessEngine chessEngine = new ChessEngine();


            chessEngine["E1"] = new King(Color.White, MoveType.Normal);
            chessEngine["E4"] = new Pawn(Color.White);
            chessEngine["G8"] = new King(Color.Black, MoveType.Normal);
            chessEngine["D5"] = new Pawn(Color.Black);

            Move move = chessEngine.CalculateMove();
            Assert.IsTrue(move.End.AreEqual(new Position("D5")));
        }

        [TestMethod]
        public void CheckStaleMate()
        {
            ChessEngine chessEngine = new ChessEngine();


            chessEngine["H3"] = new King(Color.White, MoveType.Normal);
            chessEngine["H2"] = new Pawn(Color.White);
            chessEngine["H4"] = new Pawn(Color.White);
            chessEngine["H5"] = new Pawn(Color.Black);
            chessEngine["H8"] = new King(Color.Black, MoveType.Normal);
            chessEngine["G8"] = new Rook(Color.Black);

            Move move = chessEngine.CalculateMove();
            Assert.IsTrue(move.Rating.Evaluation == Evaluation.WhiteStaleMate);
        }

        [TestMethod]
        public void CalculateOneMoveMate()
        {
            ChessEngine chessEngine = new ChessEngine();

            chessEngine["G6"] = new King(Color.White, MoveType.Normal );
            chessEngine["E4"] = new Pawn(Color.White);
            chessEngine["G8"] = new King(Color.Black, MoveType.Normal);
            chessEngine["D5"] = new Pawn(Color.Black);
            chessEngine["A1"] = new Rook(Color.White);

            Move move = chessEngine.CalculateMove();

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));
            
        }

        [TestMethod]
        public void CalculateTwoMoveMate()
        {
            ChessEngine chessEngine = new ChessEngine();

            chessEngine["G6"] = new King(Color.White, MoveType.Normal);
            chessEngine["C4"] = new Pawn(Color.White);
            chessEngine["H8"] = new King(Color.Black, MoveType.Normal);
            chessEngine["B5"] = new Pawn(Color.Black);
            chessEngine["G5"] = new Rook(Color.White);

            Move move = chessEngine.CalculateMove();

            Assert.IsTrue(move.Rating.Evaluation == Evaluation.BlackCheckMate);
            Assert.IsTrue(move.Rating.Situation == Situation.WhiteVictory);
            Assert.IsTrue(move.Piece is Rook);
        }

        #endregion
    }
}
