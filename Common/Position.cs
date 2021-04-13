using System;
using System.Collections.Generic;
using System.Text;

namespace MyChess.Common
{
    public class Position
    {
        public int Row;
        public int Column;

        public Position()
        {

        }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public static List<Position> AllPositions()
        {
            List<Position> allPositions = new List<Position>();

            for (int row = 0; row < 8; row++)
            for (int column = 0; column < 8; column++)
            {
                allPositions.Add(new Position(row, column));
            }

            return allPositions;
        }
    }
}
