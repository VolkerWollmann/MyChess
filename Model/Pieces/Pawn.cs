using System.Collections.Generic;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    public class Pawn : Piece
    {
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            if (Color == ChessConstants.Color.White)
            {
                // beat left
                Position newPosition = this.Position.GetDeltaPosition(1, -1);
                if ( (newPosition != null ) && Board[newPosition] != null & Board[newPosition].Color != Color )
                    moves.Add(new Move(this.Position, newPosition));

                // up
                newPosition = this.Position.GetDeltaPosition(0, 1);
                if ((newPosition != null) && Board[newPosition] == null )
                    moves.Add(new Move(this.Position, newPosition));

                // beat right
                newPosition = this.Position.GetDeltaPosition(1, 1);
                if ((newPosition != null) && Board[newPosition] != null & Board[newPosition].Color != Color)
                    moves.Add(new Move(this.Position, newPosition));

                // start with two
                if (this.Position.Row == 1)
                {
                    newPosition = this.Position.GetDeltaPosition(0, 1);
                    Position newPosition2 = this.Position.GetDeltaPosition(0, 2);
                    if ((newPosition != null) && Board[newPosition] == null &&  
                        (newPosition2 != null) && Board[newPosition2] == null )
                    {
                        moves.Add(new Move(this.Position, newPosition));
                    }
                }

                // ToDo: en passant 

            }
            else
            {
                // beat left
                Position newPosition = this.Position.GetDeltaPosition(-1, -1);
                if ((newPosition != null) && Board[newPosition] != null & Board[newPosition].Color != Color)
                    moves.Add(new Move(this.Position, newPosition));

                // down
                newPosition = this.Position.GetDeltaPosition(0, -1);
                if ((newPosition != null) && Board[newPosition] == null)
                    moves.Add(new Move(this.Position, newPosition));

                // beat right
                newPosition = this.Position.GetDeltaPosition(1, -1);
                if ((newPosition != null) && Board[newPosition] != null & Board[newPosition].Color != Color)
                    moves.Add(new Move(this.Position, newPosition));

                // start with two
                if (this.Position.Row == 1)
                {
                    newPosition = this.Position.GetDeltaPosition(0, -1);
                    Position newPosition2 = this.Position.GetDeltaPosition(0, -2);
                    if ((newPosition != null) && Board[newPosition] == null &&
                        (newPosition2 != null) && Board[newPosition2] == null)
                    {
                        moves.Add(new Move(this.Position, newPosition));
                    }
                }

                // ToDo: en passant 
            }

            return moves;
        }
        public Pawn(ChessConstants.Color color): base(color, ChessConstants.Piece.Pawn)
        {

        }
    }
}
