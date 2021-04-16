using System.Collections.Generic;
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
                PieceType.Pawn => new Pawn(Color),
                PieceType.Bishop => new Bishop(Color),
                PieceType.Knight => new Knight(Color),
                PieceType.Queen => new Queen(Color),
                PieceType.Rook => new Rook(Color),
                _ => null
            };
        }

        public virtual bool ExecuteMove(Move move)
        {
            Board[move.End] = Board[move.Start];
            Board[move.Start] = null;

            return true;
        }

        #endregion

        /// <summary>
        /// Adds position to to moves.
        /// </summary>
        /// <param name="moves">moves so far</param>
        /// <param name="position"></param>
        /// <returns>returns false, if this is last move in that direction</returns>
        public bool AddPosition(MoveList moveList, Position position)
        {
            if (!this.Board.IsValidPosition(position, this.Color))
                return false;

            moveList.Add(new Move(this.Position, position, this));
            if (this.Board[position] != null)
                return false;

            return true;
        }

        public Piece(Color color, PieceType piece)
        {
            Color = color;
            Type = piece;
        }


    }
}
