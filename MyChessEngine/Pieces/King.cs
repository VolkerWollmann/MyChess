using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MyChessEngineBase;
// ReSharper disable InconsistentNaming

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Color={Color} Position={Position} M={KingMoves}")]
    public class King : Piece
    {
        public MoveType KingMoves;

        static readonly Position WhiteKingField = new Position("E1");
        static readonly Position WhiteKingBishopField = new Position("F1");
        static readonly Position WhiteKingKnightField = new Position("G1");
        static readonly Position WhiteQueenField = new Position("D1");
        static readonly Position WhiteQueenBishopField = new Position("C1");
        static readonly Position WhiteQueenKnightField = new Position("B1");

        static readonly List<Position> WhiteCastleFields = new List<Position>() { WhiteKingField, WhiteKingBishopField, WhiteKingKnightField };
        static readonly List<Position> WhiteLongCastleFields = new List<Position>() { WhiteKingField, WhiteQueenField, WhiteQueenBishopField, WhiteQueenKnightField };

        static readonly Position BlackKingField = new Position("E8");
        static readonly Position BlackKingBishopField = new Position("F8");
        static readonly Position BlackKingKnightField = new Position("G8");
        static readonly Position BlackQueenField = new Position("D8");
        static readonly Position BlackQueenBishopField = new Position("C8");
        static readonly Position BlackQueenKnightField = new Position("B8");

        static readonly List<Position> BlackCastleFields = new List<Position>() { BlackKingField, BlackKingBishopField, BlackKingKnightField };
        static readonly List<Position> BlackLongCastleFields = new List<Position>() { BlackKingField, BlackQueenField, BlackQueenBishopField, BlackQueenKnightField };

        private static readonly MoveType WhiteKingInitialMoveTypes = MoveType.Normal | MoveType.WhiteCastle | MoveType.WhiteCastleLong;
        private static readonly MoveType BlackKingInitialMoveTypes = MoveType.Normal | MoveType.BlackCastle | MoveType.BlackCastleLong;
        #region IEnginePiece

		static readonly int[,] Delta = new int[,]
        {
            { -1, -1 }, { -1, 0 }, {  -1, +1 }, 
            {  0, -1 },            {   0, +1 },
            { +1, -1 }, { +1, 0 }, {  +1, +1 }
        };

        public override MoveList GetThreatenMoveList()
        {
            MoveList moveList = new MoveList();

            for (int i = 0; i < 8; i++)
            {
                Position newPosition = Position.GetDeltaPosition(Delta[i, 0], Delta[i, 1]);
                if (newPosition != null )
                    AddPosition(moveList, newPosition);
            }

            return moveList;
        }

        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            
            for (int i=0; i< 8; i++)
            {
                    Position newPosition = Position.GetDeltaPosition(Delta[i, 0], Delta[i, 1]);
                    if ((newPosition != null) && (!Board[newPosition].Threat))
                        AddPosition(moveList, newPosition);
            }

            // Castle
            if (KingMoves != MoveType.Normal )
            {
                if (Color == Color.White)
                {
                    if ((KingMoves & MoveType.WhiteCastle) > 0)
                    {
                        if (WhiteCastleFields.All(field => Board[field].Piece == null && !Board[field].Threat))
                        {
                            Move whiteCastle = new Move(WhiteKingField, WhiteKingKnightField, this,
                                MoveType.WhiteCastle);
                            
                            whiteCastle.AffectedPositionAfter[0] = new Position("F1");
                            whiteCastle.AffectedPieceAfter[0] = Board["H1"].Piece;

                            whiteCastle.AffectedPositionBefore[0] = new Position("H1");
                            whiteCastle.AffectedPieceBefore[0] = Board["H1"].Piece;

                            moveList.Add(whiteCastle);
                        }
                    }

                    if ((KingMoves & MoveType.WhiteCastleLong) != 0)
                    {
                        if (WhiteLongCastleFields.All(field => Board[field].Piece == null && !Board[field].Threat))
                        {
                            Move whiteCastleLong = new Move(WhiteKingField, WhiteQueenBishopField, this,
                                MoveType.WhiteCastleLong);
                            whiteCastleLong.AffectedPositionAfter[0] = new Position("D1");
                            whiteCastleLong.AffectedPieceAfter[0] = Board["A1"].Piece;
                            whiteCastleLong.AffectedPositionBefore[0] = new Position("A1");
                            whiteCastleLong.AffectedPieceBefore[0] = Board["A1"].Piece;

                            moveList.Add(whiteCastleLong);
                        }
                    }
                }
                else
                {
                    if (BlackCastleFields.All(field => Board[field].Piece == null && !Board[field].Threat))
                    {
                        Move blackCastle = new Move(BlackKingField, BlackKingKnightField, this, MoveType.BlackCastle);

                        blackCastle.AffectedPositionAfter[0] = new Position("F8");
                        blackCastle.AffectedPieceAfter[0] = Board["H8"].Piece;

                        blackCastle.AffectedPositionBefore[0] = new Position("H8");
                        blackCastle.AffectedPieceBefore[0] = Board["H8"].Piece;

                        moveList.Add(blackCastle);
                    }

                    if ((KingMoves & MoveType.BlackCastleLong) != 0)
                    {
                        if (BlackLongCastleFields.All(field => Board[field].Piece == null && !Board[field].Threat))
                        {
                            Move blackCastleLong = new Move(BlackKingField, BlackQueenBishopField, this, MoveType.BlackCastleLong);

                            blackCastleLong.AffectedPositionAfter[0] = new Position("D8");
                            blackCastleLong.AffectedPieceAfter[0] = Board["A8"].Piece;

                            blackCastleLong.AffectedPositionBefore[0] = new Position("A8");
                            blackCastleLong.AffectedPieceBefore[0] = Board["A8"].Piece;

                            moveList.Add(blackCastleLong);
                        }
                    }
                }
            }

            return moveList;
        }

        public override Piece Copy()
        {
            return new King(Color, Position, KingMoves, IsMoved);
        }

        public override bool ExecuteMove(Move move)
        {
            base.ExecuteMove(move);
            KingMoves = MoveType.Normal;

            return true;
        }

        #endregion

        public King(Color color, Position position, bool isMoved=true) : base(color, PieceType.King, position, isMoved)
        { 
            if (isMoved)
	            KingMoves = (color == Color.White) ? WhiteKingInitialMoveTypes : BlackKingInitialMoveTypes;
            else
                KingMoves = MoveType.Normal;
        }

        public King(Color color, string positionString, bool isMoved):
            this(color, new Position(positionString), isMoved) { }

        public bool IsChecked()
        {
            return Board[Position].Threat;
        }

        public King(Color color, string positionString, MoveType kingMoves, bool isMoved) : this(color, new Position(positionString), isMoved)
        {
            KingMoves = kingMoves;
        }

        public King(Color color, Position position, MoveType kingMoves, bool isMoved) : this(color, position, isMoved)
        {
            KingMoves = kingMoves;
        }
    }
}
