using System.Collections.Generic;
using System.Linq;
using MyChessEngineBase;

namespace MyChessEngineInteger
{
    public class MoveList
    {
        private readonly List<Move> _Moves = new List<Move>();

        public List<Move> Moves => _Moves;

        public void Add(Move move)
        {
            _Moves.Add(move);
        }

        public Move GetBestMove(Color color)
        {
            Move bestMove = _Moves.FirstOrDefault();

            IComparer<Move> comparer = new MoveSorter(color);

            if ((bestMove == null) || (_Moves.Count <= 1))
                return bestMove;

            for (int i = 1; i < _Moves.Count; i++)
            {
                if (comparer.Compare(bestMove, _Moves[i]) > 0)
                    bestMove = _Moves[i];
            }

            return bestMove;
        }


        public void Sort(Color color)
        {
            _Moves.Sort(new MoveSorter(color));
        }
    }
}
