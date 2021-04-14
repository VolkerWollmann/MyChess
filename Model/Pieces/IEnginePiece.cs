using System;
using System.Collections.Generic;
using System.Text;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    public interface IEnginePiece : IPiece
    {
        List<Move> GetMoves();

        Board Board { get; set; }

        Position Position { get; set; }

    }
}
