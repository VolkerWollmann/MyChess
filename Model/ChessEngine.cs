using System.Linq;
using MyChess.Common;
using MyChess.Model.Pieces;

namespace MyChess.Model
{
    public class ChessEngine : IChessEngine
    {
        private readonly Board Board;
        public ChessConstants.Color ColorToMove { get; set; }

        public IPiece  GetPiece(Position position)
        {
            return Board[position];
        }

        public ChessEngine Copy()
        {
            return new ChessEngine(Board.Copy(), ChessConstants.NextColor(ColorToMove));
        }


        public void New()
        {
            // pawn
            for (int column = 0; column < 8; column++)
            {
                Position position = new Position(1, column);
                Board[position] = new Pawn(ChessConstants.Color.White);

                position = new Position(6, column);
                Board[position] = new Pawn(ChessConstants.Color.Black);
               
            }

            // rook
            Board[new Position(0, 0)] = new Rook(ChessConstants.Color.White);
            Board[new Position(0, 7)] = new Rook(ChessConstants.Color.White);
            Board[new Position(7, 0)] = new Rook(ChessConstants.Color.Black);
            Board[new Position(7, 7)] = new Rook(ChessConstants.Color.Black);


            // bishop 
            Board[new Position(0, 2)] = new Bishop(ChessConstants.Color.White);
            Board[new Position(0, 5)] = new Bishop(ChessConstants.Color.White);
            Board[new Position(7, 2)] = new Bishop(ChessConstants.Color.Black);
            Board[new Position(7, 5)] = new Bishop(ChessConstants.Color.Black);


            // knight
            Board[new Position(0, 1)] = new Knight(ChessConstants.Color.White);
            Board[new Position(0, 6)] = new Knight(ChessConstants.Color.White);
            Board[new Position(7, 1)] = new Knight(ChessConstants.Color.Black);
            Board[new Position(7, 6)] = new Knight(ChessConstants.Color.Black);


            // queen
            Board[new Position(0, 3)] = new Queen(ChessConstants.Color.White);
            Board[new Position(7, 3)] = new Queen(ChessConstants.Color.Black);
            

            // king
            Board[new Position(0, 4)] = new King(ChessConstants.Color.White);
            Board[new Position(7, 4)] = new King(ChessConstants.Color.Black);

            ColorToMove = ChessConstants.Color.White;
        }

        public void Clear()
        {
            Board.Clear();
        }

        public bool ExecuteMove(Move move)
        {
            bool result = Board.ExecuteMove(move);

            ColorToMove = ChessConstants.NextColor(ColorToMove);
            
            return result;
        }

        public void Test()
        {
            var allMoves = this.Board.GetAllPieces(ColorToMove).Select( (piece => piece.GetMoves())).SelectMany( move => move).ToList();
        }
        public ChessEngine()
        {
            Board = new Board();
        }
        private ChessEngine(Board board, ChessConstants.Color colorToMove)
        {
            Board = board;
            ColorToMove = colorToMove;
        }
    }
}
