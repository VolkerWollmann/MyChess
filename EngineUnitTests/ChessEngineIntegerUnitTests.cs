using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine.Pieces;
using MyChessEngineBase;
using MyChessEngineBase.Rating;
using MCEI = MyChessEngineInteger;

namespace EngineUnitTests
{
    [TestClass]
    public class ChessEngineIntegerUnitTests
    {
        [TestMethod]
        public void CheckBoardRatingBlackMate()
        {
            MCEI.ChessEngineInteger chessEngineInteger = new MCEI.ChessEngineInteger();
                chessEngineInteger.SetPiece("G6", MCEI.Pieces.NumPieces.WhiteKing);
                chessEngineInteger.SetPiece("A8", MCEI.Pieces.NumPieces.WhiteRook);
                chessEngineInteger.SetPiece("H8", MCEI.Pieces.NumPieces.BlackKing);

            BoardRating boardRating = chessEngineInteger.GetRating(Color.Black);
            Assert.AreEqual(Situation.WhiteVictory, boardRating.Situation);
        }

        [TestMethod]
        public void CheckBoardRatingBlackMateCalculateMove()
        {
            MCEI.ChessEngineInteger chessEngineInteger = new MCEI.ChessEngineInteger();

            chessEngineInteger.SetPiece("G6", MCEI.Pieces.NumPieces.WhiteKing);
            chessEngineInteger.SetPiece("A8", MCEI.Pieces.NumPieces.WhiteRook);
            chessEngineInteger.SetPiece("H8", MCEI.Pieces.NumPieces.BlackKing);
            chessEngineInteger.ColorToMove = Color.Black;
            
            MyChessEngineBase.Move move = chessEngineInteger.CalculateMove();
            Assert.IsNotNull(move);
        }

        [TestMethod]
        public void CalculateOneMoveMate1()
        {
            MCEI.ChessEngineInteger chessEngineInteger = new MCEI.ChessEngineInteger();

            chessEngineInteger.SetPiece("G6", MCEI.Pieces.NumPieces.WhiteKing);
            chessEngineInteger.SetPiece("H8", MCEI.Pieces.NumPieces.BlackKing);
            chessEngineInteger.SetPiece("A1", MCEI.Pieces.NumPieces.WhiteRook);

            MyChessEngineBase.Move move = chessEngineInteger.CalculateMove();

            Assert.IsTrue(move.End.AreEqual(new Position("A8")));

        }
    }
}
