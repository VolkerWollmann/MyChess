using MyChessEngine;
using MyChessEngine.Pieces;

namespace MyChessEngine
{ 
    public interface IChessEngine
    {
        IPiece GetPiece(Position position);

        void New();

        void Clear();

        void Test();

        public bool ExecuteMove(Move move);

        public string Message { get; }

    }
}
