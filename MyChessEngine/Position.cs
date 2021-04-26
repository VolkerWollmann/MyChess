using System.Collections.Generic;
using System.Data;
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
            Row = bytes[1] - 49;
            Column = bytes[0] - 65;

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
            int newRow = Row + deltaRow;
            if ((newRow < 0) || (newRow >= ChessEngineConstants.Length))
                return null;

            int newColumn = Column + deltaColumn;
            if ((newColumn < 0) || (newColumn >= ChessEngineConstants.Length))
                return null;

            return new Position(newRow, newColumn);
        }
    }
}
