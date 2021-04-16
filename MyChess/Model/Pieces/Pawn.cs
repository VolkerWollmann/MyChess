using System;
using System.Collections.Generic;
using System.Diagnostics;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color} PMT={PossibleMoveType}")]
    public class Pawn : Piece
    {
        public ChessConstants.MoveType PossibleMoveType { get; set; } = ChessConstants.MoveType.Normal;

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

                // pwn double step
                if (this.Position.Row == 1)
                {
                    newPosition = this.Position.GetDeltaPosition(1, 0);
                    Position newPosition2 = this.Position.GetDeltaPosition(2, 0);
                    if ((newPosition != null) && Board[newPosition] == null &&  
                        (newPosition2 != null) && Board[newPosition2] == null )
                    {
                        moves.Add(new Move(this.Position, newPosition2, this, ChessConstants.MoveType.PawnDoubleStep));
                    }
                }

                // enpasant 

                if (PossibleMoveType == ChessConstants.MoveType.EnpasantWhiteLeft)
                {
                    moves.Add(new Move(this.Position, this.Position.GetDeltaPosition(1, -1), this,
                        ChessConstants.MoveType.EnpasantWhiteLeft));
                }

                if (PossibleMoveType == ChessConstants.MoveType.EnpasantWhiteRight)
                {
                    moves.Add(new Move(this.Position, this.Position.GetDeltaPosition(1, +1), this,
                        ChessConstants.MoveType.EnpasantWhiteRight));
                }

            }
            else
            {
                // beat left
                Position newPosition = this.Position.GetDeltaPosition(-1, -1);
                if ((newPosition != null) && (Board[newPosition] != null) && Board[newPosition].Color != Color)
                    moves.Add(new Move(this.Position, newPosition, this));

                // down
                newPosition = this.Position.GetDeltaPosition(-1, 0);
                if ((newPosition != null) && Board[newPosition] == null)
                    moves.Add(new Move(this.Position, newPosition, this));

                // beat right
                newPosition = this.Position.GetDeltaPosition(-1, 1);
                if ((newPosition != null) && (Board[newPosition] != null) && Board[newPosition].Color != Color)
                    moves.Add(new Move(this.Position, newPosition, this));

                // start with two
                if (this.Position.Row == 6)
                {
                    newPosition = this.Position.GetDeltaPosition(-1, 0);
                    Position newPosition2 = this.Position.GetDeltaPosition(-2, 0);
                    if ((newPosition != null) && Board[newPosition] == null &&
                        (newPosition2 != null) && Board[newPosition2] == null)
                    {
                        moves.Add(new Move(this.Position, newPosition2, this, ChessConstants.MoveType.PawnDoubleStep));
                    }
                }

                // enpasant 

                if (PossibleMoveType == ChessConstants.MoveType.EnpasantBlackLeft)
                {
                    moves.Add(new Move(this.Position, this.Position.GetDeltaPosition(-1, +1), this,
                        ChessConstants.MoveType.EnpasantBlackLeft));
                }

                if (PossibleMoveType == ChessConstants.MoveType.EnpasantBlackRight)
                {
                    moves.Add(new Move(this.Position, this.Position.GetDeltaPosition(-1, -1), this,
                        ChessConstants.MoveType.EnpasantWhiteRight));
                }
            }

            return moves;
        }

        public override bool ExecuteMove(Move move)
        {
            Board[move.End] = Board[move.Start];
            Board[move.Start] = null;
            if (move.End.Row == 7 || move.End.Row == 0)
            {
                // promotion
                Board[move.End] = new Queen(Color);
            }

            if (move.Type == ChessConstants.MoveType.PawnDoubleStep)
            {
                if (Color == ChessConstants.Color.White)
                {
                    List<Tuple<int, ChessConstants.MoveType>> mmx =
                        new List<Tuple<int, ChessConstants.MoveType>>()
                        {
                            new Tuple<int, ChessConstants.MoveType>(-1, ChessConstants.MoveType.EnpasantBlackRight),
                            new Tuple<int, ChessConstants.MoveType>(1, ChessConstants.MoveType.EnpasantBlackLeft),
                        };

                    for (int i = 0; i < 2; i++)
                    {
                        Position adjacentPawnPosition = new Position(move.End.Row, move.End.Column + mmx[i].Item1);
                        if (adjacentPawnPosition.IsValidPosition())
                        {
                            if (Board[adjacentPawnPosition] is Pawn adjacentPawn)
                            {
                                if (adjacentPawn.Color == ChessConstants.Color.Black)
                                    adjacentPawn.PossibleMoveType = mmx[i].Item2;
                            }
                        }
                    }
                }
                else
                {
                    List<Tuple<int, ChessConstants.MoveType>> mmx =
                        new List<Tuple<int, ChessConstants.MoveType>>()
                        {
                            new Tuple<int, ChessConstants.MoveType>(-1, ChessConstants.MoveType.EnpasantWhiteLeft),
                            new Tuple<int, ChessConstants.MoveType>(1, ChessConstants.MoveType.EnpasantWhiteRight),
                        };

                    for (int i = 0; i < 2; i++)
                    {
                        Position adjacentPawnPosition = new Position(move.End.Row, move.End.Column + mmx[i].Item1);
                        if (adjacentPawnPosition.IsValidPosition())
                        {
                            if (Board[adjacentPawnPosition] is Pawn adjacentPawn)
                            {
                                if (adjacentPawn.Color == ChessConstants.Color.White)
                                    adjacentPawn.PossibleMoveType = mmx[i].Item2;
                            }
                        }
                    }
                }
            }
            else
            {
                switch (move.Type)
                {
                    case ChessConstants.MoveType.EnpasantWhiteLeft:
                    case ChessConstants.MoveType.EnpasantWhiteRight:
                        Board[new Position(move.End.Row - 1, move.End.Column)] = null;
                        break;

                    case ChessConstants.MoveType.EnpasantBlackLeft:
                    case ChessConstants.MoveType.EnpasantBlackRight:
                        Board[new Position(move.End.Row + 1, move.End.Column)] = null;
                        break;

                }
            }

            return true;
        }

        public Pawn(ChessConstants.Color color): base(color, ChessConstants.Piece.Pawn)
        {

        }
    }
}
