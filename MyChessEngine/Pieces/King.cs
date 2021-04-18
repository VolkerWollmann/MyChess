using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Color = {Color} Postion={Position} Rochades={Rochades}")]
    public class King : Piece
    {
        public List<MoveType> Rochades;

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
                    Position kingBishopField = new Position("F1");
                    Position kingKnightField = new Position("G1");
                    Position queenField = new Position("D1");
                    Position queenBishopField = new Position("C1");
                    Position queenKnightField = new Position("B1");

                    var threatenedFields = this.Board.GetAllPieces(Color.Black)
                        .Where( piece => (piece.Type != PieceType.King))             // King cannot threaten castle, avoid for recursion
                        .Select((piece => piece.GetMoveList().Moves))
                        .SelectMany(move => move).Select(move => move.End).ToList();

                    if (Rochades.Contains(MoveType.WhiteCastle))
                    {
                        bool thread=false;
                        for (int i = 4; i < 7; i++)
                        {
                            Position field = new Position(0, i);
                            thread = threatenedFields.Contains(field);
                            if (thread)
                                break;
                        }

                        if ((!thread) && ((Board[kingBishopField] == null) && (Board[kingKnightField] == null)))
                            moveList.Add(new Move(this.Position, new Position("G1"), this, MoveType.WhiteCastle));
                    }

                    if (Rochades.Contains(MoveType.WhiteCastleLong))
                    {
                        bool thread = false;
                        for (int i = 1; i <=5; i++)
                        {
                            Position field = new Position(0, i);
                            thread = threatenedFields.Contains(field);
                            if (thread)
                                break;
                        }

                        if ((!thread) && ((Board[queenField] == null) && (Board[queenBishopField] == null) && (Board[queenKnightField] == null)))
                            moveList.Add(new Move(this.Position, new Position("C1"), this, MoveType.WhiteCastleLong));
                    }
                }
                else
                {
                    Position kingBishopField = new Position("F8");
                    Position kingKnightField = new Position("G8");
                    Position queenField = new Position("D8");
                    Position queenBishopField = new Position("C8");
                    Position queenKnightField = new Position("B8");

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

                        if ((!thread) && ((Board[kingBishopField] == null) && (Board[kingKnightField] == null)))
                            moveList.Add(new Move(this.Position, new Position("G8"), this, MoveType.BlackCastle));
                    }

                    if (Rochades.Contains(MoveType.BlackCastleLong))
                    {
                        bool thread = false;
                        for (int i = 1; i <= 5; i++)
                        {
                            Position field = new Position(7, i);
                            thread = threatenedFields.Contains(field);
                            if (thread)
                                break;
                        }

                        if ((!thread) && ((Board[queenField] == null) && (Board[queenBishopField] == null) && (Board[queenKnightField] == null)))
                            moveList.Add(new Move(this.Position, new Position("C8"), this, MoveType.BlackCastleLong));
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
            return false;
        }

        public King(Color color, List<MoveType> rochades) : base(color,
            PieceType.King)
        {
            Rochades = rochades;
        }
    }
}
