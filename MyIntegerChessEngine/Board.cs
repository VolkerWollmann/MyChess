using System;
using System.Collections.Generic;
using MyChessEngineBase;
using MyIntegerChessEngine.Pieces;

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
            Field[Constants.BroadPlane, position.Column + 2, position.Row + 2] = piece.PieceAsInteger;
            Field[Constants.LastPlyPlane, position.Column + 2, position.Row + 2] = piece.LastPly;
            Field[Constants.PromotionPlane, position.Column + 2, position.Row + 2] = piece.PromotionPly;
            Field[Constants.EnPassantPlane, position.Column + 2, position.Row + 2] = piece.LastEnPassantPlyMarking;
        }

        public Piece GetPiece(Position position)
        {
            int pieceValue = Field[Constants.BroadPlane, position.Column + 2, position.Row + 2];
            int lastPly = Field[Constants.LastPlyPlane, position.Column + 2, position.Row + 2];
            int promotionPly = Field[Constants.PromotionPlane, position.Column + 2, position.Row + 2];
            int lastEnPassantPlyMarking = Field[Constants.EnPassantPlane, position.Column + 2, position.Row + 2];
            return new Piece(pieceValue, lastPly, promotionPly, lastEnPassantPlyMarking);
        }   

        public void ExecuteMove(Move move)
        {
            CurrentPly++;

            // Piece-specific handling runs on the pre-move board:
            // King: castle rook move + castle right invalidation
            // Rook: castle right invalidation
            // Pawn: en passant capture and en passant marking
            switch (move.Piece.PieceType)
            {
                case Constants.King:
                    King.ExecuteMove(this, move);
                    break;
                case Constants.Rook:
                    Rook.ExecuteMove(this, move);
                    break;
                case Constants.Pawn:
                    Pawn.ExecuteMove(this, move);
                    break;
            }

            MovePiece(move.Start, move.End);
        }

        /// Transfers all planes from start to end, stamps the end square with CurrentPly
        /// and clears the start square. Does not increment the ply.
        internal void MovePiece(Position start, Position end)
        {
            int startColumn = start.Column + 2;
            int startRow = start.Row + 2;
            int endColumn = end.Column + 2;
            int endRow = end.Row + 2;

            Field[Constants.BroadPlane, endColumn, endRow]
                = Field[Constants.BroadPlane, startColumn, startRow];
            Field[Constants.LastPlyPlane, endColumn, endRow] = CurrentPly;
            Field[Constants.PromotionPlane, endColumn, endRow]
                = Field[Constants.PromotionPlane, startColumn, startRow];
            Field[Constants.EnPassantPlane, endColumn, endRow]
                = Field[Constants.EnPassantPlane, startColumn, startRow];

            ClearSquare(start);
        }

        internal void ClearSquare(Position position)
        {
            int column = position.Column + 2;
            int row = position.Row + 2;

            Field[Constants.BroadPlane, column, row] = Constants.NoPiece;
            Field[Constants.LastPlyPlane, column, row] = 0;
            Field[Constants.PromotionPlane, column, row] = 0;
            Field[Constants.EnPassantPlane, column, row] = 0;
        }

        internal void SetEnPassantMarking(Position position, int ply)
        {
            Field[Constants.EnPassantPlane, position.Column + 2, position.Row + 2] = ply;
        }

        public void New()
        {
            Field = new int[Constants.Planes, Constants.GridSize, Constants.GridSize];
            CurrentPly = 0;
            InitBorder();
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

        // Index 4: indices 0-3 hold the castling right flags
        public int ColorToMove
        {
            get => Field[Constants.LastPlyPlane, 0, 4];
            set => Field[Constants.LastPlyPlane, 0, 4] = value;
        }


        public Board Copy()
        {
            Board newBoard = new();
            newBoard.Field = (int[,,])Field.Clone();
            newBoard.CurrentPly = CurrentPly;

            return newBoard;
        }

        public bool CompareBoard(Board other)
        {

            for (int i = 2; i < 2+ ChessEngineConstants.Length; i++)
            for (int j = 2; j < 2+ ChessEngineConstants.Length; j++)
            {
                if (this.Field[Constants.BroadPlane,i, j] != other.Field[Constants.BroadPlane,i, j]) 
                    return false;
            }

            return true;
        }
    }
}
