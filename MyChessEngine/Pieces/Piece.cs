using MyChessEngine.Interfaces;
using MyChessEngineBase;
using System;
using System.Diagnostics;
using System.Linq;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Color={Color} Position={Position}")]
    public abstract class Piece : IEnginePiece
    {
        #region IPiece

        public PieceType Type { get; }

        public Color Color { get; }

        public int LastPly { get; set; }

        public int PromotionPly { get; set; }

        public int LastEnPassantPlyMarking { get; set; }

        public bool Compare(IPiece other)
        {
            if (other == null)
                throw new Exception();

            if (Type != other.Type ||
                Color != other.Color ||
                LastPly != other.LastPly ||
                PromotionPly != other.PromotionPly ||
                LastEnPassantPlyMarking != other.LastEnPassantPlyMarking)
                return false;

            return true;
        }

        public bool IsMoved()
        {
            return LastPly > 0;
        }
        #endregion

        #region IEnginePiece
        public virtual MoveList GetMoveList()
        {
            return new MoveList();
        }

        public virtual MoveList GetThreatenMoveList()
        {
            return new MoveList();
        }

        public Board Board{ get; set; }
        Position IEnginePiece.Position
        {
            get => Position;
            set => Position = value;
        }

        public Position Position { get; set; }

        public virtual Piece Copy()
        {
            return Type switch
            {
                PieceType.Bishop => new Bishop(Color, Position, LastPly, PromotionPly),
                PieceType.Knight => new Knight(Color, Position, LastPly, PromotionPly),
                PieceType.Queen => new Queen(Color, Position, LastPly,PromotionPly),
                PieceType.Pawn => new Pawn(Color, Position, ((Pawn)this).PossibleMoveType, LastPly, LastEnPassantPlyMarking),
                PieceType.Rook => new Rook(Color, Position, LastPly,PromotionPly),
                PieceType.King => new King(Color, Position, ((King)this).KingMoves, LastPly),
            _ => null 
            };
        }

        public virtual bool ExecuteMove(Move move)
        {
            if (Board[move.End].Piece is King king)
                Board.Kings[king.Color] = null;
            if (move.AffectedPositionAfter[0] != null)
            {
                Board[move.AffectedPositionAfter[0]].Piece = (Piece)move.AffectedPieceAfter[0];
                ((Pawn) move.Piece).PossibleMoveType = MoveType.Normal;
            }

            Board[move.End].Piece = Board[move.Start].Piece;
            Board[move.Start].Piece = null;

            this.Position = move.End;
            this.LastPly = Board.Ply;

            return true;
        }

        public int Weight { get; }

        #endregion

        /// <summary>
        /// Add move from Positon to endPosition  to moveList.
        /// </summary>
        /// <param name="moveList">moves so far</param>
        /// <param name="endPosition">end position</param>
        /// <param name="threat"> true, if threat through king to investigate</param>
        /// <returns>returns false, if this is last move in that direction</returns>
        public bool AddPosition(MoveList moveList, Position endPosition, bool threat = false)
        {
            IsValidPositionReturns result = Board.IsValidPosition(endPosition, Color, threat);
            if (result == IsValidPositionReturns.NoPosition)
                return false;
            
            Move move = new Move(Position, endPosition, this);
			if (result == IsValidPositionReturns.EnemyBeatPosition)
            {
                move.AffectedPositionBefore[0] = endPosition;
                move.AffectedPieceBefore[0] = Board[endPosition].Piece;
			}
            
            moveList.Add(move);
            
            return (result != IsValidPositionReturns.EnemyBeatPosition);
        }
        
        

        public Piece(Color color, PieceType piece, Position position, int lastPly=-1, int promotionPly=-1, int lastEnPassantPlyMarking=-1)
        {
            Color = color;
            Type = piece;
            Position = position;
            PromotionPly = promotionPly;
            LastEnPassantPlyMarking = lastEnPassantPlyMarking;
            LastPly = lastPly;

            switch (Type)
            {
                case PieceType.Rook:
                    Weight = (Color == Color.White) ? ChessEngineConstants.Rook : -ChessEngineConstants.Rook;
                    break;

                case PieceType.Bishop:
                    Weight = (Color == Color.White) ? ChessEngineConstants.Bishop : -ChessEngineConstants.Bishop;
                    break;

                case PieceType.King:
                    Weight = (Color == Color.White) ? ChessEngineConstants.King : -ChessEngineConstants.King;
                    break;

                case PieceType.Knight:
                    Weight = (Color == Color.White) ? ChessEngineConstants.Knight : -ChessEngineConstants.Knight;
                    break;

                case PieceType.Pawn:
                    Weight = (Color == Color.White) ? ChessEngineConstants.Pawn : -ChessEngineConstants.Pawn;
                    break;

                case PieceType.Queen:
                    Weight = (Color == Color.White) ? ChessEngineConstants.Queen : -ChessEngineConstants.Queen;
                    break;

            }
        }
    }
}
