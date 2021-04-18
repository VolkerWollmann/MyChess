namespace MyChessEngine
{ 
    public interface IChessEngine
    {
        IPiece GetPiece(Position position);

        Color ColorToMove { get; set; }

        void New();

        void Clear();

        void Test();

        MoveList GetMoveList();

        BoardRating GetBoardRating();

        public bool ExecuteMove(Move move);

        Move CalculateMove(); 

        public string Message { get; }

    }
}
