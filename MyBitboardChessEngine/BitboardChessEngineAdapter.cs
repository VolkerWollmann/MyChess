using MyChessEngineBase;
using MyChessEngineBase.Interfaces;
using MyChessEngineBase.Rating;
using BaseMove = MyChessEngineBase.Move;
using BasePosition = MyChessEngineBase.Position;
using BaseColor = MyChessEngineBase.Color;

namespace MyBitboardChessEngine
{
    /// Adapts the BitboardChessEngine to the IChessEngine interface used by
    /// the UI: translates positions, moves, pieces and ratings between the
    /// base types and the bitboard representation.
    public class BitboardChessEngineAdapter : IChessEngine
    {
        private readonly BitboardChessEngine Engine = new();

        private string _Message = "";

        public string Message => _Message;

        public BaseColor ColorToMove
        {
            get => Engine.ColorToMove == Constants.White ? BaseColor.White : BaseColor.Black;
            set => Engine.ColorToMove = value == BaseColor.White ? Constants.White : Constants.Black;
        }

        public IPiece? GetPiece(BasePosition position)
        {
            int piece = Engine.Board.PieceAt(ToSquare(position));
            if (piece == Constants.NoPiece)
                return null;

            return new Piece(ToPieceType(piece), ToColor(piece));
        }

        public void SetPiece(BasePosition position, IPiece? piece)
        {
            int square = ToSquare(position);

            if (piece == null)
            {
                Engine.Board.SetPiece(Constants.NoPiece, square);
                return;
            }

            Engine.Board.SetPiece(ToPieceCode(piece), square);
        }

        public void SetPiece(string position, IPiece? piece)
        {
            SetPiece(new BasePosition(position), piece);
        }

        public void New()
        {
            Engine.New();
        }

        public void Clear()
        {
            Engine.Clear();
        }

        public BoardRating GetRating(BaseColor color)
        {
            return ToBoardRating(Engine.GetRating());
        }

        public BoardRating GetBoardRating()
        {
            return ToBoardRating(Engine.GetRating());
        }

        public bool ExecuteMove(BaseMove move)
        {
            // guards against "no move" results (Move.CreateNoMove) coming back from the UI
            if (move == null || move.Start.AreEqual(move.End))
                return false;

            return Engine.ExecuteMove(
                move.Start.Row * 8 + move.Start.Column,
                move.End.Row * 8 + move.End.Column);
        }

        public BaseMove CalculateMove()
        {
            return CalculateMove(Constants.DefaultSearchDepth);
        }

        public BaseMove CalculateMove(int depth)
        {
            DateTime startTime = DateTime.Now;

            EngineMove? move = Engine.CalculateMove(depth);

            TimeSpan duration = DateTime.Now - startTime;

            if (move == null)
            {
                _Message = "BitboardChessEngine: no move possible. " + Engine.GetRating();
                return BaseMove.CreateNoMove(ToBoardRating(Engine.GetRating()));
            }

            _Message = $"{move.Start} -> {move.End} Time:" + duration + Environment.NewLine + move.Rating +
                       Environment.NewLine +
                       $"Nodes: {Engine.Nodes} TT: {Engine.Table.Hits} hits / {Engine.Table.Probes} probes" +
                       Environment.NewLine + "Line: " + move.Rating.MoveList;

            int movedPiece = move.Move.Piece;

            return new BaseMove(
                new BasePosition(move.Move.From & 7, move.Move.From >> 3),
                new BasePosition(move.Move.To & 7, move.Move.To >> 3),
                new Piece(ToPieceType(movedPiece), ToColor(movedPiece)),
                ToMoveType(move.Move))
            {
                Rating = ToBoardRating(move.Rating)
            };
        }

        public void Test()
        {
            _Message = string.Join(Environment.NewLine, Engine.GetLegalMoves());
        }

        #region Conversion

        private static int ToSquare(BasePosition position)
        {
            return position.Row * 8 + position.Column;
        }

        private static PieceType ToPieceType(int piece)
        {
            return Constants.TypeOf(piece) switch
            {
                Constants.Pawn => PieceType.Pawn,
                Constants.Knight => PieceType.Knight,
                Constants.Bishop => PieceType.Bishop,
                Constants.Rook => PieceType.Rook,
                Constants.Queen => PieceType.Queen,
                _ => PieceType.King
            };
        }

        private static BaseColor ToColor(int piece)
        {
            return Constants.ColorOf(piece) == Constants.White ? BaseColor.White : BaseColor.Black;
        }

        private static int ToPieceCode(IPiece piece)
        {
            int type = piece.Type switch
            {
                PieceType.Pawn => Constants.Pawn,
                PieceType.Knight => Constants.Knight,
                PieceType.Bishop => Constants.Bishop,
                PieceType.Rook => Constants.Rook,
                PieceType.Queen => Constants.Queen,
                _ => Constants.King
            };

            return Constants.MakePiece(type,
                piece.Color == BaseColor.White ? Constants.White : Constants.Black);
        }

        private static MoveType ToMoveType(Move move)
        {
            return move.Flag switch
            {
                MoveFlag.CastleKingSide => Constants.ColorOf(move.Piece) == Constants.White
                    ? MoveType.WhiteCastle : MoveType.BlackCastle,
                MoveFlag.CastleQueenSide => Constants.ColorOf(move.Piece) == Constants.White
                    ? MoveType.WhiteCastleLong : MoveType.BlackCastleLong,
                MoveFlag.Promotion => MoveType.Promotion,
                _ => MoveType.Normal
            };
        }

        private static BoardRating ToBoardRating(Rating rating)
        {
            BoardRating boardRating = new BoardRating { Weight = rating.Value, MoveList = rating.MoveList };

            switch (rating.State)
            {
                case GameState.Normal:
                    boardRating.Situation = Situation.Normal;
                    boardRating.Evaluation = Evaluation.Normal;
                    break;

                case GameState.WhiteLoss:
                    boardRating.Situation = Situation.BlackVictory;
                    boardRating.Evaluation = Evaluation.WhiteCheckMate;
                    break;

                case GameState.BlackLoss:
                    boardRating.Situation = Situation.WhiteVictory;
                    boardRating.Evaluation = Evaluation.BlackCheckMate;
                    break;

                case GameState.Remis:
                    boardRating.Situation = Situation.StaleMate;
                    boardRating.Evaluation = Evaluation.Remis;
                    break;
            }

            return boardRating;
        }

        #endregion
    }
}
