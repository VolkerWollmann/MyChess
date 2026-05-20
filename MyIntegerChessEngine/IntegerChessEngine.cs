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

        public Move CalculateMove()
        {
            throw new NotImplementedException();
        }

        public string Message { get; }
    }
}
