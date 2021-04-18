using System;
using System.Linq;
using MyChessEngine.Pieces;

namespace MyChessEngine
{
    public partial class ChessEngine : IChessEngine
    {
        public readonly Board Board;
        
        
        #region IChessEngine

        public Color ColorToMove { get; set; }

        public IPiece  GetPiece(Position position)
        {
            return Board[position];
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
                Board[whitePawnPositions[i]] = new Pawn(Color.White);
                Board[blackPawnPositions[i]] = new Pawn(Color.Black);
            }

            // rook
            Board["A1"] = new Rook(Color.White);
            Board["H1"] = new Rook(Color.White);
            Board["A8"] = new Rook(Color.Black);
            Board["H8"] = new Rook(Color.Black);


            // bishop 
            Board["C1"] = new Bishop(Color.White);
            Board["F1"] = new Bishop(Color.White);
            Board["C8"] = new Bishop(Color.Black);
            Board["F8"] = new Bishop(Color.Black);


            // knight
            Board["B1"] = new Knight(Color.White);
            Board["G1"] = new Knight(Color.White);
            Board["B8"] = new Knight(Color.Black);
            Board["G8"] = new Knight(Color.Black);


            // queen
            Board["D1"] = new Queen(Color.White);
            Board["D8"] = new Queen(Color.Black);
            

            // king
            Board["E1"] = new King(Color.White);
            Board["E8"] = new King(Color.Black);

            ColorToMove = Color.White;
        }

        public void Clear()
        {
            Board.Clear();
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
            var allMoves = this.Board.GetAllPieces(ColorToMove).Select( (piece => piece.GetMoveList().Moves)).SelectMany( move => move).ToList();
            
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

        public ChessEngine()
        {
            Board = new Board();
        }
        private ChessEngine(Board board, Color colorToMove)
        {
            Board = board;
            ColorToMove = colorToMove;
        }

        public Move CalculateMove()
        {
            DateTime s = DateTime.Now;

            Board.Counter = 0;
            Move move = Board.CalculateMove(4, ColorToMove);

            TimeSpan ts = DateTime.Now.Subtract(s);

            _Message = move + " " + ts + " " + move.Rating.Situation + " "
                      + move.Rating.Evaluation + " " + move.Rating.Weight + " " + Board.Counter;

            return move;
        }
    }
}
