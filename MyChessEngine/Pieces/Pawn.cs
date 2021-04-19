using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color} PMT={PossibleMoveType}")]
    public class Pawn : Piece
    {
        public MoveType PossibleMoveType { get; set; } = MoveType.Normal;

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
                        if (this.Position.Row == 1)
                        {
                            Position newPosition2 = this.Position.GetDeltaPosition(2, 0);
                            if ((newPosition2 != null) && Board[newPosition2] == null)
                            {
                                moveList.Add(new Move(this.Position, newPosition2, this, MoveType.PawnDoubleStep));
                            }
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

                if (PossibleMoveType == MoveType.EnpasantWhiteLeft)
                {
                    moveList.Add(new Move(this.Position, this.Position.GetDeltaPosition(1, -1), this,
                        MoveType.EnpasantWhiteLeft));
                }

                if (PossibleMoveType == MoveType.EnpasantWhiteRight)
                {
                    moveList.Add(new Move(this.Position, this.Position.GetDeltaPosition(1, +1), this,
                        MoveType.EnpasantWhiteRight));
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

                if (PossibleMoveType == MoveType.EnpasantBlackLeft)
                {
                    moveList.Add(new Move(this.Position, this.Position.GetDeltaPosition(-1, +1), this,
                        MoveType.EnpasantBlackLeft));
                }

                if (PossibleMoveType == MoveType.EnpasantBlackRight)
                {
                    moveList.Add(new Move(this.Position, this.Position.GetDeltaPosition(-1, -1), this,
                        MoveType.EnpasantBlackRight));
                }
            }

            return moveList;
        }

        static readonly List<Tuple<int, MoveType>> possibleBlackEnpasants =
            new List<Tuple<int, MoveType>>()
            {
                new Tuple<int, MoveType>(-1, MoveType.EnpasantBlackLeft),
                new Tuple<int, MoveType>(1, MoveType.EnpasantBlackRight),
            };

        static readonly List<Tuple<int, MoveType>> possibleWhiteEnpasants =
            new List<Tuple<int, MoveType>>()
            {
                new Tuple<int, MoveType>(-1, MoveType.EnpasantWhiteRight),
                new Tuple<int, MoveType>(1, MoveType.EnpasantWhiteLeft),
            };


        public override bool ExecuteMove(Move move)
        {
            Board[move.End] = Board[move.Start];
            Board[move.Start] = null;
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
                        Position adjacentPawnPosition = new Position(move.End.Row, move.End.Column + possibleBlackEnpasants[i].Item1);
                        if (adjacentPawnPosition.IsValidPosition())
                        {
                            if (Board[adjacentPawnPosition] is Pawn adjacentPawn)
                            {
                                if (adjacentPawn.Color == Color.Black)
                                    adjacentPawn.PossibleMoveType = possibleBlackEnpasants[i].Item2;
                            }
                        }
                    }
                }
                else
                {

                    for (int i = 0; i < 2; i++)
                    {
                        Position adjacentPawnPosition = new Position(move.End.Row, move.End.Column + possibleWhiteEnpasants[i].Item1);
                        if (adjacentPawnPosition.IsValidPosition())
                        {
                            if (Board[adjacentPawnPosition] is Pawn adjacentPawn)
                            {
                                if (adjacentPawn.Color == Color.White)
                                    adjacentPawn.PossibleMoveType = possibleWhiteEnpasants[i].Item2;
                            }
                        }
                    }
                }
            }
            else
            {
                switch (move.Type)
                {
                    case MoveType.EnpasantWhiteLeft:
                    case MoveType.EnpasantWhiteRight:
                        Board[new Position(move.End.Row - 1, move.End.Column)] = null;
                        break;

                    case MoveType.EnpasantBlackLeft:
                    case MoveType.EnpasantBlackRight:
                        Board[new Position(move.End.Row + 1, move.End.Column)] = null;
                        break;

                }
            }

            return true;
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
