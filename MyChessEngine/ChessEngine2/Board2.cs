using System.Linq;
using MyChessEngineBase;
using MyChessEngineBase.Rating;
using MyChessEngine.Pieces;

namespace MyChessEngine
{
    public class Board2 : Board
    {
        public new Board2 Copy()
        {
            Board2 copy = new Board2();

            for (int column = 0; column < ChessEngineConstants.Length; column++)
            for (int row = 0; row < ChessEngineConstants.Length; row++)
            {
                Piece piece = this[column, row].Piece;
                if (piece != null)
                {
                        copy.SetPiece(piece.Position, piece);
                }
            }

            return copy;
        }

        
        public BoardRating GetRating(Color color, bool isChecked, bool moves)
        {
            Counter++;

            BoardRating rating = new BoardRating();

            if (Kings[color] == null)
            {
                rating.Situation = color == Color.White ? Situation.BlackVictory : Situation.WhiteVictory;
                rating.Weight = (color == Color.White) ? ChessEngineConstants.King : -ChessEngineConstants.King;
                return rating;
            }

            if (isChecked)
            {
                rating.Situation = color == Color.White ? Situation.WhiteChecked : Situation.BlackChecked;
                if (!moves)
                {
                    rating.Situation = color == Color.White ? Situation.BlackVictory : Situation.WhiteVictory;
                    rating.Evaluation = color == Color.White ? Evaluation.WhiteCheckMate : Evaluation.BlackCheckMate;
                    return rating;
                }
            }
            else
            {
                if (!moves)
                {
                    rating.Situation = Situation.StaleMate;
                    rating.Evaluation = color == Color.White ? Evaluation.WhiteStaleMate : Evaluation.BlackStaleMate;
                    return rating;
                }
            }

            int boardWeight = 0;

            GetAllPieces(Color.White).ForEach(piece => { boardWeight += piece.Weight; });
            GetAllPieces(Color.Black).ForEach(piece => { boardWeight += piece.Weight; });

            rating.Weight = boardWeight;
            return rating;
        }

        public override Move CalculateMove(int depth, Color color)
        {
            if (Kings[color] == null)
                return Move.CreateNoMove(GetRating(color, true, false));

            var moves  = GetBaseMoveList(color);
            bool hasMoves = moves.Any();
            bool isChecked = IsChecked(color);

            int localDepth = depth - 1;
            if (localDepth == 1)
            {
                if (isChecked)
                    localDepth++;
                else
                    return Move.CreateNoMove(GetRating(color, false, hasMoves));
            }
            
            MoveList result = new MoveList();
            IBoardRatingComparer comparer = BoardRatingComparerFactory.GetComparer(color);

            Board2 copy = Copy();
            foreach (Move move in moves)
            {
                copy.ExecuteMove(move);
                if (!copy.IsChecked(color))
                {
                    Move resultMove = copy.CalculateMove(localDepth, ChessEngineConstants.NextColorToMove(color));
                    if ((move.Rating == null) ||
                        (comparer.Compare(move.Rating, resultMove.Rating) > 0))
                    {
                        move.Rating = resultMove.Rating;
                        move.Rating.Depth = + 1;
                    }

                    result.Add(move);
                }

                copy.UndoLastMove() ;
            }

            
            if (!result.Moves.Any())
                return Move.CreateNoMove(GetRating(color, isChecked, false));

            return result.GetBestMove(color, isChecked);
        }
    }
}
