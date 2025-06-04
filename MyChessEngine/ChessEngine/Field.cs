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

        public Field(Piece piece)
        {
            Piece = piece;
        }

        public Field()
        {
            Piece = null;
        }
    }
}
