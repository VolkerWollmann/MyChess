using System.Linq;
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

        [TestMethod]
        public void CompareBoard()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.New();
            Board board = chessEngine.Board;
            Board board2 = board.Copy();

            Assert.IsTrue(board.Compare(board2));

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

                Assert.AreEqual(Situation.Normal, boardRating.Situation);
                Assert.AreEqual(Evaluation.Normal, boardRating.Evaluation);
                Assert.AreEqual(0, boardRating.Weight);
            }

        }

        [TestMethod]
        public void CheckMoveList()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.New();

            var x = chessEngine.GetMoveList();

            Assert.HasCount(20, x.Moves);
        }

        [TestMethod]
        public void CheckBoardRatingBlackMate()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, 1));
            chessEngine.SetPiece(new King(Color.Black, "G8", MoveType.Normal, 1));
            chessEngine.SetPiece(new Rook(Color.White, "A8", 1));
            chessEngine.ColorToMove = Color.Black;
            Move move = chessEngine.CalculateMove();

            BoardRating rating = move.Rating;
            Assert.AreEqual(Situation.WhiteVictory, rating.Situation);
            Assert.AreEqual(Evaluation.BlackCheckMate, rating.Evaluation);
        }

		#endregion

		#region MoveCalculation

		[DataTestMethod]
		[DataRow(1)]
		[DataRow(2)]
		[DataRow(3)]
		[DataRow(4)]
		[DataRow(5)]
		public void CalculateOpeningMoveParallel(int depth)
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.New();

            Move move = chessEngine.CalculateMoveWithDepthParallel(depth);
            Assert.IsNotNull(move);
        }

		[DataTestMethod]
		[DataRow(1)]
		[DataRow(2)]
		[DataRow(3)]
		[DataRow(4)]
		[DataRow(5)]
        [DataRow(6)]
        public void CalculateOpeningMove(int depth)
		{
			ChessEngine chessEngine = new ChessEngine();
			chessEngine.New();

			Move move = chessEngine.CalculateMoveWithDepth(depth);
			Assert.IsNotNull(move);
		}

		[TestMethod]
        public void A1()
        {
	        ChessEngine chessEngine = new ChessEngine();
	        chessEngine.New();

	        var w1MoveList = chessEngine.GetMoveList();
	        var w1Move = w1MoveList.Moves.First(m => m.Start.ToString() == "A2" && m.End.ToString() == "A4");
	        chessEngine.ExecuteMove(w1Move);

            var b1MoveList = chessEngine.GetMoveList();
            var b1Move = b1MoveList.Moves.First(m => m.Start.ToString() == "B7" && m.End.ToString() == "B5");
            chessEngine.ExecuteMove(b1Move);

            var w2MoveList = chessEngine.GetMoveList();
            var w2Move = w2MoveList.Moves.First(m => m.Start.ToString() == "A1" && m.End.ToString() == "A3");
            chessEngine.ExecuteMove(w2Move);

			Move move = chessEngine.CalculateMoveWithDepth();
		}

		[TestMethod]
        public void CalculateOpeningMoveBlackParallel()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.New();

            chessEngine.ExecuteMove(new Move("E2", "E4", chessEngine["E2"].Piece, MoveType.PawnDoubleStep));
            Move move = chessEngine.CalculateMove();
            Assert.IsNotNull(move);
        }

        [TestMethod]
        public void CalculateOneMoveMateParallel()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, 1));
            chessEngine.SetPiece(new King(Color.Black, "G8", MoveType.Normal, 1));
            chessEngine.SetPiece(new Rook(Color.White, "A1", 1));

            Move move = chessEngine.CalculateMove();

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }

        [TestMethod]
        public void CalculateOneMoveMate()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, 1));
            chessEngine.SetPiece(new King(Color.Black, "G8", MoveType.Normal, 1));
            chessEngine.SetPiece(new Rook(Color.White, "A1", 1));

            Move move = chessEngine.CalculateMoveWithDepth(6);

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }

        [TestMethod]
        public void CalculateTwoMoveMate()
        {
            ChessEngine chessEngine = new ChessEngine();

            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, 1));
            chessEngine.SetPiece(new Pawn(Color.White, "C4"));
            chessEngine.SetPiece(new King(Color.Black, "H8", MoveType.Normal, 1));
            chessEngine.SetPiece(new Pawn(Color.Black, "B5"));
            chessEngine.SetPiece(new Rook(Color.White, "G5",1));
            

            Move move = chessEngine.CalculateMoveWithDepth(6);

            Assert.AreEqual(Evaluation.BlackCheckMate, move.Rating.Evaluation);
            Assert.AreEqual(Situation.WhiteVictory, move.Rating.Situation);
            Assert.IsTrue(move.Piece is Rook);
        }

        [TestMethod]
        public void CalculateTwoMoveMateParallel()
        {
            ChessEngine chessEngine = new ChessEngine();
            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, 1));
            chessEngine.SetPiece(new Pawn(Color.White, "C4"));
            chessEngine.SetPiece(new King(Color.Black, "H8", MoveType.Normal, 1));
            chessEngine.SetPiece(new Pawn(Color.Black, "B5"));
            chessEngine.SetPiece(new Rook(Color.White, "G5",1));


            Move move = chessEngine.CalculateMoveWithDepthParallel(6);

            Assert.AreEqual(Evaluation.BlackCheckMate, move.Rating.Evaluation);
            Assert.AreEqual(Situation.WhiteVictory, move.Rating.Situation);
            Assert.IsTrue(move.Piece is Rook);

        }

        [TestMethod]
        public void CalculateTwoMoveMateWithDepth8()
        {
            ChessEngine chessEngine = new ChessEngine();

            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, 1));
            chessEngine.SetPiece(new Pawn(Color.White, "C4"));
            chessEngine.SetPiece(new King(Color.Black, "H8", MoveType.Normal, 1));
            chessEngine.SetPiece(new Pawn(Color.Black, "B5"));
            chessEngine.SetPiece(new Rook(Color.White, "G5", 1));


            Move move = chessEngine.CalculateMoveWithDepth(8);

            Assert.AreEqual(Evaluation.BlackCheckMate, move.Rating.Evaluation);
            Assert.AreEqual(Situation.WhiteVictory, move.Rating.Situation);
            Assert.IsTrue(move.Piece is Rook);
        }

        [TestMethod]
        public void CalculateTwoMoveMateWithDepth8Parallel()
        {
            ChessEngine chessEngine = new ChessEngine();

            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, 1));
            chessEngine.SetPiece(new Pawn(Color.White, "C4"));
            chessEngine.SetPiece(new King(Color.Black, "H8", MoveType.Normal, 1));
            chessEngine.SetPiece(new Pawn(Color.Black, "B5"));
            chessEngine.SetPiece(new Rook(Color.White, "G5", 1));


            Move move = chessEngine.CalculateMoveWithDepthParallel(8);

            Assert.AreEqual(Evaluation.BlackCheckMate, move.Rating.Evaluation);
            Assert.AreEqual(Situation.WhiteVictory, move.Rating.Situation);
            Assert.IsTrue(move.Piece is Rook);
        }

        [TestMethod]
        public void CheckEnpassant()
        {
            ChessEngine chessEngine = new ChessEngine();

            chessEngine.SetPiece(new King(Color.White, "G6", MoveType.Normal, 1));
            chessEngine.SetPiece(new Pawn(Color.White, "B2"));
            chessEngine.SetPiece(new King(Color.Black, "H8", MoveType.Normal, 1));
            chessEngine.SetPiece(new Pawn(Color.Black, "C4"));

            chessEngine.ColorToMove = Color.White;

            Board b1 = chessEngine.Board.Copy();

            var moveListWhite = chessEngine.GetMoveList();
            var doubleStepMove = moveListWhite.Moves.First(move => move.Type == MoveType.PawnDoubleStep);
            chessEngine.ExecuteMove(doubleStepMove);

            Board b2 = chessEngine.Board.Copy();

            var moveListBlack = chessEngine.GetMoveList();

            Move move = chessEngine.CalculateMoveWithDepth(1);
            
            Assert.AreEqual(MoveType.EnpassantBlackLowRow, move.Type);
            chessEngine.ExecuteMove(move);
            chessEngine.Board.UndoLastMove();

            Assert.IsTrue(b2.Compare(chessEngine.Board));

            chessEngine.Board.UndoLastMove();
            Assert.IsTrue(b1.Compare(chessEngine.Board));

        }

        [TestMethod]
        public void CalculatePawnBeat()
        {
            ChessEngine chessEngine = new ChessEngine();

            chessEngine.SetPiece(new King(Color.White, "H1", MoveType.Normal, 1));
            chessEngine.SetPiece(new Pawn(Color.White, "E4"));
            chessEngine.SetPiece(new King(Color.Black, "G8", MoveType.Normal, 1));
            chessEngine.SetPiece(new Pawn(Color.Black, "D5"));

            Move move = chessEngine.CalculateMoveWithDepth(4);
            Assert.IsTrue(move.End.AreEqual(new Position("D5")));
        }

        #endregion
    }
}
