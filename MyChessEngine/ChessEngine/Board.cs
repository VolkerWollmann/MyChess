using MyChessEngine.Pieces;
using MyChessEngineBase;
using MyChessEngineBase.Rating;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;


namespace MyChessEngine
{
    public class Board
    {
        private readonly Field[,] Field;
        

        private List<Move> Moves = new List<Move>();

        public static int Counter;

        public int Ply = 0;

        public Board()
        {
            Field = new Field[8, 8];
            for(int row = 0; row < ChessEngineConstants.Length; row++)
            {
                for (int column = 0; column < ChessEngineConstants.Length; column++)
                {
                    Field[column, row] = new Field(ChessEngineConstants.FieldNames[column,row]);
                }
            }
        }

        public Field this[int column, int row]
        {
            get => Field[column, row];
        }


        public Field this[Position position]
        {
            get => Field[position.Column, position.Row];
        }
        public Field this[string positionString]
        {
            get
            {
                Position position = new Position(positionString);
                return this[position];
            }
        }

        public void SetPiece(Position position,Piece? piece)
        {
            this[position].Piece = piece;
            if (piece == null) return;

            piece.Position = position;
            piece.Board = this;
            if (piece is King king)
                Kings[king.Color] = king;
        }

        public void SetPiece(Piece piece)
        {
            this[piece.Position].Piece = piece;
            piece.Board = this;
            if (piece is King king)
                Kings[king.Color] = king;

        }

        public void SetPiece(string positionString, Piece piece)
        {
            Position position = new Position(positionString);
            SetPiece(position, piece);
        }

        public IsValidPositionReturns IsValidPosition(Position position, Color color, bool threat = false)
        {
            if (position == null)
                return IsValidPositionReturns.NoPosition;

            Piece piece = this[position].Piece;
            if (piece == null)
                return IsValidPositionReturns.EmptyField;

            if (piece.Color != color)
            {
                if (threat && piece is King)
                    return IsValidPositionReturns.EnemyKingThreatPosition;
                return IsValidPositionReturns.EnemyBeatPosition;
            }

            // do not beat own pieces
            return IsValidPositionReturns.NoPosition;
        }

        private Dictionary<Color, List<Piece>> _AllPiecesByColor = new Dictionary<Color, List<Piece>>();

        public void ClearAllPieces()
        {
            _AllPiecesByColor = new Dictionary<Color, List<Piece>>();
        }
        public List<Piece> GetAllPieces(Color color)
        {
            var pieces = Field.Cast<Field>().Where(field => (field.Piece?.Color == color)).Select(field => field.Piece).ToList();

            return pieces;
        }


        public void ClearOptimizationVariables()
        {
            _AllPiecesByColor = new Dictionary<Color, List<Piece>>();
            _AllMovesByColor = new Dictionary<Color, MoveList>();
        }

        public void Clear()
        {
            Position.AllPositions().ForEach(position => { this[position].Piece = null; this[position].Threat = false; });
        }


        public virtual Board Copy()
        {
            Board copy = new Board();

            copy.Ply = Ply;
            
            foreach (var field in Field)
            {
                Piece piece = field.Piece;
                if (field.Piece != null)
                {
                    Piece pc = piece.Copy();
                    copy.SetPiece(piece.Position, pc);
                }
            }
            
            return copy;
        }

        public virtual bool ExecuteMove(Move move)
        {
            if (this[move.Start] == null)
                throw new Exception("Move not Existing piece.");

            if (!move.End.IsValidPosition())
                throw new Exception("Move to invalid position.");

            Ply++;
            Moves.Add(move);

            
            this[move.End].Piece = this[move.Start].Piece;
            this[move.Start].Piece = null;
            this[move.End].Piece.Position = move.End;

            for (int i = 0; i < 2; i++)
            {
                var pos = move.AffectedPositionAfter[i];
                if (pos != null)
                {
                    if (move.AffectedPieceAfter[i] != null)
                        SetPiece(pos, (Piece)move.AffectedPieceAfter[i]);
                    else
                        this[pos].Piece = null;
                }
            }

            return true;
        }

        public virtual bool UndoLastMove()
        {
            var list = Moves;
            if (list == null || list.Count == 0)
                return false;

            // get last element (C# index-from-end)
            var move = list[^1];

            // remove last element
            list.RemoveAt(list.Count - 1);

            this[move.Start].Piece = this[move.End].Piece;
            this[move.End].Piece = null;
            this[move.Start].Piece.Position = move.Start;

            for (int i = 0; i < 2; i++)
            {
                Position pos = move.AffectedPositionBefore[i];
                if (pos != null)
                {
                    if (move.AffectedPieceBefore[i] != null)
                        SetPiece(pos, (Piece)move.AffectedPieceBefore[i]);
                    else
                        this[pos].Piece = null;
                }
            }

            Ply--;
            return true;
        }

        private Dictionary<Color, MoveList> _AllMovesByColor = new Dictionary<Color, MoveList>();

        public MoveList GetMoveList(Color color)
        {
	        return new MoveList(GetBaseMoveList(color));
		}

        internal List<Move> GetBaseMoveList(Color color)
        {
            return GetAllPieces(color).Select((piece => piece.GetMoveList().Moves)).SelectMany(move => move).ToList();
        }

        public Dictionary<Color, King> Kings = new Dictionary<Color, King> {{Color.White, null}, {Color.Black, null}};

        internal bool IsChecked(Color color)
        {
            return Kings[color]?.IsChecked() ?? true;
        }

        public virtual BoardRating GetRating(Color color)
        {
            Counter++;

            BoardRating rating = new BoardRating();

            if (Kings[color] == null)
            {
                rating.Situation = color == Color.White ? Situation.BlackVictory : Situation.WhiteVictory;
                rating.Weight = (color == Color.White) ? -ChessEngineConstants.CheckMate : ChessEngineConstants.CheckMate;
                return rating;
            }

            Color opponentColor = ChessEngineConstants.NextColorToMove(color);
            if (Kings[opponentColor] == null)
            {
                rating.Situation = opponentColor == Color.White ? Situation.BlackVictory : Situation.WhiteVictory;
                rating.Weight = (opponentColor == Color.White) ? -ChessEngineConstants.CheckMate : ChessEngineConstants.CheckMate;
                return rating;
            }

            int boardWeight = 0;

            foreach (var field in Field)
            {
                var piece = field.Piece;
                if (piece == null)
                    continue;
                boardWeight += piece.Weight;
            }
            
            rating.Weight = boardWeight;
            rating.Evaluation = Evaluation.Normal;

            rating.Situation = Situation.Normal;
            if (this[Kings[color].Position].Threat)
            {
                rating.Situation = color == Color.White ? Situation.WhiteChecked : Situation.BlackChecked;
            }

            return rating;
        }

        private void MarkThreatenedFields(Color color)
        {
            foreach (var field in Field)
                field.Threat = false;

            var pieces = GetAllPieces(color);
            List<Move> listOfMoves = new List<Move>();
            foreach (Piece piece in pieces)
            {
	            var moves = piece.GetThreatenMoveList().Moves;
	            foreach (var move in moves)
	            {
                    this[move.End].Threat = true;
	            }
            }
            
            //return GetAllPieces(color)
            //    .Select((piece => piece.GetThreatenMoveList().Moves))
            //    .SelectMany(move => move).Select(move => move.End).ToList();
        }
        public virtual Move CalculateMove(int depth, Color color)
        {
            MarkThreatenedFields(ChessEngineConstants.NextColorToMove(color));

            var moves = new MoveList(GetBaseMoveList(color));

            var rating = GetRating(color);
            
            if ( depth == 0 || rating.Situation == Situation.WhiteVictory || rating.Situation == Situation.BlackVictory )    
                return Move.CreateNoMove(rating);
            

            MoveList result = new MoveList();
            IBoardRatingComparer comparer = BoardRatingComparerFactory.GetComparer(color);

            foreach (Move move in moves.Moves)
            {
                ExecuteMove(move);

                Move resultMove = CalculateMove(depth - 1, ChessEngineConstants.NextColorToMove(color));
                move.Rating = resultMove.Rating;
                move.Rating.Depth = move.Rating.Depth + 1;
                
                result.Add(move);
                UndoLastMove();
            }

            var king = this.Kings[color];
            bool check = this[king.Position].Threat;
            Move resultMove2 = result.GetBestMove(color, check);
            return resultMove2;
        }

        public virtual Move CalculateMoveParallel(int depth, Color color)
        {
            MarkThreatenedFields(ChessEngineConstants.NextColorToMove(color));

            var moves = this.GetMoveList(color);

            var rating = GetRating(color);

            if (depth == 0 || rating.Situation == Situation.WhiteVictory || rating.Situation == Situation.BlackVictory)
                return Move.CreateNoMove(rating);


            ParallelMoveList result = new ParallelMoveList();
            IBoardRatingComparer comparer = BoardRatingComparerFactory.GetComparer(color);

            var moveIndexes = Enumerable.Range(0, moves.Moves.Count);
            Parallel.ForEach(moveIndexes, moveindex =>
            {
                Move move = moves.Moves[moveindex];
             
                //MoveStack[depth] = move.ToString();
                Board copy2 = this.Copy();
                copy2.ExecuteMove(move);

                Move resultMove = copy2.CalculateMove(depth - 1, ChessEngineConstants.NextColorToMove(color));
                move.Rating = resultMove.Rating;
                move.Rating.Depth = move.Rating.Depth + 1;

                result.Add(move);
            });

            var king = this.Kings[color];
            bool check = this[king.Position].Threat;
            Move resultMove2 = result.GetBestMove(color, check);
            return resultMove2;
        }
    }
}
