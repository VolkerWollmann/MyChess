using System.Collections.Generic;
using System.Diagnostics;
using MyChess.Common;
using MyChessEngineCommon;

namespace MyChess.Model.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Piece : IEnginePiece
    {
        #region IPiece

        public ChessEngineConstants.Piece Type { get; }

        public ChessEngineConstants.Color Color { get; }

        #endregion

        #region IEnginePiece
        public virtual List<Move> GetMoves()
        {
            return new List<Move>();
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
                ChessEngineConstants.Piece.Pawn => new Pawn(Color),
                ChessEngineConstants.Piece.Bishop => new Bishop(Color),
                ChessEngineConstants.Piece.Knight => new Knight(Color),
                ChessEngineConstants.Piece.Queen => new Queen(Color),
                ChessEngineConstants.Piece.Rook => new Rook(Color),
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
        public bool AddPosition(List<Move> moves, Position position)
        {
            if (!this.Board.IsValidPosition(position, this.Color))
                return false;

            moves.Add(new Move(this.Position, position, this));
            if (this.Board[position] != null)
                return false;

            return true;
        }

        public Piece(ChessEngineConstants.Color color, ChessEngineConstants.Piece piece)
        {
            Color = color;
            Type = piece;
        }


    }
}
