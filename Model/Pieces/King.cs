using System.Collections.Generic;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    public class King : Piece
    {
        private List<ChessConstants.MoveType> Rochades;

        #region IEnginePiece
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            for (int row = -1; row <= 1; row++)
            for (int column = -1; column <= 1; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(row, column);

                if (this.Board.IsValidPosition(newPosition, this.Color))
                    moves.Add(new Move(this.Position, newPosition, this));
            }

            // ToDo : Castle

            return moves;
        }

        public override Piece Copy()
        {
            return new King(Color, Rochades);
        }

        public override bool ExecuteMove(Move move)
        {
            Board[move.End] = this;
            Board[move.Start] = null;
            Rochades = new List<ChessConstants.MoveType>() { };
            
            return true;
        }

        #endregion

        public King(ChessConstants.Color color) : base(color, ChessConstants.Piece.King)
        {
            if (color == ChessConstants.Color.White)
            {
                Rochades = new List<ChessConstants.MoveType>()
                    {ChessConstants.MoveType.WhiteCastle, ChessConstants.MoveType.WhiteCastleLong};
            }
            else
            {
                Rochades = new List<ChessConstants.MoveType>()
                    {ChessConstants.MoveType.BlackCastle, ChessConstants.MoveType.BlackCastleLong};
            }
        }

        public King(ChessConstants.Color color, List<ChessConstants.MoveType> rochades) : base(color,
            ChessConstants.Piece.King)
        {
            Rochades = rochades;
        }
    }
}
