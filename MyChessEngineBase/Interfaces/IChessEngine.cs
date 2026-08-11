using MyChessEngineBase.Rating;

namespace MyChessEngineBase.Interfaces
{ 
    public interface IChessEngine
    {
        IPiece GetPiece(Position position);

        void SetPiece(Position position, IPiece piece);

        void SetPiece(string position, IPiece piece);

        Color ColorToMove { get; set; }

        void New();

        void Clear();

        BoardRating GetRating(Color color);
        void Test();


        BoardRating GetBoardRating();

        public bool ExecuteMove(Move move);

        Move CalculateMove();

        /// Like CalculateMove, but searches with the given depth (plies).
        Move CalculateMove(int depth);

        public string Message { get; }

    }
}
