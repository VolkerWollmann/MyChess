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
                if (newPosition != null && !Board[newPosition].Threat)
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
                            moveList.Add(new Move(WhiteKingField, WhiteKingKnightField, this, MoveType.WhiteCastle));
                    }

                    if ((KingMoves & MoveType.WhiteCastleLong) != 0)
                    {
                        if (WhiteLongCastleFields.All(field => Board[field].Piece == null && !Board[field].Threat))
                            moveList.Add(new Move(WhiteKingField, WhiteQueenBishopField, this, MoveType.WhiteCastleLong));
                    }
                }
                else
                {
                    if ((KingMoves & MoveType.BlackCastle) != 0)
                    {
                        if (BlackCastleFields.All(field => Board[field].Piece == null && !Board[field].Threat))
                            moveList.Add(new Move(BlackKingField, BlackKingKnightField, this, MoveType.BlackCastle));
                    }

                    if ((KingMoves & MoveType.BlackCastleLong) != 0)
                    {
                        if (BlackLongCastleFields.All(field => Board[field].Piece == null && !Board[field].Threat))
                           moveList.Add(new Move(BlackKingField, BlackQueenBishopField, this, MoveType.BlackCastleLong));
                    }
                }
            }

            return moveList;
        }

        public override Piece Copy()
        {
            return new King(Color, Position, KingMoves);
        }

        public override bool ExecuteMove(Move move)
        {
            switch (move.Type)
            {
                case MoveType.WhiteCastle:
                    Board["G1"].Piece = this;
                    Board["E1"].Piece = null;
                    Board["F1"].Piece = Board["H1"].Piece;
                    Board["H1"].Piece = null;
                    break;

                case MoveType.WhiteCastleLong:
                    Board["C1"].Piece = this;
                    Board["E1"].Piece = null;
                    Board["D1"].Piece = Board["A1"].Piece;
                    Board["A1"].Piece = null;
                    break;

                case MoveType.BlackCastle:
                    Board["G8"].Piece = this;
                    Board["E8"].Piece = null;
                    Board["F8"].Piece = Board["H8"].Piece;
                    Board["H8"].Piece = null;
                    break;

                case MoveType.BlackCastleLong:
                    Board["C8"].Piece = this;
                    Board["E8"].Piece = null;
                    Board["D8"].Piece = Board["A8"].Piece;
                    Board["A8"].Piece = null;
                    break;

                default:
                    base.ExecuteMove(move);
                    break;
            }

            KingMoves = MoveType.Normal;

            return true;
        }

        #endregion

        public King(Color color, Position position) : base(color, PieceType.King, position)
        {
            if (color == Color.White)
            {
                KingMoves = MoveType.Normal |MoveType.WhiteCastle | MoveType.WhiteCastleLong;
            }
            else
            {
                KingMoves = MoveType.Normal | MoveType.BlackCastle | MoveType.BlackCastleLong;
            }
        }

        public King(Color color, string positionString):
            this(color, new Position(positionString)) { }

        private bool _IsChecked = false;
        public bool IsChecked()
        {
            var l = Board.GetAllPieces(ChessEngineConstants.NextColorToMove(Color))
                .Select((piece => piece.GetThreatenMoveList().Moves))
                .SelectMany(move => move);

            var threatenedFields = l.Select(move => move.End);

            _IsChecked = threatenedFields.Any(position => position.AreEqual(Position));

            return _IsChecked;
        }

        public King(Color color, string positionString, MoveType kingMoves) : this(color, new Position(positionString))
        {
            KingMoves = kingMoves;
        }

        public King(Color color, Position position, MoveType kingMoves) : this(color, position)
        {
            KingMoves = kingMoves;
        }
    }
}
