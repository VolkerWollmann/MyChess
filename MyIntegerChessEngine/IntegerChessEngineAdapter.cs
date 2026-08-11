using MyChessEngineBase;
using MyChessEngineBase.Interfaces;
using MyChessEngineBase.Rating;
using BaseMove = MyChessEngineBase.Move;
using BasePosition = MyChessEngineBase.Position;

namespace MyIntegerChessEngine
{
    /// Adapts the IntegerChessEngine to the IChessEngine interface used by the UI:
    /// translates positions, moves, pieces and ratings between the base types
    /// and the integer representation.
    public class IntegerChessEngineAdapter : IChessEngine
    {
        private readonly IntegerChessEngine Engine = new();

        private string _Message = "";

        public string Message => _Message;

        public Color ColorToMove
        {
            get => Engine.ColorToMove == Constants.White ? Color.White : Color.Black;
            set => Engine.ColorToMove = value == Color.White ? Constants.White : Constants.Black;
        }

        public IPiece? GetPiece(BasePosition position)
        {
            Piece piece = Engine.Board.GetPiece(new Position(position.Column, position.Row));

            if (piece.IsEmpty || piece.IsBorder)
                return null;

            return piece;
        }

        public void SetPiece(BasePosition position, IPiece? piece)
        {
            Position target = new Position(position.Column, position.Row);

            if (piece == null)
            {
                Engine.Board.ClearSquare(target);
                return;
            }

            if (piece is Piece integerPiece)
            {
                Engine.Board.SetPiece(integerPiece, target);
                return;
            }

            Engine.Board.SetPiece(ToIntegerPiece(piece), target);
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

        public BoardRating GetRating(Color color)
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

            Position start = new Position(move.Start.Column, move.Start.Row);
            Position end = new Position(move.End.Column, move.End.Row);

            Piece piece = Engine.Board.GetPiece(start);
            if (piece.IsEmpty || piece.IsBorder)
                return false;

            return Engine.ExecuteMove(new Move(start, end, piece, GetCastleType(piece, start, end)));
        }

        public BaseMove CalculateMove()
        {
            return CalculateMove(Constants.DefaultSearchDepth);
        }

        public BaseMove CalculateMove(int depth)
        {
            DateTime startTime = DateTime.Now;

            Move? move = Engine.CalculateMoveParallel(depth);

            TimeSpan duration = DateTime.Now - startTime;

            if (move == null)
            {
                _Message = "IntegerChessEngine: no move possible. " + Engine.GetRating();
                return BaseMove.CreateNoMove(ToBoardRating(Engine.GetRating()));
            }

            _Message = FormatMove(move) + " Time:" + duration + Environment.NewLine + move.Rating +
                       Environment.NewLine + "Line: " + move.Rating.MoveList;

            return new BaseMove(
                new BasePosition(move.Start.Column, move.Start.Row),
                new BasePosition(move.End.Column, move.End.Row),
                move.Piece,
                ToMoveType(move.CastleType))
            {
                Rating = ToBoardRating(move.Rating)
            };
        }

        public void Test()
        {
            _Message = string.Join(Environment.NewLine, Engine.GetMoveList().Select(FormatMove));
        }

        #region Conversion

        private static string FormatMove(Move move)
        {
            return $"{move.Piece.Color} {move.Piece.Type} {move.Start} -> {move.End}";
        }

        /// The integer engine encodes castling in the move; the base move only shows
        /// the two-square king move, so the castle type is derived from the geometry.
        private static CastleType GetCastleType(Piece piece, Position start, Position end)
        {
            if (piece.PieceType != Constants.King || Math.Abs(end.Column - start.Column) != 2)
                return CastleType.None;

            if (piece.IntColor == Constants.White)
                return end.Column > start.Column ? CastleType.WhiteKingSide : CastleType.WhiteQueenSide;

            return end.Column > start.Column ? CastleType.BlackKingSide : CastleType.BlackQueenSide;
        }

        private static MoveType ToMoveType(CastleType castleType)
        {
            return castleType switch
            {
                CastleType.WhiteKingSide => MoveType.WhiteCastle,
                CastleType.WhiteQueenSide => MoveType.WhiteCastleLong,
                CastleType.BlackKingSide => MoveType.BlackCastle,
                CastleType.BlackQueenSide => MoveType.BlackCastleLong,
                _ => MoveType.Normal
            };
        }

        private static Piece ToIntegerPiece(IPiece piece)
        {
            int pieceType = piece.Type switch
            {
                PieceType.Pawn => Constants.Pawn,
                PieceType.Knight => Constants.Knight,
                PieceType.Bishop => Constants.Bishop,
                PieceType.Rook => Constants.Rook,
                PieceType.Queen => Constants.Queen,
                PieceType.King => Constants.King,
                _ => Constants.NoPiece
            };

            int color = piece.Color == Color.White ? Constants.White : Constants.Black;

            return PieceFactory.Create(pieceType, color);
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
