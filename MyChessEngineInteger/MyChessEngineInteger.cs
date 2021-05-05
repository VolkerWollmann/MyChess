using System;
using MyChessEngineBase;
using MyChessEngineBase.Rating;
using MyChessEngineInteger.Pieces;

namespace MyChessEngineInteger
{
    public class ChessEngineInteger : IChessEngine
    {
        private readonly Board Board;
        public IPiece GetPiece(Position position)
        {
            NumPieces numPiece = Board[position.Row, position.Column];

            return new Piece(numPiece);
        }

        public Color ColorToMove { get; set; }


        private void SetPiece(string positionString, NumPieces piece)
        {
            Position position = new Position(positionString);
            Board[position.Row, position.Column] = piece;
        }
        public void New()
        {
            Board.Clear();
            string[] whitePawnPositions = new[] { "A2", "B2", "C2", "D2", "E2", "F2", "G2", "H2" };
            string[] blackPawnPositions = new[] { "A7", "B7", "C7", "D7", "E7", "F7", "G7", "H7" };
            for (int i = 0; i < 8; i++)
            {
                SetPiece(whitePawnPositions[i], NumPieces.WhitePawn);
                SetPiece(blackPawnPositions[i], NumPieces.BlackPawn);
            }

            SetPiece("A1", NumPieces.WhiteRook);
            Board.SetFlag(ChessEngineIntegerFlags.WhiteA1RookMoved,0);
            SetPiece("H1", NumPieces.WhiteRook);
            Board.SetFlag(ChessEngineIntegerFlags.WhiteH1RookMoved, 1);

            SetPiece("A8", NumPieces.BlackRook);
            Board.SetFlag(ChessEngineIntegerFlags.BlackA8RookMoved, 1);
            SetPiece("H8", NumPieces.BlackRook);
            Board.SetFlag(ChessEngineIntegerFlags.BlackH8RookMoved, 1);

            SetPiece("B1", NumPieces.WhiteKnight);
            SetPiece("G1", NumPieces.WhiteKnight);
            SetPiece("B8", NumPieces.BlackKnight);
            SetPiece("G8", NumPieces.BlackKnight);

            SetPiece("C1", NumPieces.WhiteBishop);
            SetPiece("F1", NumPieces.WhiteBishop);
            SetPiece("C8", NumPieces.BlackBishop);
            SetPiece("F8", NumPieces.BlackBishop);

            SetPiece("D1", NumPieces.WhiteQueen);
            SetPiece("D8", NumPieces.BlackQueen);

            SetPiece("E1", NumPieces.WhiteKing);
            Board.SetFlag(ChessEngineIntegerFlags.WhiteKingCastle,1);
            Board.SetFlag(ChessEngineIntegerFlags.WhiteKingLongCastle, 1);
            Board.SetFlag(ChessEngineIntegerFlags.WhiteKingRow,0);
            Board.SetFlag(ChessEngineIntegerFlags.WhiteKingColumn, 4);

            SetPiece("E8", NumPieces.BlackKing);
            Board.SetFlag(ChessEngineIntegerFlags.BlackKingCastle, 1);
            Board.SetFlag(ChessEngineIntegerFlags.BlackKingLongCastle, 1);
            Board.SetFlag(ChessEngineIntegerFlags.BlackKingRow, 7);
            Board.SetFlag(ChessEngineIntegerFlags.BlackKingColumn, 4);
        }

        public void Clear()
        {
            Board.Clear();
        }

        public IPiece this[string positionString]
        {
            get 
            { 
                Position position = new Position(positionString);
                return new Piece(Board[position.Row, position.Column]);
            }
            set
            {
                Piece piece = new Piece(value);
                NumPieces numPiece = piece.GetNumPieces();
                SetPiece(positionString, numPiece);
            }
        }

        public BoardRating GetRating(Color color)
        {
            Board copy = Board.Copy();

            var move = Board.CalculateMove(2, color);

            return move.Rating;
        }


        public BoardRating GetBoardRating()
        {
            return GetRating(ColorToMove);
        }

        public void Test()
        {
            throw new NotImplementedException();
        }

        public MoveList GetMoveList()
        {
            throw new NotImplementedException();
        }

        public bool ExecuteMove(MyChessEngineBase.Move move)
        {

            Board.ExecuteMove(new Move(move));

            ColorToMove = ChessEngineConstants.NextColorToMove(ColorToMove);

            return true;
        }

        private MyChessEngineBase.Move MoveToMove(Move move)
        {
            if (move.StartRow < 0)
            {
                return MyChessEngineBase.Move.CreateNoMove(move.Rating);
            }

            MyChessEngineBase.Move resultMove = new MyChessEngineBase.Move(
                new Position(move.StartColumn, move.StartColumn),
                new Position(move.EndRow, move.EndColumn),
                new Piece(Board[move.StartRow, move.StartColumn]),
                move.MoveType);

            resultMove.Rating = move.Rating;

            return resultMove;
        }

        public MyChessEngineBase.Move CalculateMove()
        {
            DateTime s = DateTime.Now;

            var move = Board.CalculateMove(6, ColorToMove);

            TimeSpan ts = DateTime.Now.Subtract(s);

            Message = move + " Time:" + ts + Environment.NewLine +
                       " Situation:" + move.Rating.Situation + " Evaluation:" + Environment.NewLine +
                       move.Rating.Evaluation + " Pieces:" + move.Rating.Weight;

            return MoveToMove(move);
        }

        public string Message { get; private set; }

        public ChessEngineInteger(Board board)
        {
            Board = board;
            Message = "";
        }

        public ChessEngineInteger()
        {
            Board = new Board();
            Message = "";
        }


    }
}
