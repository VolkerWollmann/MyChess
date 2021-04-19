using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Color = {Color} Postion={Position} Rochades={Rochades}")]
    public class King : Piece
    {
        public MoveType Rochades;

        static readonly Position _whiteKingBishopField = new Position("F1");
        static readonly Position _whiteKingKnightField = new Position("G1");
        static readonly Position _whiteQueenField = new Position("D1");
        static readonly Position _whiteQueenBishopField = new Position("C1");
        static readonly Position _whiteQueenKnightField = new Position("B1");

        static readonly Position _blackKingBishopField = new Position("F8");
        static readonly Position _blackKingKnightField = new Position("G8");
        static readonly Position _blackQueenField = new Position("D8");
        static readonly Position _blackQueenBishopField = new Position("C8");
        static readonly Position _blackQueenKnightField = new Position("B8");

        #region IEnginePiece
        public override MoveList GetMoveList()
        {
            List<Position> threatenedFields = null;
            
            MoveList moveList = new MoveList();

            for (int row = -1; row <= 1; row++)
                for (int column = -1; column <= 1; column++)
                {
                    Position newPosition = this.Position.GetDeltaPosition(row, column);
                    if (newPosition != null)
                        AddPosition(moveList, newPosition);
                }

            // Castle

            if (Rochades != MoveType.Normal )
            {
                if (Color == Color.White)
                {
                    if ((Rochades & MoveType.WhiteCastle) > 0)
                    {
                        if ((Board[_whiteKingBishopField] == null) && (Board[_whiteKingKnightField] == null))
                        {

                            if (threatenedFields == null)
                            {
                                threatenedFields = this.Board.GetAllPieces(Color.Black)
                                    .Where(piece =>
                                        (piece.Type !=
                                         PieceType.King)) // King cannot threaten castle, avoid for recursion
                                    .Select((piece => piece.GetMoveList().Moves))
                                    .SelectMany(move => move).Select(move => move.End).ToList();
                            }

                            bool thread = false;
                            for (int i = 4; i < 7; i++)
                            {
                                Position field = new Position(0, i);
                                thread = threatenedFields.Contains(field);
                                if (thread)
                                    break;
                            }

                            if (!thread)
                                moveList.Add(new Move(this.Position, new Position("G1"), this, MoveType.WhiteCastle));
                        }
                    }

                    if ((Rochades & MoveType.WhiteCastleLong) != 0)
                    {
                        if ((Board[_whiteQueenField] == null) && (Board[_whiteQueenBishopField] == null) && (Board[_whiteQueenKnightField] == null))
                        {
                            if (threatenedFields == null)
                            {
                                threatenedFields = this.Board.GetAllPieces(Color.Black)
                                    .Where(piece =>
                                        (piece.Type !=
                                         PieceType.King)) // King cannot threaten castle, avoid for recursion
                                    .Select((piece => piece.GetMoveList().Moves))
                                    .SelectMany(move => move).Select(move => move.End).ToList();
                            }

                            bool thread = false;
                            for (int i = 1; i <= 5; i++)
                            {
                                Position field = new Position(0, i);
                                thread = threatenedFields.Contains(field);
                                if (thread)
                                    break;
                            }

                            if ((!thread) )
                                moveList.Add(new Move(this.Position, new Position("C1"), this, MoveType.WhiteCastleLong));
                        }
                    }
                }
                else
                {
                    if ((Rochades & MoveType.BlackCastle) != 0)
                    {
                        if ((Board[_blackKingBishopField] == null) && (Board[_blackKingKnightField] == null))
                        {
                            if (threatenedFields == null)
                            {
                                threatenedFields = this.Board.GetAllPieces(Color.White)
                                    .Where(piece =>
                                        (piece.Type !=
                                         PieceType.King)) // King cannot threaten castle, avoid for recursion
                                    .Select((piece => piece.GetMoveList().Moves))
                                    .SelectMany(move => move).Select(move => move.End).ToList();
                            }

                            bool thread = false;
                            for (int i = 4; i < 7; i++)
                            {
                                Position field = new Position(7, i);
                                thread = threatenedFields.Contains(field);
                                if (thread)
                                    break;
                            }

                            if ((!thread))
                                moveList.Add(new Move(this.Position, new Position("G8"), this, MoveType.BlackCastle));
                        }
                    }

                    if ((Rochades & MoveType.BlackCastleLong) != 0)
                    {
                        if ((Board[_blackQueenField] == null) && (Board[_blackQueenBishopField] == null) && (Board[_blackQueenKnightField] == null))
                        {
                            bool thread = false;
                            for (int i = 1; i <= 5; i++)
                            {
                                if(threatenedFields == null)
                                {
                                    threatenedFields = this.Board.GetAllPieces(Color.White)
                                        .Where(piece =>
                                            (piece.Type !=
                                             PieceType.King)) // King cannot threaten castle, avoid for recursion
                                        .Select((piece => piece.GetMoveList().Moves))
                                        .SelectMany(move => move).Select(move => move.End).ToList();
                                }

                                Position field = new Position(7, i);
                                thread = threatenedFields.Contains(field);
                                if (thread)
                                    break;
                            }

                            if ((!thread))
                                moveList.Add(new Move(this.Position, new Position("C8"), this, MoveType.BlackCastleLong));
                        }
                    }
                }

            }

            return moveList;
        }

        public override Piece Copy()
        {
            return new King(Color, Rochades);
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
                    Board[move.End] = this;
                    Board[move.Start] = null;
                    break;
            }

            Rochades = MoveType.Normal;

            return true;
        }

        #endregion

        public King(Color color) : base(color, PieceType.King)
        {
            if (color == Color.White)
            {
                Rochades = MoveType.Normal |MoveType.WhiteCastle | MoveType.WhiteCastleLong;
            }
            else
            {
                Rochades = MoveType.Normal | MoveType.BlackCastle | MoveType.BlackCastleLong;
            }
        }

        public bool IsChecked()
        {
            var l = Board.GetAllPieces(ChessEngineConstants.NextColorToMove(Color))
                .Select((piece => piece.GetMoveList().Moves))
                .SelectMany(move => move);

            var threatenedFields = l.Select(move => move.End).ToList();

            bool result = threatenedFields.Any(position => position.AreEqual(Position));

            return result;
        }

        public King(Color color, MoveType rochades) : base(color,
            PieceType.King)
        {
            Rochades = rochades;
        }
    }
}
