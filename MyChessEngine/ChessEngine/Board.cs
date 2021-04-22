using System;
using System.Collections.Generic;
using System.Linq;
using MyChessEngine.Pieces;


namespace MyChessEngine
{
    public class Board
    {
        private readonly Piece[,] Pieces;

        public static int Counter;

        public Board()
        {
            Pieces = new Piece[8, 8];
        }

        public Piece this[int row, int column]
        {
            get => Pieces[row, column];
            set
            {
                Pieces[row, column] = value;
                if (value != null)
                {
                    Pieces[row, column].Board = this;
                    Pieces[row, column].Position = new Position(row, column);
                }
            }
        }


        public Piece this[Position position]
        {
            get => Pieces[position.Row, position.Column];
            set
            {
                Pieces[position.Row, position.Column] = value;
                if (value != null)
                {
                    Pieces[position.Row, position.Column].Board = this;
                    Pieces[position.Row, position.Column].Position = position;
                }
            }
        }

        public Piece this[string positionString]
        {
            get
            {
                Position position = new Position(positionString);
                return this[position];
            }

            set
            {
                Position position = new Position(positionString);
                this[position] = value;
            }
        }



        public IsValidPositionReturns IsValidPosition(Position position, Color color)
        {
            Piece piece = this[position];
            if (piece == null)
                return IsValidPositionReturns.EmptyField;

            if (piece.Color != color)
                return IsValidPositionReturns.EnemyBeatPosition;

            // do not beat own pieces
            return IsValidPositionReturns.NoPosition;
        }

        private Dictionary<Color, List<Piece>> _AllPiecesByColor = new Dictionary<Color, List<Piece>>();

        public List<Piece> GetAllPieces(Color color)
        {
            if (_AllPiecesByColor.ContainsKey(color))
                return _AllPiecesByColor[color];

            List<Piece> pieces = new List<Piece>();
            Position.AllPositions().ForEach(position =>
            {
                Piece piece = this[position];
                if ((piece != null) && piece.Color == color)
                    pieces.Add(piece);
            });

            _AllPiecesByColor.Add(color, pieces);

            return pieces;
        }


        public void ClearOptimizationVariables()
        {
            _AllPiecesByColor = new Dictionary<Color, List<Piece>>();
            _AllMovesByColor = new Dictionary<Color, MoveList>();
        }

        public void Clear()
        {
            Position.AllPositions().ForEach(position => { this[position] = null; });
        }


        public virtual Board Copy()
        {
            Board copy = new Board();

            for (int i = 0; i < ChessEngineConstants.Length; i++)
            for (int j = 0; j < ChessEngineConstants.Length; j++)
            {
                Piece piece = this[i, j];
                copy[i, j] = piece?.Copy();
            }

            return copy;
        }

        public bool ExecuteMove(Move move)
        {
            if (this[move.Start] == null)
                throw new Exception("Move not Existing piece.");

            if (!move.End.IsValidPosition())
                throw new Exception("Move to invalid position.");

            this[move.Start].ExecuteMove(move);

            return true;
        }

        private Dictionary<Color, MoveList> _AllMovesByColor = new Dictionary<Color, MoveList>();

        public MoveList GetMoveList(Color color)
        {
            if (_AllMovesByColor.ContainsKey(color))
                return _AllMovesByColor[color];

            List<Move> baseMoveList = GetBaseMoveList(color);

            MoveList moveList = new MoveList();

            foreach (Move move in baseMoveList)
            {
                Board testBoard = this.Copy();
                testBoard.ExecuteMove(move);
                if (!testBoard.IsChecked(color))
                    moveList.Add(move);
            }

            _AllMovesByColor[color] = moveList;

            return moveList;
        }

        internal List<Move> GetBaseMoveList(Color color)
        {
            return GetAllPieces(color).Select((piece => piece.GetMoveList().Moves)).SelectMany(move => move).ToList();
        }

        internal bool IsChecked(Color color)
        {
            King king = (King) GetAllPieces(color).FirstOrDefault(piece => piece.Type == PieceType.King);
            return king?.IsChecked() ?? true;
        }

        public virtual BoardRating GetRating(Color color)
        {
            Counter++;

            BoardRating rating = new BoardRating();

            King king = (King) GetAllPieces(color).FirstOrDefault(piece => piece.Type == PieceType.King);

            if (king == null)
            {
                rating.Situation = color == Color.White ? Situation.BlackVictory : Situation.WhiteVictory;
                rating.Evaluation = color == Color.White ? Evaluation.WhiteCheckMate : Evaluation.BlackCheckMate;
                rating.Weight = (color == Color.White) ? ChessEngineConstants.King : -ChessEngineConstants.King;
                return rating;
            }

            if (IsChecked(color))
            {
                rating.Situation = color == Color.White ? Situation.WhiteChecked : Situation.BlackChecked;
                if ((!GetMoveList(color).Moves.Any()))
                {
                    rating.Situation = color == Color.White ? Situation.BlackVictory : Situation.WhiteVictory;
                    rating.Evaluation = color == Color.White ? Evaluation.WhiteCheckMate : Evaluation.BlackCheckMate;
                    return rating;
                }
            }
            else
            {
                if ((!GetMoveList(color).Moves.Any()))
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

        public virtual Move CalculateMove(int depth, Color color)
        {
            King king = (King) GetAllPieces(color).FirstOrDefault(piece => piece.Type == PieceType.King);

            var moves = GetMoveList(color);
            if ((depth <= 1) || (king == null) || (!moves.Moves.Any()))
                return Move.CreateNoMove(GetRating(color));

            MoveList result = new MoveList();

            foreach (Move move in moves.Moves)
            {
                Board copy = this.Copy();
                copy.ExecuteMove(move);
                if (!copy.IsChecked(color))
                {
                    Move resultMove = copy.CalculateMove(depth - 1, ChessEngineConstants.NextColorToMove(color));
                    if ((move.Rating == null) ||
                        (new BoardRatingComparer(color).Compare(move.Rating, resultMove.Rating) > 0))
                    {
                        move.Rating = resultMove.Rating;
                        move.Rating.Depth++;
                    }
                    result.Add(move);
                }
            }

            return result.GetBestMove(color);
        }
    }
}
