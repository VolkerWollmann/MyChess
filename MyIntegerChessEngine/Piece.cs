using MyChessEngineBase;

namespace MyIntegerChessEngine
{
    public class Piece : IPiece
    {
        public int PieceAsInteger;

        #region Properties

        public int PieceType
        {
            get { return PieceAsInteger & Constants.PieceMask; }
        }

        // Border (-1) and empty (0) squares must be tested on the raw value:
        // masking -1 with PieceMask/ColorMask yields 7/Black, and White == NoPiece == 0.
        public bool IsBorder
        {
            get { return PieceAsInteger == Constants.BoardBorder; }
        }

        public bool IsEmpty
        {
            get { return PieceAsInteger == Constants.NoPiece; }
        }

        public PieceType Type
        {
            get
            {
                return (PieceAsInteger & Constants.PieceMask) switch
                {
                    Constants.Pawn => MyChessEngineBase.PieceType.Pawn,
                    Constants.Knight => MyChessEngineBase.PieceType.Knight,
                    Constants.Bishop => MyChessEngineBase.PieceType.Bishop,
                    Constants.Rook => MyChessEngineBase.PieceType.Rook,
                    Constants.Queen => MyChessEngineBase.PieceType.Queen,
                    Constants.King => MyChessEngineBase.PieceType.King,
                    _ => throw new InvalidOperationException("No PieceType for empty square or border.")
                };
            }
        }

        public Color Color
        {
            get
            {
                return IntColor == Constants.White ? Color.White : Color.Black;
            }
        }

        public int LastPly { get; set; }
        public int PromotionPly { get; set; }
        public int LastEnPassantPlyMarking { get; set; }

        /// Bit mask of the castles still possible; only evaluated
        /// when a king is placed on the board (Board.SetPiece).
        public CastleType PossibleCastles { get; set; } = CastleType.None;

        public int IntColor
        {
            get { return PieceAsInteger & Constants.ColorMask; }
        }

        // Assume both on same position
        public bool Compare(Piece other)
        {
            return PieceAsInteger == other.PieceAsInteger
                   && LastPly == other.LastPly
                   && PromotionPly == other.PromotionPly
                   && LastEnPassantPlyMarking == other.LastEnPassantPlyMarking;
        }

        public virtual bool Move(Move move)
        {
            return true;
        }

        public bool Compare(IPiece other)
        {
            throw new NotImplementedException();
        }

        public bool IsMoved()
        {
            return LastPly > 0;
        }

        #endregion

        #region Constructors

        public Piece(int pieceAsInteger, int lastPly, int promotionPly, int lastEnPassantPlyMarking)
        {
            PieceAsInteger = pieceAsInteger;
            LastPly = lastPly;
            PromotionPly = promotionPly;
            LastEnPassantPlyMarking = lastEnPassantPlyMarking;
        }

        public Piece()
        {

        }

        #endregion

        internal virtual MoveList GetMoveList(Board board, Position position)
        {
            MoveList moveList = new MoveList();
            return moveList;
        }
    }

    public class PieceFactory
    {
        public static Piece Create(int pieceType, int color ,int lastPly=-1, int promotionPly=-1, int lastEnPassantPlyMarking=-1)
        {
            if (color != Constants.White && color != Constants.Black)
            {
                throw new ArgumentException("Invalid color for piece creation.");
            }
            return new Piece(pieceType | color , lastPly, promotionPly, lastEnPassantPlyMarking);
        }

        public static Piece WhitePawn()
        {
            return Create(Constants.Pawn, Constants.White);
        }

        public static Piece BlackPawn()
        {
            return Create(Constants.Pawn, Constants.Black);
        }

        public static Piece WhiteKnight()
        {
            return Create(Constants.Knight, Constants.White);
        }

        public static Piece BlackKnight()
        {
            return Create(Constants.Knight, Constants.Black);
        }

        public static Piece WhiteBishop()
        {
            return Create(Constants.Bishop, Constants.White);
        }
        public static Piece BlackBishop()
        {
            return Create(Constants.Bishop, Constants.Black);
        }
        public static Piece WhiteRook()
        {
            return Create(Constants.Rook, Constants.White);
        }
        public static Piece BlackRook()
        {
            return Create(Constants.Rook, Constants.Black);
        }

        public static Piece WhiteQueen()
        {
            return Create(Constants.Queen, Constants.White);
        }

        public static Piece BlackQueen()
        {
            return Create(Constants.Queen, Constants.Black);
        }

        public static Piece WhiteKing(CastleType possibleCastles = CastleType.WhiteKingSide | CastleType.WhiteQueenSide)
        {
            Piece king = Create(Constants.King, Constants.White);
            king.PossibleCastles = possibleCastles;
            return king;
        }

        public static Piece BlackKing(CastleType possibleCastles = CastleType.BlackKingSide | CastleType.BlackQueenSide)
        {
            Piece king = Create(Constants.King, Constants.Black);
            king.PossibleCastles = possibleCastles;
            return king;
        }
    }
}

