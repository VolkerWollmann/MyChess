using System;
using System.Linq;
using MyChessEngine.Pieces;
using MyChessEngineBase.Rating;
using MyChessEngineBase;
using MyChessEngineBase.Interfaces;

namespace MyChessEngine
{
    public class ChessEngine2 : IChessEngine
    {
        private readonly Board2 Board;

        #region IChessEngine

        public Color ColorToMove { get; set; }

        public IPiece GetPiece(Position position)
        {
            return Board[position].Piece;
        }

        public ChessEngine2 Copy()
        {
            return new ChessEngine2(Board.Copy(), ChessEngineConstants.NextColorToMove(ColorToMove));
        }

        public void SetPiece(string position, Piece piece)
        {
            Board.SetPiece(position, piece);
        }

        public void SetPiece(Piece piece)
        {
            Board.SetPiece(piece);
        }

        public void New()
        {
            // pawn
            string[] whitePawnPositions = new[] { "A2", "B2", "C2", "D2", "E2", "F2", "G2", "H2" };
            string[] blackPawnPositions = new[] { "A7", "B7", "C7", "D7", "E7", "F7", "G7", "H7" };
            for (int i = 0; i < 8; i++)
            {
                Board.SetPiece(new Pawn(Color.White, whitePawnPositions[i] ));
                Board.SetPiece(new Pawn(Color.Black, blackPawnPositions[i] ));
               
            }

            // rook
            Board.SetPiece(new Rook(Color.White, "A1"));
            Board.SetPiece(new Rook(Color.White, "H1"));
            Board.SetPiece(new Rook(Color.Black, "A8"));
            Board.SetPiece(new Rook(Color.Black, "H8"));
            
            // bishop 
            Board.SetPiece(new Bishop(Color.White, "C1"));
            Board.SetPiece(new Bishop(Color.White, "F1"));
            Board.SetPiece(new Bishop(Color.Black, "C8"));
            Board.SetPiece(new Bishop(Color.Black, "F8"));

            // knight
            Board.SetPiece(new Knight(Color.White, "B1"));
            Board.SetPiece(new Knight(Color.White, "G1"));
            Board.SetPiece(new Knight(Color.Black, "B8"));
            Board.SetPiece(new Knight(Color.Black, "G8"));



            // queen
            Board.SetPiece(new Queen(Color.White, "D1"));
            Board.SetPiece(new Queen(Color.Black, "D8"));


            // king
            Board.SetPiece( new King(Color.White, "E1",
                (MoveType.Normal | MoveType.WhiteCastle | MoveType.WhiteCastleLong), -1));
            Board.SetPiece( new King(Color.Black, "E8",
                (MoveType.Normal | MoveType.BlackCastle | MoveType.BlackCastleLong), -1));
            

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
            return Board.CalculateMove(2, color).Rating;
        }


        public BoardRating GetBoardRating()
        {
            return Board.CalculateMove(2, ColorToMove).Rating;
        }

        public bool ExecuteMove(Move move)
        {
            move.Piece ??= Board[move.Start].Piece;

            Board.ExecuteMove(move);

            ColorToMove = ChessEngineConstants.NextColorToMove(ColorToMove);

            return true;
        }

        public void Test()
        {
            var allMoves = Board.GetAllPieces(ColorToMove).Select((piece => piece.GetMoveList().Moves)).SelectMany(move => move).ToList();

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

        public ChessEngine2()
        {
            Board = new Board2();
        }
        private ChessEngine2(Board2 board, Color colorToMove)
        {
            Board = board;
            ColorToMove = colorToMove;
        }

        public Move CalculateMove()
        {
            DateTime s = DateTime.Now;

            MyChessEngine.Board.Counter = 0;

            Board.ClearOptimizationVariables();
            var move = Board.CalculateMove(6, ColorToMove);

            TimeSpan ts = DateTime.Now.Subtract(s);

            _Message = move + " Time:" + ts + Environment.NewLine +
                       " Situation:" + move.Rating.Situation + " Evaluation:" + Environment.NewLine +
                       move.Rating.Evaluation + " Pieces:" + move.Rating.Weight + " Nodes:" + MyChessEngine.Board.Counter;

            return move;
        }
    }
}
