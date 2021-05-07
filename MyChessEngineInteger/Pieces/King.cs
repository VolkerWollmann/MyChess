using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyChessEngineBase;

namespace MyChessEngineInteger.Pieces
{
    public class King
    {
        public static void GetMoveList(Board board, int row, int column, Color color, MoveList moveList )
        {
            int[,] delta = {
                { -1, -1 }, { -1, 0 }, {  -1, +1 },
                {  0, -1 },            {   0, +1 },
                { +1, -1 }, { +1, 0 }, {  +1, +1 }
            };

            for (int i = 0; i < 8; i++)
            {
                int actualRow = row + delta[i, 0];
                if (actualRow >= 0 && actualRow < ChessEngineConstants.Length)
                {
                    int actualColumn = column + delta[i, 1];
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
                            if (board[actualRow, actualColumn] >= 0)
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

        public static bool IsChecked(Board board, Color color, MoveList movelist)
        {
            int kingRow, kingColumn;

            if (color == Color.White)
            {
                kingRow = board.Data[(int)ChessEngineIntegerFlags.WhiteKingRow];
                kingColumn = board.Data[(int)ChessEngineIntegerFlags.WhiteKingColumn];
            }
            else
            {
                kingRow = board.Data[(int)ChessEngineIntegerFlags.BlackKingRow];
                kingColumn = board.Data[(int)ChessEngineIntegerFlags.BlackKingColumn];
            }

            return movelist.Moves.Any(move => move.EndRow == kingRow && move.EndColumn == kingColumn);
        }

        public static void MoveWhiteKing(Board board, int endRow, int endColumn)
        {
            board.Data[(int)ChessEngineIntegerFlags.WhiteKingCastle] = 0;
            board.Data[(int)ChessEngineIntegerFlags.WhiteKingLongCastle] = 0;
            board.Data[(int)ChessEngineIntegerFlags.WhiteKingRow] = endRow;
            board.Data[(int)ChessEngineIntegerFlags.WhiteKingColumn] = endColumn;
        }

        public static void MoveBlackKing(Board board, int endRow, int endColumn)
        {
            board.Data[(int)ChessEngineIntegerFlags.BlackKingCastle] = 0;
            board.Data[(int)ChessEngineIntegerFlags.BlackKingLongCastle] = 0;
            board.Data[(int)ChessEngineIntegerFlags.BlackKingRow] = endRow;
            board.Data[(int)ChessEngineIntegerFlags.BlackKingColumn] = endColumn;
        }
    }
}
