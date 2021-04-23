using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("T={Type}, C = {Color} P={Position} M={PossibleMoveType}")]
    public class Pawn : Piece
    {
        public MoveType PossibleMoveType { get; set; }

        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            if (Color == Color.White)
            {
                // beat left
                Position newPosition = this.Position.GetDeltaPosition(1, -1);
                if (newPosition != null)
                {
                    Piece pieceToBeat = Board[newPosition];
                    if ((pieceToBeat != null) && (pieceToBeat.Color != Color))
                        moveList.Add(new Move(this.Position, newPosition, this));
                }

                // up
                newPosition = this.Position.GetDeltaPosition(1, 0);
                if ((newPosition != null) && Board[newPosition] == null)
                {
                    moveList.Add(new Move(this.Position, newPosition, this));
                    if (this.Position.Row == 1)
                    {
                        // pwn double step
                        Position newPosition2 = this.Position.GetDeltaPosition(2, 0);
                        if ((newPosition2 != null) && Board[newPosition2] == null)
                        {
                            moveList.Add(new Move(this.Position, newPosition2, this, MoveType.PawnDoubleStep));
                        }

                    }
                }

                // beat right
                newPosition = this.Position.GetDeltaPosition(1, 1);
                if (newPosition != null)
                {
                    Piece pieceToBeat = Board[newPosition];
                    if ((pieceToBeat != null) && (pieceToBeat.Color != Color))
                        moveList.Add(new Move(this.Position, newPosition, this));
                }

                // enpasant 

                if ((PossibleMoveType & MoveType.EnpassantWhiteLeft)>0)
                {
                    moveList.Add(new Move(this.Position, this.Position.GetDeltaPosition(1, -1), this,
                        MoveType.EnpassantWhiteLeft));
                }

                if ((PossibleMoveType & MoveType.EnpassantWhiteRight)>0)
                {
                    moveList.Add(new Move(this.Position, this.Position.GetDeltaPosition(1, +1), this,
                        MoveType.EnpassantWhiteRight));
                }

            }
            else
            {
                // beat left
                Position newPosition = this.Position.GetDeltaPosition(-1, -1);
                if (newPosition != null)
                {
                    Piece pieceToBeat = Board[newPosition];
                    if ((pieceToBeat != null) && (pieceToBeat.Color != Color))
                        moveList.Add(new Move(this.Position, newPosition, this));
                }

                // down
                newPosition = this.Position.GetDeltaPosition(-1, 0);
                if ((newPosition != null) && Board[newPosition] == null)
                {
                    moveList.Add(new Move(this.Position, newPosition, this));
                    // start with two
                    if (this.Position.Row == 6)
                    {
                        Position newPosition2 = this.Position.GetDeltaPosition(-2, 0);
                        if ((newPosition2 != null) && (Board[newPosition2] == null))
                            moveList.Add(new Move(this.Position, newPosition2, this, MoveType.PawnDoubleStep));
                    }
                }

                // beat right
                newPosition = this.Position.GetDeltaPosition(-1, 1);
                if (newPosition != null)
                {
                    Piece pieceToBeat = Board[newPosition];
                    if ((pieceToBeat != null) && (pieceToBeat.Color != Color) )
                        moveList.Add(new Move(this.Position, newPosition, this));
                }
                // enpasant 

                if ((PossibleMoveType & MoveType.EnpassantBlackLeft) > 0)
                {
                    moveList.Add(new Move(this.Position, this.Position.GetDeltaPosition(-1, +1), this,
                        MoveType.EnpassantBlackLeft));
                }

                if ((PossibleMoveType & MoveType.EnpassantBlackRight) > 0)
                {
                    moveList.Add(new Move(this.Position, this.Position.GetDeltaPosition(-1, -1), this,
                        MoveType.EnpassantBlackRight));
                }
            }

            return moveList;
        }

        static readonly List<Tuple<int, MoveType>> PossibleBlackEnpassants =
            new List<Tuple<int, MoveType>>()
            {
                new Tuple<int, MoveType>(-1, MoveType.EnpassantBlackLeft),
                new Tuple<int, MoveType>(1, MoveType.EnpassantBlackRight),
            };

        static readonly List<Tuple<int, MoveType>> PossibleWhiteEnpassants =
            new List<Tuple<int, MoveType>>()
            {
                new Tuple<int, MoveType>(-1, MoveType.EnpassantWhiteRight),
                new Tuple<int, MoveType>(1, MoveType.EnpassantWhiteLeft),
            };


        public override bool ExecuteMove(Move move)
        {
            if (move.End.Row == 7 || move.End.Row == 0)
            {
                // promotion
                Board[move.End] = new Queen(Color);
            }

            if (move.Type == MoveType.PawnDoubleStep)
            {
                if (Color == Color.White)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Position adjacentPawnPosition = new Position(move.End.Row, move.End.Column + PossibleBlackEnpassants[i].Item1);
                        if (adjacentPawnPosition.IsValidPosition())
                        {
                            if (Board[adjacentPawnPosition] is Pawn {Color: Color.Black} adjacentPawn) adjacentPawn.PossibleMoveType |= PossibleBlackEnpassants[i].Item2;
                        }
                    }
                }
                else
                {

                    for (int i = 0; i < 2; i++)
                    {
                        Position adjacentPawnPosition = new Position(move.End.Row, move.End.Column + PossibleWhiteEnpassants[i].Item1);
                        if (adjacentPawnPosition.IsValidPosition())
                        {
                            if (Board[adjacentPawnPosition] is Pawn {Color: Color.White} adjacentPawn) adjacentPawn.PossibleMoveType |= PossibleWhiteEnpassants[i].Item2;
                        }
                    }
                }
            }
            else
            {
                if (((PossibleMoveType & (MoveType.EnpassantWhiteLeft | MoveType.EnpassantWhiteRight)) & move.Type) > 0)
                {
                    Board[new Position(move.End.Row - 1, move.End.Column)] = null;
                    PossibleMoveType = MoveType.Normal;
                }

                if (((PossibleMoveType & (MoveType.EnpassantBlackLeft | MoveType.EnpassantBlackRight)) & move.Type) > 0)
                {
                    Board[new Position(move.End.Row + 1, move.End.Column)] = null;
                    Board.ClearAllPieces();
                    PossibleMoveType = MoveType.Normal;
                }
            }

            return base.ExecuteMove(move); ;
        }

        public Pawn(Color color, MoveType possibleMoveType) : base(color, PieceType.Pawn)
        {
            PossibleMoveType = possibleMoveType;
        }

        public Pawn(Color color): this(color, MoveType.Normal)
        {

        }
    }
}
