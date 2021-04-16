using System.Collections.Generic;
using MyChess.Common;
using MyChessEngineCommon;

namespace MyChess.Model.Pieces
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
