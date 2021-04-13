namespace MyChess.Common
{
    public class Move
    {
        public Position Start;
        public Position End;

        public Move(Position start, Position end)
        {
            Start = start;
            End = end;
        }

    }
}
