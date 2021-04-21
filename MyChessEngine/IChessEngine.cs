using System.Runtime.CompilerServices;
using MyChessEngine.Pieces;

namespace MyChessEngine
{ 
    public interface IChessEngine
    {
        IPiece GetPiece(Position position);

        Color ColorToMove { get; set; }

        void New();

        void Clear();

        Piece this[string position]
        {
            get;
            set;
        }



        BoardRating GetRating(Color color);
        void Test();

        MoveList GetMoveList();

        BoardRating GetBoardRating();

        public bool ExecuteMove(Move move);

        Move CalculateMove(); 

        public string Message { get; }

    }
}
