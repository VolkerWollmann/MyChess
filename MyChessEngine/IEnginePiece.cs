using System.Collections.Generic;
using MyChessEngine.Pieces;

namespace MyChessEngine
{
    public interface IEnginePiece : IPiece
    {
        List<Move> GetMoves();

        Board Board { get; set; }

        Position Position { get; set; }

        public bool ExecuteMove(Move move);

        Piece Copy();

    }
}
