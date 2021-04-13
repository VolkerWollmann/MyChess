using MyChess.Common;
using MyChess.Model.Pieces;

namespace MyChess.Model
{
    public class ChessEngine : IChessEngine
    {
        private readonly Piece[,] Board;
        public ChessConstants.Color ColorToMove { get; set; }

        public IPiece[,] GetBoard()
        {
            return Board;
        }

        public ChessEngine Copy()
        {
            Piece[,] copiedboard = new Piece[8, 8];

            for(int row=0; row<8; row++)
            for (int column = 0; column < 8; column++)
            {
                copiedboard[row, column] = null;
                if (Board[row, column] != null)
                {
                    copiedboard[row, column] = PieceFactory.Copy(Board[row,column]);
                }
            }

            return new ChessEngine(copiedboard, ChessConstants.NextColor(ColorToMove));
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

            ColorToMove = ChessConstants.NextColor(ColorToMove);
            
            return true;
        }

        public ChessEngine()
        {
            Board = new Piece[8, 8];
        }
        private ChessEngine(Piece[,] board, ChessConstants.Color colorToMove)
        {
            Board = board;
            ColorToMove = colorToMove;
        }
    }
}
