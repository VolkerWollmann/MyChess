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
        WhiteA1RookMoved = 8,
        WhiteH1RookMoved = 9,
        BlackA8RookMoved = 10,
        BlackH8RookMoved = 11,
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
        public int[,] Pieces = new int[8, 8];

        public int[] Data = new int[20];

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

        public IntegerBoard Copy()
        {
            IntegerBoard copy = new IntegerBoard
            {
                Pieces = (int[,]) this.Pieces.Clone(), Data = (int[]) this.Data.Clone()
            };

            return copy;
        }

        private bool ExecuteMove(int startRow, int startColumn, int endRow, int endColumn)
        {
            if (Pieces[endRow, endColumn] == (int) NumPieces.WhiteKing)
            {
                Data[(int) ChessEngineIntegerFlags.WhiteKingRow] = -1;
                Data[(int)ChessEngineIntegerFlags.WhiteKingColumn] = -1;
            }

            if (Pieces[endRow, endColumn] == (int)NumPieces.BlackKing)
            {
                Data[(int)ChessEngineIntegerFlags.BlackKingRow] = -1;
                Data[(int)ChessEngineIntegerFlags.BlackKingColumn] = -1;
            }



            return true;
        }
        public bool ExecuteMove(Move move)
        {
            return Piece.ExecuteMove(this, move);

        }

        private bool KingAlive(Color color)
        {
            if (color == Color.White) 
                return (Data[(int)ChessEngineIntegerFlags.WhiteKingRow] >= 0);

            return (Data[(int) ChessEngineIntegerFlags.BlackKingRow] >= 0);
        }

        private int GetIntegerRating()
        {
            return Pieces.Cast<int>().ToList().Sum();
        }

        public virtual BoardRating GetRating(Color color)
        {
            BoardRating rating = new BoardRating();

            if (!KingAlive(color))
            {
                rating.Situation = color == Color.White ? Situation.BlackVictory : Situation.WhiteVictory;
                rating.Evaluation = color == Color.White ? Evaluation.WhiteCheckMate : Evaluation.BlackCheckMate;
                rating.Weight = ((color == Color.White) ? -(int)NumPieces.WhiteKing : -(int)NumPieces.BlackKing);
                return rating;
            }

            rating.Situation = Situation.Normal;
            rating.Weight = GetIntegerRating();

            return rating;
        }
    }
}