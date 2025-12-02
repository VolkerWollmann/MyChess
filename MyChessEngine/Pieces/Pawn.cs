using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
                // beat low
                Position newPosition = Position.GetDeltaPosition(-1, 1);
                if (newPosition != null)
                {
                    Piece pieceToBeat = Board[newPosition].Piece;
                    if ((pieceToBeat != null) && (pieceToBeat.Color != Color))
                    {
                        Move move = new Move(Position, newPosition, this);
                       
                        move.AffectedPositionBefore[0] = newPosition;
                        move.AffectedPieceBefore[0] = pieceToBeat;
                        moveList.Add(move);
                    }
                }

                // up
                newPosition = Position.GetDeltaPosition(0, 1);
                if ((newPosition != null) && Board[newPosition].Piece == null)
                {
                    moveList.Add(new Move(Position, newPosition, this));
                    if (Position.Row == 1)
                    {
                        // pawn double step
                        Position newPosition2 = Position.GetDeltaPosition(0,2);
                        if (Board[newPosition2].Piece == null)
                        {
                            Move move = new Move(Position, newPosition2, this, MoveType.PawnDoubleStep);
                            
                            Position low = newPosition2.GetDeltaPosition(-1, 0);
                            if ( low != null )
                            {
	                            var lowPawn =Board[low]?.Piece;

	                            if (lowPawn is { Type: PieceType.Pawn, Color: Color.Black })
	                            {
		                            move.AffectedPositionBefore[0] = low;
		                            move.AffectedPieceBefore[0] = lowPawn;

		                            move.AffectedPositionAfter[0] = low;
		                            Pawn pawn = (Pawn)lowPawn.Copy();
		                            pawn.LastEnPassantPlyMarking = this.Board.Ply;
		                            pawn.PossibleMoveType |= MoveType.EnpassantBlackHighRow;
		                            move.AffectedPieceAfter[0] = pawn;
	                            }
                            }

							Position high = newPosition2.GetDeltaPosition(1, 0);
							if (high != null)
							{
								var highPawn = Board[high]?.Piece;
								if (highPawn is { Type: PieceType.Pawn, Color: Color.Black })
								{
									move.AffectedPositionBefore[1] = high;
									move.AffectedPieceBefore[1] = highPawn;

									move.AffectedPositionAfter[1] = high;
									Pawn pawn = (Pawn)highPawn.Copy();
									pawn.LastEnPassantPlyMarking = this.Board.Ply;
									pawn.PossibleMoveType |= MoveType.EnpassantBlackLowRow;
									move.AffectedPieceAfter[1] = pawn;
                                }
							}

							moveList.Add(move);
                        }

                    }
                }

                // beat high
                newPosition = Position.GetDeltaPosition(1, 1);
                if (newPosition != null)
                {
	                Piece pieceToBeat = Board[newPosition].Piece;
					if ((pieceToBeat != null) && (pieceToBeat.Color != Color))
                    {
                        Move move = new Move(Position, newPosition, this);

                        move.AffectedPositionBefore[0] = newPosition;
                        move.AffectedPieceBefore[0] = pieceToBeat;
						moveList.Add(move);
					}
				}

                // enpassant 
                if (this.LastEnPassantPlyMarking + 1 == this.Board.Ply)
                {
                    Position low = Position.GetDeltaPosition(-1, 0);
                    if (low != null)
                    {
                        var lowPawn = Board[low]?.Piece;
                        if (lowPawn is { Type: PieceType.Pawn, Color: Color.Black })
                        {
                            Move move = new Move(Position, Position.GetDeltaPosition(-1, 1), this,
                                MoveType.EnpassantBlackLowRow);

                            move.AffectedPositionAfter[0] = low;
                            move.AffectedPieceAfter[0] = null;

                            move.AffectedPositionBefore[0] = low;
                            move.AffectedPieceBefore[0] = Board[low].Piece;
                            moveList.Add(move);
                        }
                    }

                    Position high = Position.GetDeltaPosition(+1, 0);
                    if (high != null)
                    {
                        var lowPawn = Board[high]?.Piece;
                        if (lowPawn is { Type: PieceType.Pawn, Color: Color.Black })
                        {
                            Move move = new Move(Position, Position.GetDeltaPosition(1, 1), this,
                                MoveType.EnpassantBlackLowRow);

                            move.AffectedPositionAfter[0] = high;
                            move.AffectedPieceAfter[0] = null;

                            move.AffectedPositionBefore[0] = high;
                            move.AffectedPieceBefore[0] = Board[high].Piece;
                            moveList.Add(move);
                        }
                    }
                }
            }
            else
            {
				#region black pawn
				// black
				// beat low
				Position newPosition = Position.GetDeltaPosition(-1, -1);
                if (newPosition != null)
                {
					Piece pieceToBeat = Board[newPosition].Piece;
					if ((pieceToBeat != null) && (pieceToBeat.Color != Color))
                    {
                        Move move = new Move(Position, newPosition, this);

                        move.AffectedPositionBefore[0] = newPosition;
                        move.AffectedPieceBefore[0] = pieceToBeat;

						moveList.Add(move);
					}
				}

                // down
                newPosition = Position.GetDeltaPosition(0, -1);
                if ((newPosition != null) && Board[newPosition].Piece == null)
                {
                    moveList.Add(new Move(Position, newPosition, this));
                    // start with two
                    if (Position.Row == 6)
                    {
                        Position newPosition2 = Position.GetDeltaPosition(0, -2);
                        if (Board[newPosition2].Piece == null)
                        {
                            Move move = new Move(Position, newPosition2, this, MoveType.PawnDoubleStep);

                            Position low = newPosition2.GetDeltaPosition(-1, 0);
                            if (low != null)
                            {
                                var lowPawn = Board[low]?.Piece;

                                if (lowPawn is {Type: PieceType.Pawn, Color: Color.White})
                                {
                                    move.AffectedPositionBefore[0] = low;
                                    move.AffectedPieceBefore[0] = lowPawn;

                                    move.AffectedPositionAfter[0] = low;
                                    Pawn pawn = (Pawn) lowPawn.Copy();
                                    pawn.LastEnPassantPlyMarking = this.Board.Ply;
                                    pawn.PossibleMoveType |= MoveType.EnpassantWhiteHighRow;
                                    move.AffectedPieceAfter[0] = pawn;
                                }
                            }

                            Position high = newPosition2.GetDeltaPosition(1, 0);
                            if (high != null)
                            {
                                var highPawn = Board[high]?.Piece;
                                if (highPawn is {Type: PieceType.Pawn, Color: Color.White})
                                {
                                    move.AffectedPositionBefore[1] = high;
                                    move.AffectedPieceBefore[1] = highPawn;

                                    move.AffectedPositionAfter[1] = high;
                                    Pawn pawn = (Pawn) highPawn.Copy();
                                    pawn.LastEnPassantPlyMarking = this.Board.Ply;
                                    pawn.PossibleMoveType |= MoveType.EnpassantWhiteLowRow;
                                    move.AffectedPieceAfter[1] = pawn;
                                }
                            }

                            moveList.Add(move);
                        }
                    }
                }

                // beat high
                newPosition = Position.GetDeltaPosition(1, -1);
                if (newPosition != null)
                {
					Piece pieceToBeat = Board[newPosition].Piece;
					if ((pieceToBeat != null) && (pieceToBeat.Color != Color))
                    {
                        Move move = new Move(Position, newPosition, this);

                        move.AffectedPositionBefore[0] = newPosition;
                        move.AffectedPieceBefore[0] = pieceToBeat;
						
						moveList.Add(move);
					}
				}
                // enpassant 

                if (this.LastEnPassantPlyMarking + 1 == this.Board.Ply)
                {
                    Position low = Position.GetDeltaPosition(-1, 0);
                    if ( low != null)
                    {
                        var lowPawn = Board[low]?.Piece;
                        if (lowPawn is { Type: PieceType.Pawn, Color: Color.White })
                        {
                            Move move = new Move(Position, Position.GetDeltaPosition(-1, -1), this,
                                MoveType.EnpassantBlackLowRow);

                            move.AffectedPositionAfter[0] = low;
                            move.AffectedPieceAfter[0] = null;

                            move.AffectedPositionBefore[0] = low;
                            move.AffectedPieceBefore[0] = Board[low].Piece;
                            moveList.Add(move);
                        }
                    }

                    Position high = Position.GetDeltaPosition(+1, 0);
                    if (high != null)
                    {
                        var lowPawn = Board[high]?.Piece;
                        if (lowPawn is { Type: PieceType.Pawn, Color: Color.White })
                        {
                            Move move = new Move(Position, Position.GetDeltaPosition(1, -1), this,
                                MoveType.EnpassantBlackLowRow);

                            move.AffectedPositionAfter[0] = high;
                            move.AffectedPieceAfter[0] = null;

                            move.AffectedPositionBefore[0] = high;
                            move.AffectedPieceBefore[0] = Board[high].Piece;
                            moveList.Add(move);
                        }
                    }
                }
				#endregion
			}

			return moveList;
        }

        static readonly List<Tuple<int, MoveType>> PossibleBlackEnpassants =
        [
	        new(-1, MoveType.EnpassantBlackLowRow),
	        new(1, MoveType.EnpassantBlackHighRow)
        ];

        static readonly List<Tuple<int, MoveType>> PossibleWhiteEnpassants =
        [
	        new(-1, MoveType.EnpassantWhiteHighRow),
	        new(1, MoveType.EnpassantWhiteLowRow)
        ];


        public override bool ExecuteMove(Move move)
        {
            if (move.End.Row == 7 || move.End.Row == 0)
            {
                // promotion
                Board[move.End].Piece = new Queen(Color, move.End, Board.Ply);
                Board[move.End].Piece.Board = Board;
            }

            return base.ExecuteMove(move); 
        }

        public Pawn(Color color, Position position, MoveType possibleMoveType, int lastPly, int lastEnPassantPlyMarking=-1, bool isMoved=false) : base(color, PieceType.Pawn, position, lastPly, lastEnPassantPlyMarking)
        {
            PossibleMoveType = possibleMoveType;
        }

        public Pawn(Color color, Position position, int lastPly=-1) : this(color, position, MoveType.Normal, lastPly)
        {

        }

        public Pawn(Color color, string position, MoveType possibleMoveType, int lastPly=-1) : base(color, PieceType.Pawn, new Position(position), lastPly)           
        {
            PossibleMoveType = possibleMoveType;
        }

        public Pawn(Color color, string position, int lastPly = -1) : this(color, new Position(position), lastPly)
        {

        }
    }
}
