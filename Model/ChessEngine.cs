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
            return new ChessEngine(Board.Copy(), ChessConstants.NextColorToMove(ColorToMove));
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
            Board["A1"] = new Rook(ChessConstants.Color.White);
            Board["H1"] = new Rook(ChessConstants.Color.White);
            Board["A8"] = new Rook(ChessConstants.Color.Black);
            Board["H8"] = new Rook(ChessConstants.Color.Black);


            // bishop 
            Board["C1"] = new Bishop(ChessConstants.Color.White);
            Board["F1"] = new Bishop(ChessConstants.Color.White);
            Board["C8"] = new Bishop(ChessConstants.Color.Black);
            Board["F8"] = new Bishop(ChessConstants.Color.Black);


            // knight
            Board["B1"] = new Knight(ChessConstants.Color.White);
            Board["G1"] = new Knight(ChessConstants.Color.White);
            Board["B8"] = new Knight(ChessConstants.Color.Black);
            Board["G8"] = new Knight(ChessConstants.Color.Black);


            // queen
            Board["D1"] = new Queen(ChessConstants.Color.White);
            Board["D8"] = new Queen(ChessConstants.Color.Black);
            

            // king
            Board["E1"] = new King(ChessConstants.Color.White);
            Board["E8"] = new King(ChessConstants.Color.Black);

            ColorToMove = ChessConstants.Color.White;
        }

        public void Clear()
        {
            Board.Clear();
        }

        public bool ExecuteMove(Move move)
        {
            bool result = Board.ExecuteMove(move);

            ColorToMove = ChessConstants.NextColorToMove(ColorToMove);
            
            return result;
        }

        public void Test()
        {
            var x = new Position("A1");
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
