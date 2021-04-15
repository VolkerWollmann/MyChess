using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Microsoft.VisualBasic;
using MyChess.Common;

namespace MyChess.Model.Pieces
{
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
                    var threatenedFields = this.Board.GetAllPieces(ChessConstants.Color.Black).Select((piece => piece.GetMoves()))
                        .SelectMany(move => move).Select(move => move.End).ToList();

                    if (Rochades.Contains(ChessConstants.MoveType.WhiteCastle))
                    {
                        if (!(threatenedFields.Contains(new Position(0, 4)) ||
                              threatenedFields.Contains(new Position(0, 5)) ||
                              threatenedFields.Contains(new Position(0, 6))))
                        {
                            moves.Add(new Move(this.Position, new Position(0, 6), this, ChessConstants.MoveType.WhiteCastle));
                        }
                    }

                    if (Rochades.Contains(ChessConstants.MoveType.WhiteCastleLong))
                    {
                        if (!(threatenedFields.Contains(new Position(0, 4)) ||
                              threatenedFields.Contains(new Position(0, 3)) ||
                              threatenedFields.Contains(new Position(0, 2))))
                        {
                            moves.Add(new Move(this.Position, new Position(0, 2), this, ChessConstants.MoveType.WhiteCastleLong));
                        }
                    }
                }
                else
                {
                    var threatenedFields = this.Board.GetAllPieces(ChessConstants.Color.White).Select((piece => piece.GetMoves()))
                        .SelectMany(move => move).Select(move => move.End).ToList();

                    if (Rochades.Contains(ChessConstants.MoveType.WhiteCastle))
                    {
                        if (!(threatenedFields.Contains(new Position(7, 4)) ||
                              threatenedFields.Contains(new Position(7, 5)) ||
                              threatenedFields.Contains(new Position(7, 6))))
                        {
                            moves.Add(new Move(this.Position, new Position(7, 6), this, ChessConstants.MoveType.BlackCastle));
                        }
                    }

                    if (Rochades.Contains(ChessConstants.MoveType.WhiteCastleLong))
                    {
                        if (!(threatenedFields.Contains(new Position(7, 4)) ||
                              threatenedFields.Contains(new Position(7, 3)) ||
                              threatenedFields.Contains(new Position(7, 2))))
                        {
                            moves.Add(new Move(this.Position, new Position(7, 2), this, ChessConstants.MoveType.BlackCastleLong));
                        }
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
                    Board[new Position(0, 6)] = this;
                    Board[new Position(0, 4)] = null;
                    Board[new Position(0, 6)] = Board[new Position(0, 7)];
                    Board[new Position(0, 7)] = null;
                    break;

                case ChessConstants.MoveType.WhiteCastleLong:
                    Board[new Position(0, 2)] = this;
                    Board[new Position(0, 4)] = null;
                    Board[new Position(0, 3)] = Board[new Position(0, 0)];
                    Board[new Position(0, 0)] = null;
                    break;

                case ChessConstants.MoveType.BlackCastle:
                    Board[new Position(7, 6)] = this;
                    Board[new Position(7, 4)] = null;
                    Board[new Position(7, 6)] = Board[new Position(7, 7)];
                    Board[new Position(7, 7)] = null;
                    break;

                case ChessConstants.MoveType.BlackCastleLong:
                    Board[new Position(7, 2)] = this;
                    Board[new Position(7, 4)] = null;
                    Board[new Position(7, 3)] = Board[new Position(7, 0)];
                    Board[new Position(7, 0)] = null;
                    break;

                default:
                    Board[move.End] = this;
                    Board[move.Start] = null;
                    break;
            }
            
            Rochades = new List<ChessConstants.MoveType>() { };

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
