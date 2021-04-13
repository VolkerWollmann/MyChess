using System.Collections.Generic;

namespace MyChess.Common
{
    public class Position
    {
        public int Row;
        public int Column;

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        private static List<Position> _allPositions;
        public static List<Position> AllPositions()
        {
            if (_allPositions == null)
            {
                _allPositions = new List<Position>();
                for (int row = 0; row < ChessConstants.Length; row++)
                for (int column = 0; column < ChessConstants.Length; column++)
                {
                    _allPositions.Add(new Position(row, column));
                }
            }

            return _allPositions;
        }
    }
}
