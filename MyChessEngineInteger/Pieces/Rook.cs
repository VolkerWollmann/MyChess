using System;
using System.Collections.Generic;
using System.Text;
using MyChessEngineBase;

namespace MyChessEngineInteger.Pieces
{
    public class Rook
    {
        public static void AddMovesToMoveList(Board board, int row, int column, Color color, MoveList moveList)
        {
            for (int actualRow = row - 1; actualRow >= 0; actualRow--)
            {
                if (color == Color.White)
                {
                    if (board[actualRow, column] <= 0)
                    {
                        Move move = new Move(row, column, actualRow, column);
                        moveList.Add(move);
                    }
                }
                else
                {
                    if (board[actualRow, column] >= 0)
                    {
                        Move move = new Move(row, column, actualRow, column);
                        moveList.Add(move);
                    }
                }

                if (board[actualRow, column] != 0)
                    break;
            }

            for (int actualRow = row + 1; actualRow < ChessEngineConstants.Length; actualRow++)
            {
                if (color == Color.White)
                {
                    if (board[actualRow, column] <= 0)
                    {
                        Move move = new Move(row, column, actualRow, column);
                        moveList.Add(move);
                    }
                }
                else
                {
                    if (board[actualRow, column] <= 0)
                    {
                        Move move = new Move(row, column, actualRow, column);
                        moveList.Add(move);
                    }
                }

                if (board[actualRow, column] != 0)
                    break;
            }

            for (int actualColumn = column - 1; actualColumn >= 0; actualColumn--)
            {
                if (color == Color.White)
                {
                    if (board[row, actualColumn] <= 0)
                    {
                        Move move = new Move(row, column, row, actualColumn);
                        moveList.Add(move);
                    }
                }
                else
                {
                    if (board[row, actualColumn] >= 0)
                    {
                        Move move = new Move(row, column, row, actualColumn);
                        moveList.Add(move);
                    }
                }

                if (board[row, actualColumn] != 0)
                    break;
            }

            for (int actualColumn = column + 1; actualColumn < ChessEngineConstants.Length; actualColumn++)
            {
                if (color == Color.White)
                {
                    if (board[row, actualColumn] <= 0)
                    {
                        Move move = new Move(row, column, row, actualColumn);
                        moveList.Add(move);
                    }
                }
                else
                {
                    if (board[row, actualColumn] >= 0)
                    {
                        Move move = new Move(row, column, row, actualColumn);
                        moveList.Add(move);
                    }
                }

                if (board[row, actualColumn] != 0)
                    break;
            }
        }
    }
}
