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
                SetPiece(whitePawnPositions[i], new Pawn(Color.White));
                SetPiece(blackPawnPositions[i], new Pawn(Color.Black));
            }

            // rook
            SetPiece("A1", new Rook(Color.White));
            SetPiece("H1", new Rook(Color.White));
            SetPiece("A8", new Rook(Color.Black));
            SetPiece("H8", new Rook(Color.Black));


            // bishop 
            SetPiece("C1", new Bishop(Color.White));
            SetPiece("F1", new Bishop(Color.White));
            SetPiece("C8", new Bishop(Color.Black));
            SetPiece("F8", new Bishop(Color.Black));


            // knight
            SetPiece("B1", new Knight(Color.White));
            SetPiece("G1", new Knight(Color.White));
            SetPiece("B8", new Knight(Color.Black));
            SetPiece("G8", new Knight(Color.Black));


            // queen
            SetPiece("D1", new Queen(Color.White));
            SetPiece("D8", new Queen(Color.Black));
            

            // king
            SetPiece("E1", new King(Color.White));
            SetPiece("E8", new King(Color.Black));

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
            DateTime s = DateTime.Now;

            Board.Counter = 0;
            Board.ClearOptimizationVariables();
            var move = Board.CalculateMove(4, ColorToMove); 

            TimeSpan ts = DateTime.Now.Subtract(s);

            _Message = move + " Time:" + ts + Environment.NewLine +
                       " Situation:" + move.Rating.Situation + " Evaluation:" + Environment.NewLine +
                       move.Rating.Evaluation + " Pieces:" + move.Rating.Weight + " Nodes:" + Board.Counter;

            return move;
        }
    }
}
