using System;
using System.Collections.Generic;
using System.Text;
using MyChessEngineBase;

namespace MyChessEngineInteger.Pieces
{
    public class King
    {
        static readonly int[,] Delta = {
            { -1, -1 }, { -1, 0 }, {  -1, +1 },
            {  0, -1 },            {   0, +1 },
            { +1, -1 }, { +1, 0 }, {  +1, +1 }
        };

        public static void AddMovesToMoveList(Board board, int row, int column, Color color, MoveList moveList )
        {
            for (int i = 0; i < 8; i++)
            {
                int actualRow = row + Delta[i, 0];
                if (actualRow >= 0 && actualRow < ChessEngineConstants.Length)
                {
                    int actualColumn = column + Delta[i, 1];
                    if (actualColumn >= 0 && actualColumn < ChessEngineConstants.Length)
                    {
                        if (color == Color.White)
                        {
                            if (board[actualRow, actualColumn] <= 0)
                            {
                                Move move = new Move(row, column, actualRow, actualColumn);
                                moveList.Add(move);
                            }
                        }
                        else
                        {
                            if (board[actualRow, actualColumn] <= 0)
                            {
                                Move move = new Move(row, column, actualRow, actualColumn);
                                moveList.Add(move);
                            }
                        }
                    }
                }
            }

            // ToDo : Add Rochades
        }
    }
}
