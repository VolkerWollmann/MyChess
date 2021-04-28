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
            throw new NotImplementedException();
        }

        public Color ColorToMove { get; set; }


        private void SetPiece(string positionString, NumPieces piece)
        {
            Position position = new Position(positionString);
            Board[position.Row, position.Column] = (int)piece;
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
            Board.SetFlag(ChessEngineIntegerFlags.WhiteA1Rook,1);
            SetPiece("H1", NumPieces.WhiteRook);
            Board.SetFlag(ChessEngineIntegerFlags.WhiteH1Rook, 1);

            SetPiece("A8", NumPieces.BlackRook);
            Board.SetFlag(ChessEngineIntegerFlags.BlackA8Rook, 1);
            SetPiece("H8", NumPieces.BlackRook);
            Board.SetFlag(ChessEngineIntegerFlags.BlackH8Rook, 1);

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
            Board.SetFlag(ChessEngineIntegerFlags.WhiteKingRow, 7);
            Board.SetFlag(ChessEngineIntegerFlags.WhiteKingColumn, 4);
        }

        public void Clear()
        {
            Board.Clear();
        }

        public IPiece this[string positionString]
        {
            get { Position position = new Position(positionString);
                     return new Piece(Board[position.Row, position.Column]);
               }
            set
            {
                NumPieces piece = 0;
                switch (value.Type)
                {
                    case PieceType.Pawn:
                        piece = (value.Color == Color.White) ? NumPieces.WhitePawn : NumPieces.BlackPawn;
                        break;
                    case PieceType.Knight:
                        piece = (value.Color == Color.White) ? NumPieces.WhiteKnight : NumPieces.BlackKnight;
                        break;
                    case PieceType.Bishop:
                        piece = (value.Color == Color.White) ? NumPieces.WhiteBishop : NumPieces.BlackBishop;
                        break;
                    case PieceType.Rook:
                        piece = (value.Color == Color.White) ? NumPieces.WhiteRook : NumPieces.BlackRook;
                        break;
                    case PieceType.Queen:
                        piece = (value.Color == Color.White) ? NumPieces.WhiteQueen : NumPieces.BlackQueen;
                        break;
                    case PieceType.King:
                        piece = (value.Color == Color.White) ? NumPieces.WhiteKing : NumPieces.BlackKing;
                        break;

                }
                SetPiece(positionString, piece);
            }
        }

        public BoardRating GetRating(Color color)
        {
            throw new NotImplementedException();
        }

        public void Test()
        {
            throw new NotImplementedException();
        }

        public MoveList GetMoveList()
        {
            throw new NotImplementedException();
        }

        public BoardRating GetBoardRating()
        {
            throw new NotImplementedException();
        }

        public bool ExecuteMove(Move move)
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
