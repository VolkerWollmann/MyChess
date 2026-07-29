using MyChessEngine.Pieces;
using MyChessEngineBase;
using MyChessEngineBase.Rating;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (position.Column == -1)
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
            var pieces = new List<Piece>(16);
            for (int c = 0; c < ChessEngineConstants.Length; c++)
            for (int r = 0; r < ChessEngineConstants.Length; r++)
            {
                var piece = Field[c, r].Piece;
                if (piece != null && piece.Color == color)
                    pieces.Add(piece);
            }

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

        #region Copy
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

        public bool Compare(Board other)
        {
            for (int i = 0; i < ChessEngineConstants.Length; i++)
            for (int j = 0; j < ChessEngineConstants.Length; j++)
            {
                var piece = this[i, j].Piece;
                var otherPiece = other[i, j].Piece;
                if (piece == null && otherPiece == null)
                    continue;
                if (piece == null || otherPiece == null)
                    return false;
                if (!piece.Compare(otherPiece))
                    return false;
            }

            return true;
        }
    

        #endregion
        public virtual bool ExecuteMove(Move move)
        {
            if (this[move.Start] == null)
                throw new Exception("Move not Existing piece.");

            if (!move.End.IsValidPosition())
                throw new Exception("Move to invalid position.");

            Ply++;
            Moves.Add(move);

            if (this[move.End].Piece is King king)
	            Kings[king.Color] = null;

			Piece p = this[move.Start].Piece;
            move.PlyBefore = p.LastPly;
            this[move.End].Piece = p;
            move.PieceBefore = p.Copy();
            this[move.Start].Piece = null;
            p.Position = move.End;
            p.LastPly = Ply;

            for (int i = 0; i < 2; i++)
            {
                var pos = move.AffectedPositionAfter[i];
                if (pos.Column > -1)
                {
                    if (move.AffectedPieceAfter[i] != null)
                        SetPiece(pos, (Piece)move.AffectedPieceAfter[i]);
                    else
                        this[pos].Piece = null;
                }
            }

            p.ExecuteMove(move);

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

            Piece p = (Piece)move.PieceBefore;
            p.Board = this;
			this[move.Start].Piece = p;
            this[move.End].Piece = null;
            this[move.Start].Piece.Position = move.Start;
            //p.LastPly = move.PlyBefore;

            for (int i = 0; i < 2; i++)
            {
                Position pos = move.AffectedPositionBefore[i];
                if (pos.Column > -1)
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
            var list = new List<Move>(64);
            for (int c = 0; c < ChessEngineConstants.Length; c++)
            for (int r = 0; r < ChessEngineConstants.Length; r++)
            {
                var piece = Field[c, r].Piece;
                if (piece == null || piece.Color != color)
                    continue;
                var pieceMoves = piece.GetMoveList().Moves;
                for (int i = 0; i < pieceMoves.Count; i++)
                    list.Add(pieceMoves[i]);
            }
            return list;
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

            for (int c = 0; c < ChessEngineConstants.Length; c++)
            for (int r = 0; r < ChessEngineConstants.Length; r++)
            {
                var piece = Field[c, r].Piece;
                if (piece == null || piece.Color != color)
                    continue;
                var threatened = piece.GetThreatenMoveList().Moves;
                for (int i = 0; i < threatened.Count; i++)
                    this[threatened[i].End].Threat = true;
            }
        }
        public virtual Move CalculateMove(int depth, Color color)
        {
            return CalculateMove(depth, color, null, null);
        }

        /// Alpha-beta pruning: alpha is the best rating white can already force on
        /// this path, beta the best for black (both from white's point of view).
        /// Once alpha is at least beta the remaining moves are cut off.
        public virtual Move CalculateMove(int depth, Color color, BoardRating alpha, BoardRating beta)
        {
            MarkThreatenedFields(ChessEngineConstants.NextColorToMove(color));

            var rating = GetRating(color);

            if (rating.Situation == Situation.WhiteVictory || rating.Situation == Situation.BlackVictory)
                return Move.CreateNoMove(rating);

            if (depth == 0)
                return Move.CreateNoMove(rating);

            var moves = GetBaseMoveList(color);

            // Captures first (most valuable victim first, stable order) so cutoffs come early
            var searchOrder = moves
                .OrderByDescending(move => move.AffectedPieceBefore[0] is Piece victim ? Math.Abs(victim.Weight) : 0)
                .ToList();

            foreach (Move move in searchOrder)
            {
                ExecuteMove(move);

                Move resultMove = CalculateMove(depth - 1, ChessEngineConstants.NextColorToMove(color), alpha, beta);
                move.Rating = resultMove.Rating;
                move.Rating.Depth = move.Rating.Depth + 1;

                UndoLastMove();

                if (color == Color.White)
                {
                    if (alpha == null || alpha.Weight < move.Rating.Weight)
                        alpha = move.Rating;
                }
                else
                {
                    if (beta == null || beta.Weight > move.Rating.Weight)
                        beta = move.Rating;
                }

                // Cut only on strictly greater weight: lines rated equal in weight (e.g.
                // all the mate lines) stay fully evaluated, so their depth tie-breaking
                // and Situation/Evaluation stay exactly as in the search without pruning.
                if (alpha != null && beta != null && alpha.Weight > beta.Weight)
                    break; // the opponent avoids this line, the remaining moves cannot matter
            }

            // Rated moves in generation order, GetBestMove ties resolve like without pruning
            MoveList result = new MoveList();
            foreach (Move move in moves)
            {
                if (move.Rating != null)
                    result.Add(move);
            }

            var king = this.Kings[color];
            bool check = this[king.Position].Threat;
            Move resultMove2 = result.GetBestMove(color, check);
            resultMove2.Rating.AddMove(resultMove2.ShortString());
			return resultMove2;
        }

        public virtual Move CalculateMoveParallel(int depth, Color color)
        {
            MarkThreatenedFields(ChessEngineConstants.NextColorToMove(color));

            var rating = GetRating(color);

            if (rating.Situation == Situation.WhiteVictory || rating.Situation == Situation.BlackVictory)
                return Move.CreateNoMove(rating);

            if (depth == 0)
                return Move.CreateNoMove(rating);

            var moves = this.GetMoveList(color).Moves;

            // Every root move writes only its own Rating; the reduce below runs on the
            // list in generation order, so ties resolve deterministically like in the
            // sequential search (a ConcurrentBag would order by thread completion).
            Parallel.For(0, moves.Count, moveindex =>
            {
                Move move = moves[moveindex];

                Board copy2 = this.Copy();
                var copy2Moves = copy2.GetMoveList(color);
                var copy2Move = copy2Moves.GetMoveByPositions(move.Start, move.End);
                copy2.ExecuteMove(copy2Move);

                Move resultMove = copy2.CalculateMove(depth - 1, ChessEngineConstants.NextColorToMove(color));
                move.Rating = resultMove.Rating;
                move.Rating.Depth = move.Rating.Depth + 1;
            });

            MoveList result = new MoveList(moves.Where(move => move.Rating != null).ToList());

            var king = this.Kings[color];
            bool check = this[king.Position].Threat;
            Move resultMove2 = result.GetBestMove(color, check);
            resultMove2.Rating.AddMove(resultMove2.ShortString());
            return resultMove2;
        }
    }
}
