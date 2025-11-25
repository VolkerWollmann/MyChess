using System.Diagnostics;
using System.Linq;
using MyChessEngine.Interfaces;
using MyChessEngineBase;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Color={Color} Position={Position}")]
    public abstract class Piece : IEnginePiece
    {
        #region IPiece

        public PieceType Type { get; }

        public Color Color { get; }
        public bool IsMoved { get; private set; } = false;
        public int PromotionPly { get; set; }

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
                PieceType.Bishop => new Bishop(Color, Position, PromotionPly),
                PieceType.Knight => new Knight(Color, Position, PromotionPly),
                PieceType.Queen => new Queen(Color, Position, PromotionPly),
                PieceType.Pawn => new Pawn(Color, Position, ((Pawn)this).PossibleMoveType),
                PieceType.Rook => new Rook(Color, Position, IsMoved, PromotionPly),
                PieceType.King => new King(Color, Position, ((King)this).KingMoves, IsMoved),
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

            this.IsMoved = true;

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
            else
                moveList.Add(move);
            
            return (result != IsValidPositionReturns.EnemyBeatPosition);
        }
        
        

        public Piece(Color color, PieceType piece, Position position, bool isMoved, int promotionPly )
        {
            Color = color;
            Type = piece;
            Position = position;
            IsMoved = isMoved;
            PromotionPly = promotionPly;

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

        public Piece(Color color, PieceType piece, string positionString, bool isMoved, int promotionPly) :
            this( color, piece, new Position(positionString), isMoved, promotionPly)
        {
        }

    }
}
