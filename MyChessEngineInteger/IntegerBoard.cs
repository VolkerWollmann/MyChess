using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using MyChessEngineBase;
using MyChessEngineInteger.Pieces;

namespace MyChessEngineInteger
{
    public enum ChessEngineIntegerFlags
    {
        WhiteKingCastle=0,
        WhiteKingLongCastle=1,
        BlackKingCastle = 2,
        BlackKingLongCastle = 3,
        WhiteKingRow=4,
        WhiteKingColumn=5,
        BlackKingRow = 6,
        BlackKingColumn = 7,
        WhiteA1Rook=8,
        WhiteH1Rook=9,
        BlackA8Rook = 10,
        BlackH8Rook = 11,
        WhiteFirstEnpassantPawnRow=12,
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
            get => (NumPieces)Pieces[row, column];
            set => Pieces[row, column] = (int)value;
        }

        public void SetFlag(ChessEngineIntegerFlags flag, int value)
        {
            Data[(int)flag] = value;
        }

        public void Clear()
        {
            for (int row = 0; row < ChessEngineConstants.Length; row++)
            {
                for (int column = 0; column < ChessEngineConstants.Length; column++)
                {
                    Pieces[row, column] = 0;
                }
            }

            for (int i = 0; i < 20; i++)
            {
                Data[i] = 0;
            }
        }


    }
}
