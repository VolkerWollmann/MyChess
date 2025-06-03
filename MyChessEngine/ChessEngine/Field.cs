using MyChessEngine.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyChessEngine
{
    public class Field
    {
        public Piece Piece;
        public bool Threat = false;

        Field(Piece piece)
        {
            Piece = piece;
        }

        Field()
        {
            Piece = null;
        }
    }
}
