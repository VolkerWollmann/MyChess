namespace MyIntegerChessEngine
{
    public class Piece
    {
        public int PieceAsInteger;
        public int LastPly;
        public int PromotionPly;

        // set by other color neighbor pawn, if move adjacent
        // will be evaluated during move generation of that pawn
        public int LastEnPassantPlyMarking; 

        #region Properties
        public int PieceType
        {
            get
            {
                return PieceAsInteger & Constants.PieceMask;
            }
        }

        public int Color
        {
            get
            {
                return PieceAsInteger & Constants.ColorMask;
            }
        }
        
        // Assume both on same position
        public bool Compare(Piece other)
        {
            return PieceAsInteger == other.PieceAsInteger 
                   && LastPly == other.LastPly 
                   && PromotionPly == other.PromotionPly 
                   && LastEnPassantPlyMarking == other.LastEnPassantPlyMarking;
        }

        public bool IsMoved()
        {
            return (PieceAsInteger & Constants.IsMovedMask) != 0;
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

        #endregion

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

        public static Piece WhiteKing()
        {
            return Create(Constants.King, Constants.White);
        }

        public static Piece BlackKing()
        {
            return Create(Constants.King, Constants.Black);
        }
    }
}
