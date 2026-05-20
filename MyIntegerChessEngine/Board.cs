using System;
using System.Collections.Generic;
using MyChessEngine.Pieces;
using MyChessEngineBase;
using MyChessEngineBase.Rating;

namespace MyIntegerChessEngine
{
    
    /// 12×12 padded board: A1 = [2,2], H8 = [9,9] 
    /// Each cell: [0] piece, [1] LastPly, [2] aux metadata.
    /// </summary>
    public class Board
    {
        public int[,,] Field = new int[Constants.Planes, Constants.GridSize, Constants.GridSize];

        public int CurrentPly;

        public void InitBorder()
        {
            for(int i=0; i<Constants.GridSize; i++)
            {
                Field[0,i,0] = Constants.BoardBorder;
                Field[0,i,1] = Constants.BoardBorder;
                Field[0,i,10] = Constants.BoardBorder;
                Field[0,i,11] =  Constants.BoardBorder;

                Field[0, 0, i] = Constants.BoardBorder;
                Field[0, 1, i] = Constants.BoardBorder;
                Field[0, 10, i] = Constants.BoardBorder;
                Field[0, 11, i] = Constants.BoardBorder;
            }
        }

        public void SetPiece(Piece piece, Position position)
        {
            Field[Constants.PiecePlane, position.Column + 2, position.Row + 2] = piece.PieceAsInteger;
            Field[Constants.LastPlyPlane, position.Column + 2, position.Row + 2] = piece.LastPly;
            Field[Constants.PromotionPlane, position.Column + 2, position.Row + 2] = piece.PromotionPly;
            Field[Constants.EnPassantPlane, position.Column + 2, position.Row + 2] = piece.LastEnPassantPlyMarking;
        }

        public Piece GetPiece(Position position)
        {
            int pieceValue = Field[Constants.PiecePlane, position.Column + 2, position.Row + 2];
            int lastPly = Field[Constants.LastPlyPlane, position.Column + 2, position.Row + 2];
            int promotionPly = Field[Constants.PromotionPlane, position.Column + 2, position.Row + 2];
            int lastEnPassantPlyMarking = Field[Constants.EnPassantPlane, position.Column + 2, position.Row + 2];
            return new Piece(pieceValue, lastPly, promotionPly, lastEnPassantPlyMarking);
        }   

        private void HandleCastlingRights(Move move)
        {
            if (move.Piece.Color == Constants.White)
            {
                if (move.Piece.PieceType == Constants.King)
                {
                    DisableWhiteCastleKingSidePossible();
                    DisableWhiteCastleQueenSidePossible();
                }
                else if (move.Piece.PieceType == Constants.Rook)
                {
                    if (move.Start is { Column: 0, Row: 0 })
                        DisableWhiteCastleQueenSidePossible();
                    else if (move.Start is { Column: 7, Row: 0 })
                        DisableWhiteCastleKingSidePossible();
                }
            }
            if (move.Piece.Color == Constants.Black)
            {
                if (move.Piece.PieceType == Constants.King)
                {
                    DisableBlackCastleKingSidePossible();
                    DisableBlackCastleQueenSidePossible();
                }
                else if (move.Piece.PieceType == Constants.Rook)
                {
                    if (move.Start is { Column: 0, Row: 7 })
                        DisableBlackCastleQueenSidePossible();
                    else if (move.Start is { Column: 7, Row: 7 })
                        DisableBlackCastleKingSidePossible();
                }
            }
        }
        public void ExecuteMove(Move move)
        {
            Field[Constants.PiecePlane, move.End.Column + 2, move.End.Row + 2] 
                = Field[Constants.PiecePlane, move.Start.Column + 2, move.Start.Row + 2];

            CurrentPly++;
            Field[Constants.LastPlyPlane, move.End.Column + 2, move.End.Row + 2] = CurrentPly;

            HandleCastlingRights(move);
            
        }

        public void New()
        {
            Field = new int[Constants.LastPlyPlane, Constants.GridSize, Constants.GridSize];

        }

        int GetPieceValue(Position position)
        {
            return Field[Constants.LastPlyPlane, position.Column+2, position.Row+2];
        }

        #region Castling
        public bool WhiteCastleKingSidePossible()
        {
            return Field[Constants.LastPlyPlane, 0, 0] == 0;
        }

        public bool WhiteCastleQueenSidePossible()
        {
            return Field[Constants.LastPlyPlane, 0, 1] == 0;
        }


        public bool BlackCastleKingSidePossible()
        {
            return Field[Constants.LastPlyPlane, 0, 2] == 0;
        }

        public bool BlackCastleQueenSidePossible()
        {
            return Field[Constants.LastPlyPlane, 0, 3] == 0;
        }

        public void DisableWhiteCastleKingSidePossible()
        {
            Field[Constants.LastPlyPlane, 0, 0] = 1;
        }

        public void DisableWhiteCastleQueenSidePossible()
        {
            Field[Constants.LastPlyPlane, 0, 1] = 1;
        }


        public void DisableBlackCastleKingSidePossible()
        {
            Field[Constants.LastPlyPlane, 0, 2] = 1;
        }

        public void DisableBlackCastleQueenSidePossible()
        {
            Field[Constants.LastPlyPlane, 0, 3] = 1;
        }
        #endregion

        public int ColorToMove
        {
            get => Field[Constants.LastPlyPlane, 0, 3];
            set => Field[Constants.LastPlyPlane, 0, 3] = value;
        }


        public Board Copy()
        {
            Board newBoard = new();
            newBoard.Field = (int[,,])Field.Clone();
            newBoard.CurrentPly = CurrentPly;

            return newBoard;
        }

        public bool Compare(Board other)
        {
            for (int i = 2; i < 2+ ChessEngineConstants.Length; i++)
            for (int j = 2; j < 2+ ChessEngineConstants.Length; j++)
            {
                if (this.Field[0,i, j] != other.Field[0,i, j]) 
                    return false;
            }

            return true;
        }
    }
}
