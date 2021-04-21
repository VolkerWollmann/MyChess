using MyChessEngine.Pieces;

namespace MyChessEngine
{
    public interface IEnginePiece : IPiece
    {
        MoveList GetMoveList();

        Board Board { get; set; }

        Position Position { get; set; }

        public bool ExecuteMove(Move move);

        Piece Copy();

        int Weight { get; }

    }
}
