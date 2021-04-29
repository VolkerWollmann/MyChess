using System;
using MyChessEngineBase;
using MyChessEngineBase.Rating;
using MyChessEngineInteger.Pieces;

namespace MyChessEngineInteger
{
    public class MyChessEngineInteger : IChessEngine
    {
        private readonly IntegerBoard Board;
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
            return Board.GetRating(color);
        }


        public BoardRating GetBoardRating()
        {
            // arr.Sum();
            return Board.GetRating(ColorToMove);
        }

        public void Test()
        {
            throw new NotImplementedException();
        }

        public MoveList GetMoveList()
        {
            throw new NotImplementedException();
        }

        public bool ExecuteMove(Move move)
        {
            Board.ExecuteMove(move);

            ColorToMove = ChessEngineConstants.NextColorToMove(ColorToMove);

            return true;
        }

        public Move CalculateMove()
        {
            throw new NotImplementedException();
        }

        public string Message { get; }

        public MyChessEngineInteger(IntegerBoard board)
        {
            Board = board;
            Message = "";
        }
    }
}
