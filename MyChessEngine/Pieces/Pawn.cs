﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using MyChessEngineBase;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Color = {Color} Position={Position} M={PossibleMoveType}")]
    public class Pawn : Piece
    {
        public MoveType PossibleMoveType { get; set; }

        public override MoveList GetThreatenMoveList()
        {
            MoveList moveList = new MoveList();
            if (Color == Color.White)
            {
                // beat left
                Position newPosition = Position.GetDeltaPosition(1, -1);
                AddPosition(moveList, newPosition);
               

                // beat right
                newPosition = Position.GetDeltaPosition(1, 1);
                AddPosition(moveList, newPosition);
            }
            else
            {
                // beat left
                Position newPosition = Position.GetDeltaPosition(-1, -1);
                AddPosition(moveList, newPosition);

                // beat right
                newPosition = Position.GetDeltaPosition(-1, 1);
                AddPosition(moveList, newPosition);
            }

            return moveList;
        }

        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            if (Color == Color.White)
            {
                // beat left
                Position newPosition = Position.GetDeltaPosition(-1, 1);
                if (newPosition != null)
                {
                    Piece pieceToBeat = Board[newPosition].Piece;
                    if ((pieceToBeat != null) && (pieceToBeat.Color != Color))
                        moveList.Add(new Move(Position, newPosition, this));
                }

                // up
                newPosition = Position.GetDeltaPosition(1, 0);
                if ((newPosition != null) && Board[newPosition].Piece == null)
                {
                    moveList.Add(new Move(Position, newPosition, this));
                    if (Position.Row == 1)
                    {
                        // pwn double step
                        Position newPosition2 = Position.GetDeltaPosition(2,0);
                        if ((newPosition2 != null) && Board[newPosition2].Piece == null)
                        {
                            moveList.Add(new Move(Position, newPosition2, this, MoveType.PawnDoubleStep));
                        }

                    }
                }

                // beat right
                newPosition = Position.GetDeltaPosition(1, 1);
                if (newPosition != null)
                {
                    Piece pieceToBeat = Board[newPosition].Piece;
                    if ((pieceToBeat != null) && (pieceToBeat.Color != Color))
                        moveList.Add(new Move(Position, newPosition, this));
                }

                // enpassant 

                if ((PossibleMoveType & MoveType.EnpassantWhiteLeft)>0)
                {
                    Move move = new Move(Position, Position.GetDeltaPosition(-1, +1), this,
                        MoveType.EnpassantBlackLeft);
                    move.EnpassantField = new Position(Position.Column -1, Position.Row);
                    moveList.Add(move);
                }

                if ((PossibleMoveType & MoveType.EnpassantWhiteRight)>0)
                {
                    Move move = new Move(Position, Position.GetDeltaPosition(1, +1), this,
                        MoveType.EnpassantBlackLeft);
                    move.EnpassantField = new Position(Position.Column + 1, Position.Row);
                    moveList.Add(move);
                }

            }
            else
            {
                // black
                // beat left
                Position newPosition = Position.GetDeltaPosition(-1, -1);
                if (newPosition != null)
                {
                    Piece pieceToBeat = Board[newPosition].Piece;
                    if ((pieceToBeat != null) && (pieceToBeat.Color != Color))
                        moveList.Add(new Move(Position, newPosition, this));
                }

                // down
                newPosition = Position.GetDeltaPosition(0, -1);
                if ((newPosition != null) && Board[newPosition].Piece == null)
                {
                    moveList.Add(new Move(Position, newPosition, this));
                    // start with two
                    if (Position.Row == 6)
                    {
                        Position newPosition2 = Position.GetDeltaPosition(0,-2);
                        if ((newPosition2 != null) && (Board[newPosition2].Piece == null))
                            moveList.Add(new Move(Position, newPosition2, this, MoveType.PawnDoubleStep));
                    }
                }

                // beat right
                newPosition = Position.GetDeltaPosition(-1, -1);
                if (newPosition != null)
                {
                    Piece pieceToBeat = Board[newPosition].Piece;
                    if ((pieceToBeat != null) && (pieceToBeat.Color != Color) )
                        moveList.Add(new Move(Position, newPosition, this));
                }
                // enpassant 

                if ((PossibleMoveType & MoveType.EnpassantBlackLeft) > 0)
                {
                    Move move =new Move(Position, Position.GetDeltaPosition(1, -1), this,
                        MoveType.EnpassantBlackLeft);
                    move.EnpassantField = new Position(Position.Column + 1, Position.Row);
                    moveList.Add(move);
                }

                if ((PossibleMoveType & MoveType.EnpassantBlackRight) > 0)
                {
                    Move move = new Move(Position, Position.GetDeltaPosition(-1, -1), this,
                        MoveType.EnpassantBlackRight);
                    move.EnpassantField = new Position(Position.Column - 1, Position.Row);
                    moveList.Add(move);
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
                Board[move.End].Piece = new Queen(Color, move.End);
                Board[move.End].Piece.Board = Board;
            }

            if (move.Type == MoveType.PawnDoubleStep)
            {
                if (Color == Color.White)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Position adjacentPawnPosition = new Position(move.End.Column + PossibleBlackEnpassants[i].Item1, move.End.Row);
                        if (adjacentPawnPosition.IsValidPosition())
                        {
                            if (Board[adjacentPawnPosition].Piece is Pawn {Color: Color.Black} adjacentPawn) adjacentPawn.PossibleMoveType |= PossibleBlackEnpassants[i].Item2;
                        }
                    }
                }
                else
                {

                    for (int i = 0; i < 2; i++)
                    {
                        Position adjacentPawnPosition = new Position(move.End.Column + PossibleWhiteEnpassants[i].Item1, move.End.Row);
                        if (adjacentPawnPosition.IsValidPosition())
                        {
                            if (Board[adjacentPawnPosition].Piece is Pawn {Color: Color.White} adjacentPawn) adjacentPawn.PossibleMoveType |= PossibleWhiteEnpassants[i].Item2;
                        }
                    }
                }
            }
            else
            {
                if (((PossibleMoveType & (MoveType.EnpassantWhiteLeft | MoveType.EnpassantWhiteRight)) & move.Type) > 0)
                {
                    Board[new Position(move.End.Column-1,move.End.Row)].Piece = null;
                    PossibleMoveType = MoveType.Normal;
                }

                if (((PossibleMoveType & (MoveType.EnpassantBlackLeft | MoveType.EnpassantBlackRight)) & move.Type) > 0)
                {
                    Board[new Position(move.End.Column + 1, move.End.Row)].Piece = null;
                    PossibleMoveType = MoveType.Normal;
                }
            }

            return base.ExecuteMove(move); 
        }

        public Pawn(Color color, Position position, MoveType possibleMoveType) : base(color, PieceType.Pawn, position)
        {
            PossibleMoveType = possibleMoveType;
        }

        public Pawn(Color color, Position position) : this(color, position, MoveType.Normal)
        {

        }

        public Pawn(Color color, string position, MoveType possibleMoveType) : base(color, PieceType.Pawn, new Position(position))
        {
            PossibleMoveType = possibleMoveType;
        }

        public Pawn(Color color, string position) : this(color, new Position(position), MoveType.Normal)
        {

        }
    }
}
