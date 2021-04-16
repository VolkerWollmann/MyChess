using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MyChessEngine;

namespace MyChessEngine.Pieces
{
    [DebuggerDisplay("Type={Type}, Name = {Color} Rochades={Rochades}")]
    public class King : Piece
    {
        public List<ChessEngineConstants.MoveType> Rochades;

        #region IEnginePiece
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            for (int row = -1; row <= 1; row++)
            for (int column = -1; column <= 1; column++)
            {
                Position newPosition = this.Position.GetDeltaPosition(row, column);
                AddPosition(moves, newPosition);
            }

            // Castle

            if (Rochades.Any())
            {
                if (Color == ChessEngineConstants.Color.White)
                {
                    Position kingBishopField = new Position("F1");
                    Position kingKnightField = new Position("G1");
                    Position queenField = new Position("D1");
                    Position queenBishopField = new Position("C1");
                    Position queenKnightField = new Position("B1");

                    var threatenedFields = this.Board.GetAllPieces(ChessEngineConstants.Color.Black)
                        .Where( piece => (piece.Type != ChessEngineConstants.PieceType.King))             // King cannot threaten castle, avoid for recursion
                        .Select((piece => piece.GetMoves()))
                        .SelectMany(move => move).Select(move => move.End).ToList();

                    if (Rochades.Contains(ChessEngineConstants.MoveType.WhiteCastle))
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
                            moves.Add(new Move(this.Position, new Position("G1"), this, ChessEngineConstants.MoveType.WhiteCastle));
                    }

                    if (Rochades.Contains(ChessEngineConstants.MoveType.WhiteCastleLong))
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
                            moves.Add(new Move(this.Position, new Position("C1"), this, ChessEngineConstants.MoveType.WhiteCastleLong));
                    }
                }
                else
                {
                    Position kingBishopField = new Position("F8");
                    Position kingKnightField = new Position("G8");
                    Position queenField = new Position("D8");
                    Position queenBishopField = new Position("C8");
                    Position queenKnightField = new Position("B8");

                    var threatenedFields = this.Board.GetAllPieces(ChessEngineConstants.Color.White)
                        .Where(piece => (piece.Type != ChessEngineConstants.PieceType.King))             // King cannot threaten castle, avoid for recursion
                        .Select((piece => piece.GetMoves()))
                        .SelectMany(move => move).Select(move => move.End).ToList();

                    if (Rochades.Contains(ChessEngineConstants.MoveType.BlackCastle))
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
                            moves.Add(new Move(this.Position, new Position("G8"), this, ChessEngineConstants.MoveType.BlackCastle));
                    }

                    if (Rochades.Contains(ChessEngineConstants.MoveType.BlackCastleLong))
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
                            moves.Add(new Move(this.Position, new Position("C8"), this, ChessEngineConstants.MoveType.BlackCastleLong));
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
                case ChessEngineConstants.MoveType.WhiteCastle:
                    Board["G1"] = this;
                    Board["E1"] = null;
                    Board["F1"] = Board["H1"];
                    Board["H1"] = null;
                    break;

                case ChessEngineConstants.MoveType.WhiteCastleLong:
                    Board["C1"] = this;
                    Board["E1"] = null;
                    Board["D1"] = Board["A1"];
                    Board["A1"] = null;
                    break;

                case ChessEngineConstants.MoveType.BlackCastle:
                    Board["G8"] = this;
                    Board["E8"] = null;
                    Board["F8"] = Board["H8"];
                    Board["H8"] = null;
                    break;

                case ChessEngineConstants.MoveType.BlackCastleLong:
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
            
            Rochades = new List<ChessEngineConstants.MoveType>();

            return true;
        }

        #endregion

        public King(ChessEngineConstants.Color color) : base(color, ChessEngineConstants.PieceType.King)
        {
            if (color == ChessEngineConstants.Color.White)
            {
                Rochades = new List<ChessEngineConstants.MoveType>()
                    {ChessEngineConstants.MoveType.WhiteCastle, ChessEngineConstants.MoveType.WhiteCastleLong};
            }
            else
            {
                Rochades = new List<ChessEngineConstants.MoveType>()
                    {ChessEngineConstants.MoveType.BlackCastle, ChessEngineConstants.MoveType.BlackCastleLong};
            }
        }

        public King(ChessEngineConstants.Color color, List<ChessEngineConstants.MoveType> rochades) : base(color,
            ChessEngineConstants.PieceType.King)
        {
            Rochades = rochades;
        }
    }
}
