using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Color = {Color} Postion={Position} Rochades={Rochades}")]
    public class King : Piece
    {
        public List<MoveType> Rochades;

        static Position _whiteKingBishopField = new Position("F1");
        static Position _whiteKingKnightField = new Position("G1");
        static Position _whiteQueenField = new Position("D1");
        static Position _whiteQueenBishopField = new Position("C1");
        static Position _whiteQueenKnightField = new Position("B1");

        static Position _blackKingBishopField = new Position("F8");
        static Position _blackKingKnightField = new Position("G8");
        static Position _blackQueenField = new Position("D8");
        static Position _blackQueenBishopField = new Position("C8");
        static Position _blackQueenKnightField = new Position("B8");

        #region IEnginePiece
        public override MoveList GetMoveList()
        {
            MoveList moveList = new MoveList();

            for (int row = -1; row <= 1; row++)
                for (int column = -1; column <= 1; column++)
                {
                    Position newPosition = this.Position.GetDeltaPosition(row, column);
                    AddPosition(moveList, newPosition);
                }

            // Castle

            if (Rochades.Any())
            {
                if (Color == Color.White)
                {
                    if ((Board[_whiteKingBishopField] == null) && (Board[_whiteKingKnightField] == null))
                    {

                        var threatenedFields = this.Board.GetAllPieces(Color.Black)
                            .Where(piece => (piece.Type != PieceType.King))             // King cannot threaten castle, avoid for recursion
                            .Select((piece => piece.GetMoveList().Moves))
                            .SelectMany(move => move).Select(move => move.End).ToList();

                        if (Rochades.Contains(MoveType.WhiteCastle))
                        {
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

                    if (Rochades.Contains(MoveType.WhiteCastleLong))
                    {
                        if ((Board[_whiteQueenField] == null) && (Board[_whiteQueenBishopField] == null) && (Board[_whiteQueenKnightField] == null))
                        {
                            var threatenedFields = this.Board.GetAllPieces(Color.Black)
                                .Where(piece =>
                                    (piece.Type != PieceType.King)) // King cannot threaten castle, avoid for recursion
                                .Select((piece => piece.GetMoveList().Moves))
                                .SelectMany(move => move).Select(move => move.End).ToList();

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
                    if ((Board[_blackKingBishopField] == null) && (Board[_blackKingKnightField] == null))
                    {
                        var threatenedFields = this.Board.GetAllPieces(Color.White)
                        .Where(piece => (piece.Type != PieceType.King))             // King cannot threaten castle, avoid for recursion
                        .Select((piece => piece.GetMoveList().Moves))
                        .SelectMany(move => move).Select(move => move.End).ToList();

                        if (Rochades.Contains(MoveType.BlackCastle))
                        {
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

                    if (Rochades.Contains(MoveType.BlackCastleLong))
                    {
                        if ((Board[_blackQueenField] == null) && (Board[_blackQueenBishopField] == null) && (Board[_blackQueenKnightField] == null))
                        {
                            bool thread = false;
                            for (int i = 1; i <= 5; i++)
                            {
                                var threatenedFields = this.Board.GetAllPieces(Color.White)
                                    .Where(piece =>
                                        (piece.Type !=
                                         PieceType.King)) // King cannot threaten castle, avoid for recursion
                                    .Select((piece => piece.GetMoveList().Moves))
                                    .SelectMany(move => move).Select(move => move.End).ToList();

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

            Rochades = new List<MoveType>();

            return true;
        }

        #endregion

        public King(Color color) : base(color, PieceType.King)
        {
            if (color == Color.White)
            {
                Rochades = new List<MoveType>()
                    {MoveType.WhiteCastle, MoveType.WhiteCastleLong};
            }
            else
            {
                Rochades = new List<MoveType>()
                    {MoveType.BlackCastle, MoveType.BlackCastleLong};
            }
        }

        public override int GetWeight()
        {
            return (Color == Color.White) ? ChessEngineConstants.King : -ChessEngineConstants.King;
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

        public King(Color color, List<MoveType> rochades) : base(color,
            PieceType.King)
        {
            Rochades = rochades;
        }
    }
}
