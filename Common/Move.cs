using System;
using System.Collections.Generic;
using System.Text;

namespace MyChess.Common
{
    public class Move
    {
        public Position Start;
        public Position End;

        public Move()
        {
            Start = new Position();
            End = new Position();
        }

        public Move(Position start, Position end)
        {
            Start = start;
            End = end;
        }

    }
}
