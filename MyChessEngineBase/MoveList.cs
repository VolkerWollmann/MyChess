using System.Collections.Generic;
using System.Linq;
using MyChessEngineBase.Rating;

namespace MyChessEngineBase
{
    public class MoveList
    {
        private readonly List<Move> _Moves = new List<Move>();

        public List<Move> Moves => _Moves;

        public void Add(Move move)
        {
            _Moves.Add(move);
        }

        public MoveList(List<Move> moves)
        {
            _Moves = moves;
        }

        public MoveList()
        {
            _Moves = new List<Move>();
        }


        public Move GetBestMove(Color color, bool check)
        {
            
            if (color == Color.White)
            {
                bool atLeastOneMove = _Moves.Any();
                bool allMovesAreBlackVictory = _Moves.All(move => move.Rating.Situation == Situation.BlackVictory);
                if (!check && !atLeastOneMove)
                    return Move.CreateNoMove(new BoardRating {Situation = Situation.StaleMate, Evaluation = Evaluation.WhiteStaleMate});
                    
                if (check && allMovesAreBlackVictory)
                        return Move.CreateNoMove(new BoardRating { Situation = Situation.BlackVictory, Evaluation = Evaluation.WhiteCheckMate, Weight = -ChessEngineConstants.CheckMate });
            }
            else
            {
                bool atLeastOneMove = _Moves.Any();
                bool allMovesAreWhiteVictory = _Moves.All(move => move.Rating.Situation == Situation.WhiteVictory);
                if (!check && !atLeastOneMove)
                        return Move.CreateNoMove(new BoardRating { Situation = Situation.StaleMate, Evaluation = Evaluation.BlackStaleMate });

                if (check && allMovesAreWhiteVictory)
                        return Move.CreateNoMove(new BoardRating { Situation = Situation.WhiteVictory, Evaluation = Evaluation.BlackCheckMate, Weight = ChessEngineConstants.CheckMate });
            }
            
            Move bestMove = _Moves.FirstOrDefault();

            IComparer<Move> comparer = new MoveComparer(color);

            if ((bestMove == null) || (_Moves.Count <= 1))
                return bestMove;

            for (int i = 1; i < _Moves.Count; i++)
            {
                if (comparer.Compare(bestMove, _Moves[i]) < 0)
                    bestMove = _Moves[i];
            }

            return bestMove;
        }


        public void Sort(Color color)
        {
            _Moves.Sort(new MoveComparer(color));
        }
    }
}
