using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyChessEngine.Rating;
using MyChessEngine.Pieces;

namespace MyChessEngine
{
    public class Board2 : Board
    {
        public new Board2 Copy()
        {
            Board2 copy = new Board2();

            for (int i = 0; i < ChessEngineConstants.Length; i++)
            for (int j = 0; j < ChessEngineConstants.Length; j++)
            {
                Piece piece = this[i, j];
                copy[i, j] = piece?.Copy();
            }

            foreach (Color color in ChessEngineConstants.BothColors)
            {
                copy.Kings[color] = (King)copy.GetAllPieces(color).FirstOrDefault(piece => piece.Type == PieceType.King);
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
                rating.Evaluation = color == Color.White ? Evaluation.WhiteCheckMate : Evaluation.BlackCheckMate;
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
            int localdepth = depth-1;

            if (Kings[color] == null)
                return Move.CreateNoMove(GetRating(color, true, false));

            var moves  = base.GetBaseMoveList(color);
            bool hasMoves = moves.Any();

            bool isChecked = this.IsChecked(color);
            if ((localdepth == 1) && isChecked)
                localdepth++;

            if ( (!hasMoves) || localdepth==1)
                    return Move.CreateNoMove(GetRating(color, isChecked, hasMoves));

            MoveList result = new MoveList();
            IBoardRatingComparer comparer = BoardRatingComparerFactory.GetComparer(color);

            foreach (Move move in moves)
            {
                Board copy = this.Copy();
                copy.ExecuteMove(move);
                if (!copy.IsChecked(color))
                {
                    Move resultMove = copy.CalculateMove(localdepth, ChessEngineConstants.NextColorToMove(color));
                    if ((move.Rating == null) ||
                        (comparer.Compare(move.Rating, resultMove.Rating) > 0))
                    {
                        move.Rating = resultMove.Rating;
                        move.Rating.Depth++;
                    }

                    result.Add(move);
                }
            }

            
            if (!result.Moves.Any())
                return Move.CreateNoMove(GetRating(color, isChecked, false));

            return result.GetBestMove(color);
        }
    }
}
