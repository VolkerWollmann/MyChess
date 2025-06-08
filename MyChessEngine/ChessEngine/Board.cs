using System;
using System.Collections.Generic;
using System.Linq;
using MyChessEngineBase.Rating;
using MyChessEngineBase;
using MyChessEngine.Pieces;


namespace MyChessEngine
{
    public class Board
    {
        private readonly Field[,] Field;
        
        private string[] MoveStack = ["", "", "", "", "", "", "", "", "", ""];

        public static int Counter;

        public Board()
        {
            Field = new Field[8, 8];
            for(int row = 0; row < ChessEngineConstants.Length; row++)
            {
                for (int column = 0; column < ChessEngineConstants.Length; column++)
                {
                    Field[column, row] = new Field( new Position(column,row).ToString());
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

        public void SetPiece(Position position,Piece piece)
        {
            this[position].Piece = piece;
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
            if (_AllPiecesByColor.ContainsKey(color))
                return _AllPiecesByColor[color];

            var pieces = Field.Cast<Field>().Where(field => (field.Piece?.Color == color)).Select(field => field.Piece).ToList();

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
            Position.AllPositions().ForEach(position => { this[position].Piece = null; this[position].Threat = false; });
        }


        public virtual Board Copy()
        {
            Board copy = new Board();

            for (int i = 0; i < 9; i++)
                copy.MoveStack[i] = (string)MoveStack[i].Clone();
            
            for (int row = 0; row < ChessEngineConstants.Length; row++)
                for (int column = 0; column < ChessEngineConstants.Length; column++)
                {
                    Piece piece = this[column,row].Piece;
                    if (piece != null)
                    {
                        Piece pc = piece.Copy();
                        copy.SetPiece(piece.Position, pc);
                    }
                }

            return copy;
        }

        public virtual bool ExecuteMove(Move move)
        {
            return ExecuteMove(move, 0);
        }
        
        public virtual bool ExecuteMove(Move move, int depth = 0)
        {
            MoveStack[depth] = move.ToString();
            if (this[move.Start] == null)
                throw new Exception("Move not Existing piece.");

            if (!move.End.IsValidPosition())
                throw new Exception("Move to invalid position.");

            this[move.Start].Piece.ExecuteMove(move);

            return true;
        }

        private Dictionary<Color, MoveList> _AllMovesByColor = new Dictionary<Color, MoveList>();

        public MoveList GetMoveList(Color color)
        {
            if (!_AllMovesByColor.ContainsKey(color))
                _AllMovesByColor[color] = new MoveList(GetBaseMoveList(color));

            return _AllMovesByColor[color];
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
                Piece piece = field.Piece;
                if (piece != null)
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

        private List<Position> GetThreatenedFields(Color color)
        {
            var pieces = GetAllPieces(color);
            List<Move> listOfMoves = new List<Move>();
            foreach (Piece piece in pieces)
            {
                List<Move> moves = new List<Move>();
                moves = piece.GetThreatenMoveList().Moves;
                listOfMoves.AddRange(moves);
            }
            
            List<Position> threatenedFields = listOfMoves.Select(move => move.End).ToList();

            return threatenedFields;


            //return GetAllPieces(color)
            //    .Select((piece => piece.GetThreatenMoveList().Moves))
            //    .SelectMany(move => move).Select(move => move.End).ToList();
        }
        public virtual Move CalculateMove(int depth, Color color)
        {
            // Mark the threatened fields
            Board copy = Copy();

            List<Position> threatenedFields = GetThreatenedFields(ChessEngineConstants.NextColorToMove(color));
            threatenedFields.ForEach(pos => copy[pos].Threat = true);

            var moves = copy.GetMoveList(color);

            var rating = GetRating(color);
            
            if ( depth == 0 || rating.Situation == Situation.WhiteVictory || rating.Situation == Situation.BlackVictory )    
                return Move.CreateNoMove(rating);
            

            MoveList result = new MoveList();
            IBoardRatingComparer comparer = BoardRatingComparerFactory.GetComparer(color);

            foreach (Move move in moves.Moves)
            {
                MoveStack[depth] = move.ToString();
                Board copy2 = copy.Copy();
                copy2.ExecuteMove(move,depth);

                Move resultMove = copy2.CalculateMove(depth - 1, ChessEngineConstants.NextColorToMove(color));
                move.Rating = resultMove.Rating;
                move.Rating.Depth = move.Rating.Depth + 1;
                
                result.Add(move);
            }

            var king = copy.Kings[color];
            bool check = copy[king.Position].Threat;
            Move resultMove2 = result.GetBestMove(color, check);
            return resultMove2;
        }
    }
}
