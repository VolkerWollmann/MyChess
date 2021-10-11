using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MyChessEngineBase;
// ReSharper disable InconsistentNaming

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("T={Type}, C={Color} P={Position} M={KingMoves}")]
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


        List<Position> ThreatenedFields;

        private List<Position> GetThreatenedFields(Color color)
        {
            return Board.GetAllPieces(color)
                .Where(piece => (piece.Type != PieceType.King)) // King is very unlikely to threaten castle, avoid for recursion
                .Select((piece => piece.GetMoveList().Moves))
                .SelectMany(move => move).Select(move => move.End).ToList();
        }


        #region IEnginePiece

        static readonly int[,] Delta = new int[,]
        {
            { -1, -1 }, { -1, 0 }, {  -1, +1 }, 
            {  0, -1 },            {   0, +1 },
            { +1, -1 }, { +1, 0 }, {  +1, +1 }
        };
        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            
            for (int i=0; i< 8; i++)
            {
                    Position newPosition = Position.GetDeltaPosition(Delta[i, 0], Delta[i, 1]);
                    if (newPosition != null)
                        AddPosition(moveList, newPosition);
            }

            // Castle
            if (KingMoves != MoveType.Normal )
            {
                if (Color == Color.White)
                {
                    if ((KingMoves & MoveType.WhiteCastle) > 0)
                    {
                        if (WhiteCastleFields.All(field => Board[field] == null))
                        {
                            ThreatenedFields ??= GetThreatenedFields(Color.Black);
                            if (WhiteCastleFields.All(field => !ThreatenedFields.Contains(field)))
                                moveList.Add(new Move(WhiteKingField, WhiteKingKnightField, this, MoveType.WhiteCastle));
                        }
                    }

                    if ((KingMoves & MoveType.WhiteCastleLong) != 0)
                    {
                        if (WhiteLongCastleFields.All(field => Board[field] == null))
                        {
                            ThreatenedFields ??= GetThreatenedFields(Color.Black);
                            if (WhiteLongCastleFields.All(field => !ThreatenedFields.Contains(field)))
                                moveList.Add(new Move(WhiteKingField, WhiteQueenBishopField, this, MoveType.WhiteCastleLong));
                        }
                    }
                }
                else
                {
                    if ((KingMoves & MoveType.BlackCastle) != 0)
                    {
                        if (BlackCastleFields.All(field => Board[field] == null))
                        {
                            ThreatenedFields ??= GetThreatenedFields(Color.White);
                            if (BlackCastleFields.All(field => !ThreatenedFields.Contains(field)))
                                moveList.Add(new Move(BlackKingField, BlackKingKnightField, this, MoveType.BlackCastle));
                        }
                    }

                    if ((KingMoves & MoveType.BlackCastleLong) != 0)
                    {
                        if (BlackLongCastleFields.All(field => Board[field] == null))
                        {
                            ThreatenedFields ??= GetThreatenedFields(Color.White);
                            if (BlackLongCastleFields.All(field => !ThreatenedFields.Contains(field)))
                                moveList.Add(new Move(BlackKingField, BlackQueenBishopField, this, MoveType.BlackCastleLong));
                        }
                    }
                }
            }

            return moveList;
        }

        public override Piece Copy()
        {
            return new King(Color, KingMoves);
        }

        public override bool ExecuteMove(Move move)
        {
            switch (move.Type)
            {
                case MoveType.WhiteCastle:
                    Board["G1"] = this;
                    Board["E1"] = null;
                    Board["F1"] = Board["H1"];
                    Board["H1"] = null;
                    break;

                case MoveType.WhiteCastleLong:
                    Board["C1"] = this;
                    Board["E1"] = null;
                    Board["D1"] = Board["A1"];
                    Board["A1"] = null;
                    break;

                case MoveType.BlackCastle:
                    Board["G8"] = this;
                    Board["E8"] = null;
                    Board["F8"] = Board["H8"];
                    Board["H8"] = null;
                    break;

                case MoveType.BlackCastleLong:
                    Board["C8"] = this;
                    Board["E8"] = null;
                    Board["D8"] = Board["A8"];
                    Board["A8"] = null;
                    break;

                default:
                    base.ExecuteMove(move);
                    break;
            }

            KingMoves = MoveType.Normal;

            return true;
        }

        #endregion

        public King(Color color) : base(color, PieceType.King)
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

        private bool _IsCheckedCalculated = false;
        private bool _IsChecked = false;
        public bool IsChecked()
        {
            if (!_IsCheckedCalculated)
            {
                var l = Board.GetAllPieces(ChessEngineConstants.NextColorToMove(Color))
                    .Select((piece => piece.GetMoveList().Moves))
                    .SelectMany(move => move);

                var threatenedFields = l.Select(move => move.End);

                _IsChecked = threatenedFields.Any(position => position.AreEqual(Position));
                _IsCheckedCalculated = true;
            }

            return _IsChecked;
        }

        public void ResetIsChecked()
        {
            _IsCheckedCalculated = false;
            _IsChecked = false;
        }

        public King(Color color, MoveType kingMoves) : base(color,
            PieceType.King)
        {
            KingMoves = kingMoves;
        }
    }
}
