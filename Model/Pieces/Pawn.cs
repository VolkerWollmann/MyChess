using System;
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
                if ( (newPosition != null ) && Board[newPosition] != null && Board[newPosition].Color != Color )
                    moves.Add(new Move(this.Position, newPosition, this));

                // up
                newPosition = this.Position.GetDeltaPosition(1, 0);
                if ((newPosition != null) && Board[newPosition] == null )
                    moves.Add(new Move(this.Position, newPosition, this));

                // beat right
                newPosition = this.Position.GetDeltaPosition(1, 1);
                if ((newPosition != null) && Board[newPosition] != null && Board[newPosition].Color != Color)
                    moves.Add(new Move(this.Position, newPosition, this));

                // start with two
                if (this.Position.Row == 1)
                {
                    newPosition = this.Position.GetDeltaPosition(1, 0);
                    Position newPosition2 = this.Position.GetDeltaPosition(2, 0);
                    if ((newPosition != null) && Board[newPosition] == null &&  
                        (newPosition2 != null) && Board[newPosition2] == null )
                    {
                        moves.Add(new Move(this.Position, newPosition, this));
                    }
                }

                // ToDo: en passant 

            }
            else
            {
                // beat left
                Position newPosition = this.Position.GetDeltaPosition(-1, -1);
                if ((newPosition != null) && Board[newPosition] != null & Board[newPosition].Color != Color)
                    moves.Add(new Move(this.Position, newPosition, this));

                // down
                newPosition = this.Position.GetDeltaPosition(-1, 0);
                if ((newPosition != null) && Board[newPosition] == null)
                    moves.Add(new Move(this.Position, newPosition, this));

                // beat right
                newPosition = this.Position.GetDeltaPosition(-1, 1);
                if ((newPosition != null) && Board[newPosition] != null & Board[newPosition].Color != Color)
                    moves.Add(new Move(this.Position, newPosition, this));

                // start with two
                if (this.Position.Row == 6)
                {
                    newPosition = this.Position.GetDeltaPosition(-1, 0);
                    Position newPosition2 = this.Position.GetDeltaPosition(-2, 0);
                    if ((newPosition != null) && Board[newPosition] == null &&
                        (newPosition2 != null) && Board[newPosition2] == null)
                    {
                        moves.Add(new Move(this.Position, newPosition, this));
                    }
                }

                // ToDo: en passant 
            }

            return moves;
        }

        public override bool ExecuteMove(Move move)
        {
            if (move.Type == ChessConstants.MoveType.Enpasant)
            {
                throw new NotImplementedException("Pawn move enpassant");
            }
            else
            {
                Board[move.End] = Board[move.Start];
                if (move.End.Row == 7 || move.End.Row == 0 )
                {
                    // promotion
                    Board[move.End] = new Queen(Color);
                }
            }

            return true;
        }

        public Pawn(ChessConstants.Color color): base(color, ChessConstants.Piece.Pawn)
        {

        }
    }
}
