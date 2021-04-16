using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MyChessEngine
{
    [DebuggerDisplay("{ToString()}")]
    public class Position 
    {
        public int Row;
        public int Column;

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="positionString"> A1 : Column=A(0) Row=1(0)</param>
        public Position (string positionString)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(positionString);
            Row = (int)(bytes[1] - 49);
            Column = (int)(bytes[0] - 65);

        }

        public override string ToString()
        {
            return (char) (Column + 65) + (Row + 1).ToString();
        }


        private static List<Position> _allPositions;
        public static List<Position> AllPositions()
        {
            if (_allPositions == null)
            {
                _allPositions = new List<Position>();
                for (int row = 0; row < ChessEngineConstants.Length; row++)
                for (int column = 0; column < ChessEngineConstants.Length; column++)
                {
                    _allPositions.Add(new Position(row, column));
                }
            }

            return _allPositions;
        }

        public bool IsValidPosition()
        {
            return (Row >= 0) && (Row < ChessEngineConstants.Length) && (Column >= 0) && (Column < ChessEngineConstants.Length);
        }

        public bool AreEqual(Position position)
        {
            return this.Row == position.Row && this.Column == position.Column;
        }

        public Position GetDeltaPosition(int deltaRow, int deltaColumn)
        {
            if (deltaRow == 0 && deltaColumn == 0)
                return null;

            Position position = new Position(this.Row + deltaRow, this.Column + deltaColumn);
            if (!position.IsValidPosition())
                return null;

            return position;
        }
    }
}
