using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace MyTranspositionChessEngine
{
    [DebuggerDisplay("{ToString()}")]
    public struct Position
    {

        public int Column=-1; // A-H
        public int Row=-1;    // 1-8

        public Position()
        {
            Column = -1;
            Row = -1;
        }
        public Position(int column, int row)
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

        public Position GetDeltaPosition(int deltaColumn, int deltaRow)
        {
            return new Position(Column + deltaColumn, Row + deltaRow);
        }

        public Position GetDeltaColumnPosition(int deltaColumn)
        {
            return new Position(Column + deltaColumn, Row);
        }

        public Position GetDeltaRowPosition(int deltaRow)
        {
            return new Position(Column, Row + deltaRow);
        }
	}
}
