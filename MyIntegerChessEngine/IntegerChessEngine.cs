using MyChessEngine.Pieces;
using MyChessEngineBase;

//using MyChessEngineBase;
using MyChessEngineBase.Interfaces;
using MyChessEngineBase.Rating;

namespace MyIntegerChessEngine
{
    public class IntegerChessEngine 
    {
        public Board Board;

        public Piece GetPiece(Position position)
        {
            throw new NotImplementedException();
        }

        public void SetPiece(Position position, Piece piece)
        {
            throw new NotImplementedException();
        }

        public void SetPiece(string position, Piece piece)
        {
            throw new NotImplementedException();
        }

        public int ColorToMove
        {
            get => Board.ColorToMove;
            set => Board.ColorToMove = value;
        }
        public void New()
        {
            Board = new Board();

            // pawn
            string[] whitePawnPositions = ["A2", "B2", "C2", "D2", "E2", "F2", "G2", "H2"];
            string[] blackPawnPositions = ["A7", "B7", "C7", "D7", "E7", "F7", "G7", "H7"];
            for (int i = 0; i < 8; i++)
            {
                SetPiece(PieceFactory.WhitePawn(), whitePawnPositions[i]);
                SetPiece(PieceFactory.BlackPawn(), blackPawnPositions[i]);
            }

            // Knights
            SetPiece(PieceFactory.WhiteKnight(), "B1");
            SetPiece(PieceFactory.WhiteKnight(), "G1");
            SetPiece(PieceFactory.BlackKnight(), "B8");
            SetPiece(PieceFactory.BlackKnight(), "G8");

            // Bishops
            SetPiece(PieceFactory.WhiteBishop(), "C1");
            SetPiece(PieceFactory.WhiteBishop(), "F1");
            SetPiece(PieceFactory.BlackBishop(), "C8");
            SetPiece(PieceFactory.BlackBishop(), "F8");

            // Rooks
            SetPiece(PieceFactory.WhiteRook(), "A1");
            SetPiece(PieceFactory.WhiteRook(), "H1");
            SetPiece(PieceFactory.BlackRook(), "A8");
            SetPiece(PieceFactory.BlackRook(), "H8");

            // Queens
            SetPiece(PieceFactory.WhiteQueen(), "D1");
            SetPiece(PieceFactory.BlackQueen(), "D8");

            // Kings
            SetPiece(PieceFactory.WhiteKing(), "E1");
            SetPiece(PieceFactory.BlackKing(), "E8");
        }

        /// Returns all possible moves of the side to move.
        public MoveList GetMoveList()
        {
            return Board.GetMoveList();
        }

        /// Returns all fields where an actual or possible beat by <paramref name="color"/> can happen.
        public MoveList GetThreatenMoveList(int color)
        {
            return Board.GetThreatenMoveList(color);
        }

        /// Material rating: white pieces count positive, black pieces negative.
        /// A missing king turns the state into WhiteLoss/BlackLoss.
        public Rating GetRating()
        {
            return Board.GetRating();
        }

        public bool ExecuteMove(Move move)
        {
            Board.ExecuteMove(move);

            ColorToMove = ColorToMove == Constants.White ? Constants.Black : Constants.White;
            
            return true;
        }

        public void Clear()
        {
            Board = new Board();
        }

        public void SetPiece(Piece piece, Position position)
        {
            Board.SetPiece(piece, position);
        }

        public void SetPiece(Piece piece, string position)
        {
            Board.SetPiece(piece, new Position( position));
        }

        public BoardRating GetRating(int color)
        {
            throw new NotImplementedException();
        }

        public void Test()
        {
            throw new NotImplementedException();
        }

        public BoardRating GetBoardRating()
        {
            throw new NotImplementedException();
        }

        /// Depth search (minimax) for the best move of the side to move.
        /// Returns null if there is no legal move or the game is already over.
        public Move CalculateMove(int depth = Constants.DefaultSearchDepth)
        {
            return Board.CalculateMove(depth);
        }

        /// Depth search like CalculateMove, the first level moves are searched in parallel.
        public Move CalculateMoveParallel(int depth = Constants.DefaultSearchDepth)
        {
            return Board.CalculateMoveParallel(depth);
        }

        public string Message { get; }
    }
}
