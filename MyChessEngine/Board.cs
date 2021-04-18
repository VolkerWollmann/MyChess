using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public bool IsValidPosition(Position position, Color color)
        {
            // does not work for pawn
            if (position == null)
                return false;

            if (!position.IsValidPosition())
                return false;

            if (this[position] == null)
                return true;

            if (this[position].Color == color)
                return false;

            return true;
        }

        private Dictionary<Color, List<Piece>> _AllPiecesByColor = new Dictionary<Color, List<Piece>>();

        public List<Piece> GetAllPieces(Color color)
        {
            if (_AllPiecesByColor.ContainsKey(color))
                return _AllPiecesByColor[color];

            List<Piece> pieces = new List<Piece>();
            Position.AllPositions().ForEach(position =>
            {
                if ((this[position] != null) && this[position].Color == color)
                    pieces.Add(this[position]);
            });

            _AllPiecesByColor.Add(color, pieces);

            return pieces;
        }

        public void Clear()
        {
            Position.AllPositions().ForEach(position => { this[position] = null; });
        }


        public Board Copy()
        {
            Board copy = new Board();

            Position.AllPositions().ForEach(position =>
            {
                copy[position] = null;
                if (this[position] != null)
                {
                    copy[position] = this[position].Copy();
                }
            });

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

        public MoveList GetMoveList(Color color)
        {
            List<Move> baseMoveList = GetBaseMoveList(color);

            MoveList moveList = new MoveList();

            foreach (Move move in baseMoveList)
            {
                Board testBoard = this.Copy();
                testBoard.ExecuteMove(move);
                if (!testBoard.IsChecked(color))
                    moveList.Add(move);
            }

            return moveList;
        }

        private List<Move> GetBaseMoveList(Color color)
        {
            return GetAllPieces(color).Select((piece => piece.GetMoveList().Moves)).SelectMany(move => move).ToList();
        }

        private bool IsChecked(Color color)
        {
            King king = (King)GetAllPieces(color).FirstOrDefault(piece => piece.Type == PieceType.King);
            return king?.IsChecked() ?? true ;
        }

        public BoardRating GetRating(Color color)
        {
            Counter++;

            BoardRating rating = new BoardRating ();

            King king = (King)GetAllPieces(color).FirstOrDefault(piece => piece.Type == PieceType.King);

            if (king == null)
            {
                rating.Situation = color == Color.White ? Situation.BlackVictory  : Situation.WhiteVictory;
                rating.Evaluation = color== Color.White ? Evaluation.WhiteCheckMate : Evaluation.BlackCheckMate;
                return rating;
            }

            if (IsChecked(color))
            {
                rating.Situation = color == Color.White ? Situation.WhiteChecked : Situation.BlackChecked;
                if (!GetMoveList(color).Moves.Any())
                {
                    rating.Situation = color == Color.White ? Situation.BlackVictory : Situation.WhiteVictory;
                    rating.Evaluation = color == Color.White ? Evaluation.WhiteCheckMate : Evaluation.BlackCheckMate;
                    return rating;
                }
            }
            else
            {
                if (!GetMoveList(color).Moves.Any())
                {
                    rating.Situation = Situation.StaleMate;
                    rating.Evaluation = color == Color.White ? Evaluation.WhiteStaleMate : Evaluation.BlackStaleMate;
                    return rating;
                }
            }

            int boardWeight = 0;

            GetAllPieces(Color.White).ForEach(piece => { boardWeight += piece.GetWeight();});
            GetAllPieces(Color.Black).ForEach(piece => { boardWeight += piece.GetWeight();});

            rating.Weight = boardWeight;

            return rating;
        }

        public Move CalculateMove(int depth, Color color)
        {
            if (depth <= 1)
                return Move.CreateNoMove(GetRating(color));

            var moveList = GetMoveList(color);
            if (!moveList.Moves.Any())
                return Move.CreateNoMove(GetRating(color));

            foreach (Move move in moveList.Moves)
            {
                Board copy = this.Copy();
                copy.ExecuteMove(move);
                copy.CalculateMove(depth - 1, ChessEngineConstants.NextColorToMove(color));
                move.Rating = copy.GetRating(color);
            }

            return moveList.GetBestMove(color);
        }
    }
}
