using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MyChess.Common;
using MyChessEngineCommon;

namespace MyChess.Model.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color}")]
    public class Rook : Piece
    {
        private bool HasMoved;

        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            // left
            for (int row = 1; row <= ChessEngineConstants.Length; row++)
            {
                Position newPosition = this.Position.GetDeltaPosition(-row, 0);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // right
            for (int row = 1; row <= ChessEngineConstants.Length; row++)
            {
                Position newPosition = this.Position.GetDeltaPosition(row, 0);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // down
            for (int column = 1; column <= ChessEngineConstants.Length; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(0, -column);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            // up
            for (int column = 1; column <= ChessEngineConstants.Length; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(0, column);
                if (!AddPosition(moves, newPosition))
                    break;
            }

            return moves;
        }

        public override bool ExecuteMove(Move move)
        {
            Board[move.End] = this;
            Board[move.Start] = null;

            if (!HasMoved)
            {
                HasMoved = true;
                if (Board.GetAllPieces(Color).First(piece => (piece.Type == ChessEngineConstants.Piece.King)) is King myKing)
                {
                    if ((this.Position.Row == 0) || (this.Position.Column == 0))
                        myKing.Rochades.Remove(ChessEngineConstants.MoveType.WhiteCastleLong);

                    if ((this.Position.Row == 0) || (this.Position.Column == 7))
                        myKing.Rochades.Remove(ChessEngineConstants.MoveType.WhiteCastle);

                    if ((this.Position.Row == 0) || (this.Position.Column == 0))
                        myKing.Rochades.Remove(ChessEngineConstants.MoveType.WhiteCastleLong);

                    if ((this.Position.Row == 0) || (this.Position.Column == 7))
                        myKing.Rochades.Remove(ChessEngineConstants.MoveType.WhiteCastle);
                }
            }

            return true;
        }

        public Rook(ChessEngineConstants.Color color) : base(color, ChessEngineConstants.Piece.Rook)
        {

        }
    }
}
