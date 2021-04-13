using MyChess.Common;
using MyChess.Model.Pieces;

namespace MyChess.Model
{
    public class ChessEngine : IChessEngine
    {
        private readonly IPiece[,] Board = new IPiece[8, 8];
        private ChessConstants.Color ColorToMove;

        public IPiece[,] GetBoard()
        {
            return Board;
        }

        public void New()
        {
            // pawn
            for (int column = 0; column < 8; column++)
            {
                Board[1, column] = new Pawn(ChessConstants.Color.White);
                Board[6, column] = new Pawn(ChessConstants.Color.Black);
               
            }

            // rook
            Board[0, 0] = new Rook(ChessConstants.Color.White);
            Board[0, 7] = new Rook(ChessConstants.Color.White);
            Board[7, 0] = new Rook(ChessConstants.Color.Black);
            Board[7, 7] = new Rook(ChessConstants.Color.Black);


            // bishop 
            Board[0, 2] = new Bishop(ChessConstants.Color.White);
            Board[0, 5] = new Bishop(ChessConstants.Color.White);
            Board[7, 2] = new Bishop(ChessConstants.Color.Black);
            Board[7, 5] = new Bishop(ChessConstants.Color.Black);


            // knight
            Board[0, 1] = new Knight(ChessConstants.Color.White);
            Board[0, 6] = new Knight(ChessConstants.Color.White);
            Board[7, 1] = new Knight(ChessConstants.Color.Black);
            Board[7, 6] = new Knight(ChessConstants.Color.Black);


            // queen
            Board[0, 3] = new Queen(ChessConstants.Color.White);
            Board[7, 3] = new Queen(ChessConstants.Color.Black);
            

            // king
            Board[0, 4] = new King(ChessConstants.Color.White);
            Board[7, 4] = new King(ChessConstants.Color.Black);

            ColorToMove = ChessConstants.Color.White;
        }

        public void Clear()
        {
            for(int row=0; row<ChessConstants.Length; row++)
            for (int column = 0; column < ChessConstants.Length; column++)
            {
                Board[row, column] = null;
            }
        }

        public bool ExecuteMove(int startRow, int startColumn, int endRow, int endColumn)
        {
            if (Board[startRow, startColumn] == null)
                return false;

            Board[endRow, endColumn] = Board[startRow, startColumn];
            Board[startRow, startColumn] = null;

            ColorToMove = ColorToMove == ChessConstants.Color.White ? ChessConstants.Color.Black : ChessConstants.Color.White;
            
            return true;
        }
    }
}
