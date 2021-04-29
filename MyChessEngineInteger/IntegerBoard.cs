using System.Linq;
using MyChessEngineBase;
using MyChessEngineBase.Rating;
using MyChessEngineInteger.Pieces;

namespace MyChessEngineInteger
{
    public enum ChessEngineIntegerFlags
    {
        WhiteKingCastle = 0,
        WhiteKingLongCastle = 1,
        BlackKingCastle = 2,
        BlackKingLongCastle = 3,
        WhiteKingRow = 4,
        WhiteKingColumn = 5,
        BlackKingRow = 6,
        BlackKingColumn = 7,
        WhiteA1Rook = 8,
        WhiteH1Rook = 9,
        BlackA8Rook = 10,
        BlackH8Rook = 11,
        WhiteFirstEnpassantPawnRow = 12,
        WhiteFirstEnpassantPawnColumn = 13,
        WhiteSecondEnpassantPawnRow = 14,
        WhiteSecondEnpassantPawnColumn = 15,
        BlackFirstEnpassantPawnRow = 16,
        BlackFirstEnpassantPawnColumn = 17,
        BlackSecondEnpassantPawnRow = 18,
        BlackSecondEnpassantPawnColumn = 19,
    }

    public class IntegerBoard
    {
        private int[,] Pieces = new int[8, 8];

        private int[] Data = new int[20];

        public NumPieces this[int row, int column]
        {
            get => (NumPieces) Pieces[row, column];
            set => Pieces[row, column] = (int) value;
        }

        public void SetFlag(ChessEngineIntegerFlags flag, int value)
        {
            Data[(int) flag] = value;
        }

        public void Clear()
        {
            for (int row = 0; row < ChessEngineConstants.Length; row++)
            {
                for (int column = 0; column < ChessEngineConstants.Length; column++)
                {
                    Pieces[row, column] = (int) NumPieces.Empty;
                }
            }

            for (int i = 0; i < 20; i++)
            {
                Data[i] = 0;
            }
        }


        private bool KingAlive(Color color)
        {
            if (color == Color.White) 
                return (Data[(int)ChessEngineIntegerFlags.WhiteKingRow] >= 0);

            return (Data[(int) ChessEngineIntegerFlags.BlackKingRow] >= 0);
        }

        public virtual BoardRating GetRating(Color color)
        {
            BoardRating rating = new BoardRating();

            if ((color == Color.White) && (Data[(int) ChessEngineIntegerFlags.WhiteKingRow] < 0))
            {
                rating.Situation = Situation.BlackVictory;
                rating.Evaluation = Evaluation.WhiteCheckMate;
                rating.Weight = -ChessEngineConstants.King;
                return rating;
            }
            else if ((color == Color.Black) && (Data[(int) ChessEngineIntegerFlags.BlackKingRow] < 0))
            {
                rating.Situation = Situation.WhiteVictory;
                rating.Evaluation = Evaluation.BlackCheckMate;
                rating.Weight = -ChessEngineConstants.King;
                return rating;
            }

            rating.Situation = Situation.Normal;
            rating.Weight = Pieces.Cast<int>().ToList().Sum();

            return rating;
        }
    }
}