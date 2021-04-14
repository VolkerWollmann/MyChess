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

        public bool IsValidPosition()
        {
            return (Row >= 0) && (Row < ChessConstants.Length) && (Column >= 0) && (Column < ChessConstants.Length);
        }

        public bool AreEqual(Position position)
        {
            return this.Row == position.Row && this.Column == position.Column;
        }

        public Position GetDeltaPosition(int deltaRow, int deltaColumn)
        {
            Position position = new Position(this.Row + deltaRow, this.Column + deltaColumn);
            if (!position.IsValidPosition())
                return null;

            return position;
        }
    }
}
