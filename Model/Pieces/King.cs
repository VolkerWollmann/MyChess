using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color} Rochades={Rochades}")]
    public class King : Piece
    {
        public List<ChessConstants.MoveType> Rochades;

        #region IEnginePiece
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            for (int row = -1; row <= 1; row++)
            for (int column = -1; column <= 1; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(row, column);

                if (this.Board.IsValidPosition(newPosition, this.Color))
                    moves.Add(new Move(this.Position, newPosition, this));
            }

            // Castle

            if (Rochades.Any())
            {
                if (Color == ChessConstants.Color.White)
                {
                    Position kingBishopField = new Position("F1");
                    Position kingKnightField = new Position("G1");
                    Position queenField = new Position("D1");
                    Position queenBishopField = new Position("C1");
                    Position queenKnightField = new Position("B1");

                    var threatenedFields = this.Board.GetAllPieces(ChessConstants.Color.Black)
                        .Where( piece => (piece.Type != ChessConstants.Piece.King))             // King cannot threaten castle, avoid for recursion
                        .Select((piece => piece.GetMoves()))
                        .SelectMany(move => move).Select(move => move.End).ToList();

                    if (Rochades.Contains(ChessConstants.MoveType.WhiteCastle))
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
                            moves.Add(new Move(this.Position, new Position("G1"), this, ChessConstants.MoveType.WhiteCastle));
                    }

                    if (Rochades.Contains(ChessConstants.MoveType.WhiteCastleLong))
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
                            moves.Add(new Move(this.Position, new Position("C1"), this, ChessConstants.MoveType.WhiteCastleLong));
                    }
                }
                else
                {
                    Position kingBishopField = new Position("F8");
                    Position kingKnightField = new Position("G8");
                    Position queenField = new Position("D8");
                    Position queenBishopField = new Position("C8");
                    Position queenKnightField = new Position("B8");

                    var threatenedFields = this.Board.GetAllPieces(ChessConstants.Color.White)
                        .Where(piece => (piece.Type != ChessConstants.Piece.King))             // King cannot threaten castle, avoid for recursion
                        .Select((piece => piece.GetMoves()))
                        .SelectMany(move => move).Select(move => move.End).ToList();

                    if (Rochades.Contains(ChessConstants.MoveType.BlackCastle))
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
                            moves.Add(new Move(this.Position, new Position("G8"), this, ChessConstants.MoveType.BlackCastle));
                    }

                    if (Rochades.Contains(ChessConstants.MoveType.BlackCastleLong))
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
                            moves.Add(new Move(this.Position, new Position("C8"), this, ChessConstants.MoveType.BlackCastleLong));
                    }
                }

            }

            return moves;
        }

        public override Piece Copy()
        {
            return new King(Color, Rochades);
        }

        public override bool ExecuteMove(Move move)
        {
            switch (move.Type)
            {
                case ChessConstants.MoveType.WhiteCastle:
                    Board["G1"] = this;
                    Board["E1"] = null;
                    Board["F1"] = Board["H1"];
                    Board["H1"] = null;
                    break;

                case ChessConstants.MoveType.WhiteCastleLong:
                    Board["C1"] = this;
                    Board["E1"] = null;
                    Board["D1"] = Board["A1"];
                    Board["A1"] = null;
                    break;

                case ChessConstants.MoveType.BlackCastle:
                    Board["G8"] = this;
                    Board["E8"] = null;
                    Board["F8"] = Board["H8"];
                    Board["H8"] = null;
                    break;

                case ChessConstants.MoveType.BlackCastleLong:
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
            
            Rochades = new List<ChessConstants.MoveType>();

            return true;
        }

        #endregion

        public King(ChessConstants.Color color) : base(color, ChessConstants.Piece.King)
        {
            if (color == ChessConstants.Color.White)
            {
                Rochades = new List<ChessConstants.MoveType>()
                    {ChessConstants.MoveType.WhiteCastle, ChessConstants.MoveType.WhiteCastleLong};
            }
            else
            {
                Rochades = new List<ChessConstants.MoveType>()
                    {ChessConstants.MoveType.BlackCastle, ChessConstants.MoveType.BlackCastleLong};
            }
        }

        public King(ChessConstants.Color color, List<ChessConstants.MoveType> rochades) : base(color,
            ChessConstants.Piece.King)
        {
            Rochades = rochades;
        }
    }
}
