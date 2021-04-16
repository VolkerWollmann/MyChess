using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MyChessEngine
{
    public class MoveList
    {
        private List<Move> _Moves = new List<Move>();

        public List<Move> Moves
        {
            get
            {
                return _Moves;
            }
        }

        public void Add(Move move)
        {
            _Moves.Add(move);
        }

        public Move GetBestMove(Color color)
        {
            Move bestMove = _Moves.FirstOrDefault();
            if (_Moves.Count <= 1)
                return bestMove;

            for (int i = 1; i < _Moves.Count; i++)
            {
                if (color == Color.White)
                {

                }
            }

            return bestMove;
        }
    }
}
