using System;
using System.Collections.Generic;
using System.Text;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    public class PieceFactory
    {
        public static Piece Copy(Piece piece)
        {
            return piece.GetPieceType() switch
            {
                ChessConstants.Piece.Pawn => new Pawn(piece.GetColor()),
                ChessConstants.Piece.Bishop => new Bishop(piece.GetColor()),
                ChessConstants.Piece.King => new King(piece.GetColor()),
                ChessConstants.Piece.Knight => new Knight(piece.GetColor()),
                ChessConstants.Piece.Queen => new Queen(piece.GetColor()),
                ChessConstants.Piece.Rook => new Rook(piece.GetColor()),
                _ => null
            };
        }
    }
}
