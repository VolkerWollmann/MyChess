using System.Diagnostics;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Piece : IEnginePiece
    {
        #region IPiece

        public PieceType Type { get; }

        public Color Color { get; }

        #endregion

        #region IEnginePiece
        public virtual MoveList GetMoveList()
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
                PieceType.Bishop => new Bishop(Color),
                PieceType.Knight => new Knight(Color),
                PieceType.Queen => new Queen(Color),
                PieceType.Pawn => new Pawn(Color, ((Pawn)this).PossibleMoveType),
                PieceType.Rook => new Rook(Color, ((Rook)this).HasMoved),
                PieceType.King => new King(Color, ((King)this).Rochades),
            _ => null 
            };
        }

        public virtual bool ExecuteMove(Move move)
        {
            Board[move.End] = Board[move.Start];
            Board[move.Start] = null;

            return true;
        }

        private int weight = 0;
        public int Weight
        {
            get { return weight; }
            private set { weight = value; }
        }

        #endregion

        /// <summary>
        /// Adds position to to moves.
        /// </summary>
        /// <param name="moveList">moves so far</param>
        /// <param name="position"></param>
        /// <returns>returns false, if this is last move in that direction</returns>
        public bool AddPosition(MoveList moveList, Position position)
        {
            IsValidPositionReturns result = this.Board.IsValidPosition(position, this.Color);
            if (result == IsValidPositionReturns.NoPosition)
                return false;

            moveList.Add(new Move(this.Position, position, this));
            if (result == IsValidPositionReturns.EnemyBeatPosition)
                return false;

            return true;
        }

        public Piece(Color color, PieceType piece)
        {
            Color = color;
            Type = piece;
            switch (Type)
            {
                case PieceType.Rook:
                    weight = (Color == Color.White) ? ChessEngineConstants.Rook : -ChessEngineConstants.Rook;
                    break;

                case PieceType.Bishop:
                    weight = (Color == Color.White) ? ChessEngineConstants.Bishop : -ChessEngineConstants.Bishop;
                    break;

                case PieceType.King:
                    weight = (Color == Color.White) ? ChessEngineConstants.King : -ChessEngineConstants.King;
                    break;

                case PieceType.Knight:
                    weight = (Color == Color.White) ? ChessEngineConstants.Knight : -ChessEngineConstants.Knight;
                    break;

                case PieceType.Pawn:
                    weight = (Color == Color.White) ? ChessEngineConstants.Pawn : -ChessEngineConstants.Pawn;
                    break;

                case PieceType.Queen:
                    weight = (Color == Color.White) ? ChessEngineConstants.Queen : -ChessEngineConstants.Queen;
                    break;

            }
        }


    }
}
