using System;
using System.Linq;
using MyChessEngine.Pieces;
using MyChessEngineBase;
using MyChessEngineBase.Interfaces;
using MyChessEngineBase.Rating;

namespace MyChessEngine
{
    public class ChessEngine : IChessEngine
    {
        private readonly Board Board;

        #region IChessEngine

        public Color ColorToMove { get; set; }

        public IPiece  GetPiece(Position position)
        {
            return Board[position].Piece;
        }

        public void SetPiece(string position, Piece piece)
        {
            Board.SetPiece(position,piece);
        }

        public void SetPiece(Piece piece)
        {
            Board.SetPiece(piece);
        }

        public Field this[string position]
        {
            get {  return Board[position]; }
        }
        public ChessEngine Copy()
        {
            return new ChessEngine(Board.Copy(), ChessEngineConstants.NextColorToMove(ColorToMove));
        }


        public void New()
        {
            // pawn
            string[] whitePawnPositions = new[] { "A2", "B2", "C2", "D2", "E2", "F2", "G2", "H2"};
            string[] blackPawnPositions = new[] { "A7", "B7", "C7", "D7", "E7", "F7", "G7", "H7" };
            for (int i = 0; i < 8; i++)
            {
                SetPiece( new Pawn(Color.White, whitePawnPositions[i]));
                SetPiece( new Pawn(Color.Black,blackPawnPositions[i]));
            }

            // rook
            SetPiece(new Rook(Color.White, "A1"));
            SetPiece(new Rook(Color.White, "H1"));
            SetPiece(new Rook(Color.Black, "A8"));
            SetPiece(new Rook(Color.Black, "H8"));


            // bishop 
            SetPiece(new Bishop(Color.White, "C1"));
            SetPiece(new Bishop(Color.White, "F1"));
            SetPiece(new Bishop(Color.Black, "C8"));
            SetPiece(new Bishop(Color.Black, "F8"));


            // knight
            SetPiece(new Knight(Color.White, "B1"));
            SetPiece(new Knight(Color.White, "G1"));
            SetPiece(new Knight(Color.Black, "B8"));
            SetPiece(new Knight(Color.Black, "G8"));


            // queen
            SetPiece(new Queen(Color.White, "D1"));
            SetPiece(new Queen(Color.Black, "D8"));
            

            // king
            SetPiece(new King(Color.White, "E1"));
            SetPiece(new King(Color.Black, "E8"));

            ColorToMove = Color.White;
        }

        public void Clear()
        {
            Board.Clear();
        }

        public BoardRating GetRating(Color color)
        {
            return Board.GetRating(color);
        }


        public BoardRating GetBoardRating()
        {
            return Board.GetRating(ColorToMove);
        }

        public bool ExecuteMove(Move move)
        {
            move.Piece = Board[move.Start].Piece;
            Board.ExecuteMove(move);

            ColorToMove = ChessEngineConstants.NextColorToMove(ColorToMove);

            return true;
        }

        public void Test()
        {
            var allMoves = Board.GetAllPieces(ColorToMove).Select( (piece => piece.GetMoveList().Moves)).SelectMany( move => move).ToList();
            
            foreach (Move move3 in allMoves)
            {
                _Message = _Message + " " + move3 + Environment.NewLine;
            }
            
        }

        public MoveList GetMoveList()
        {
            return Board.GetMoveList(ColorToMove);
        }

        private string _Message;

        public string Message => _Message;
        #endregion

        public int Counter
        {
            get { return Board.Counter;  }
        }

        #region Constructors
        public ChessEngine()
        {
            Board = new Board();
        }
        private ChessEngine(Board board, Color colorToMove)
        {
            Board = board;
            ColorToMove = colorToMove;
        }
        #endregion

        public Move CalculateMove()
        {
            return CalculateMoveWithDepthParallel(6);
        }
        public Move CalculateMoveWithDepth( int depth = 6)
        {
            DateTime s = DateTime.Now;

            Board.Counter = 0;
            Board.ClearOptimizationVariables();
            var move = Board.CalculateMove(depth, ColorToMove); 

            TimeSpan ts = DateTime.Now.Subtract(s);

            _Message = move + " Time:" + ts + Environment.NewLine +
                       " Situation:" + move.Rating.Situation + " Evaluation:" + Environment.NewLine +
                       move.Rating.Evaluation + " Pieces:" + move.Rating.Weight + " Nodes:" + Board.Counter;

            return move;
        }

        public Move CalculateMoveWithDepthParallel(int depth = 6)
        {
            DateTime s = DateTime.Now;

            Board.Counter = 0;
            Board.ClearOptimizationVariables();
            var move = Board.CalculateMoveParallel(depth, ColorToMove);

            TimeSpan ts = DateTime.Now.Subtract(s);

            _Message = move + " Time:" + ts + Environment.NewLine +
                       " Situation:" + move.Rating.Situation + " Evaluation:" + Environment.NewLine +
                       move.Rating.Evaluation + " Pieces:" + move.Rating.Weight + " Nodes:" + Board.Counter;

            return move;
        }
    }
}
