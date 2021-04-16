using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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

            IComparer<Move> comparer = new MoveSorter(color);

            if ((bestMove == null ) || (_Moves.Count <= 1))
                return bestMove;

            for (int i = 1; i < _Moves.Count; i++)
            {
                if (comparer.Compare(bestMove,_Moves[i]) < 0)
                    bestMove = _Moves[i];
            }

            return bestMove;
        }

        public void BubbleSort()
        {
            ;
        }


        public void Sort(Color color)
        {
            _Moves.Sort(new MoveSorter(color));
        }
    }

}
