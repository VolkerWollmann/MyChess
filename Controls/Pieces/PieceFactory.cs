using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using MyChess.Common;
using MyChess.Model;

namespace MyChess.Controls.Pieces
{
    public class PieceFactory
    {
        public static UserControl CreatePiece(IPiece piece)
        {
            switch (piece.GetPieceType())
            {
                case ChessConstants.Piece.Pawn:
                    return new PawnUserControl(piece);

                case ChessConstants.Piece.Bishop:
                    return new BishopUserControl(piece);

                case ChessConstants.Piece.Rook:
                    return new RookUserControl(piece);

                case ChessConstants.Piece.Queen:
                    return new QueenUserControl(piece);

                case ChessConstants.Piece.King:
                    return new KingUserControl(piece);

                case ChessConstants.Piece.Knight:
                    return new KnightUserControl(piece);
            }

            return null;
        }
    }
}
