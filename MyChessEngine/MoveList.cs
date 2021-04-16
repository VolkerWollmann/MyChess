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

            if ((bestMove == null ) || (_Moves.Count <= 1))
                return bestMove;

            for (int i = 1; i < _Moves.Count; i++)
            {
                if (bestMove.Rating.PartialCompare(_Moves[i].Rating, color) < 0)
                {
                    bestMove = _Moves[i];
                }
            }

            return bestMove;
        }

        public void BubbleSort(Color color)
        {
            for (int i = 0; i < _Moves.Count; i++)
            {
                for (int j = i+1; j < _Moves.Count; j++)
                {
                    if (_Moves[i].Rating.PartialCompare(_Moves[j].Rating, color)<0)
                    {
                        Move m = _Moves[i];
                        _Moves[i] = Moves[j];
                        _Moves[j] = m;
                    }
                }
            }
        }
    }
}
