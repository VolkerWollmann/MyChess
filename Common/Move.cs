namespace MyChess.Common
{
    public class Move
    {
        public Position Start;
        public Position End;
        public IPiece Piece;
        
        public Move(Position start, Position end, IPiece piece)
        {
            Start = start;
            End = end;
            Piece = piece;


        }

    }
}
