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
                Board[whitePawnPositions[i]].Piece = new Pawn(Color.White);
                Board[blackPawnPositions[i]].Piece = new Pawn(Color.Black);
            }

            // rook
            Board["A1"].Piece = new Rook(Color.White);
            Board["H1"].Piece = new Rook(Color.White);
            Board["A8"].Piece = new Rook(Color.Black);
            Board["H8"].Piece = new Rook(Color.Black);


            // bishop 
            Board["C1"].Piece = new Bishop(Color.White);
            Board["F1"].Piece = new Bishop(Color.White);
            Board["C8"].Piece = new Bishop(Color.Black);
            Board["F8"].Piece = new Bishop(Color.Black);


            // knight
            Board["B1"].Piece = new Knight(Color.White);
            Board["G1"].Piece = new Knight(Color.White);
            Board["B8"].Piece = new Knight(Color.Black);
            Board["G8"].Piece = new Knight(Color.Black);


            // queen
            Board["D1"].Piece = new Queen(Color.White);
            Board["D8"].Piece = new Queen(Color.Black);
            

            // king
            Board["E1"].Piece = new King(Color.White);
            Board["E8"].Piece = new King(Color.Black);

            ColorToMove = Color.White;
        }

        public void Clear()
        {
            Board.Clear();
        }

        public IPiece this[string position]
        {
            get => Board[position].Piece;
            set => Board[position].Piece = (Piece)value;
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
