using MyChessEngineBase;

namespace MyChessEngineInteger.Pieces
{
    public enum NumPieces
    {
        Empty = 0,
        WhitePawn = 100,
        WhiteKnight = 300,
        WhiteBishop = 350,
        WhiteRook = 500,
        WhiteQueen = 900,
        WhiteKing = 10000,

        BlackPawn = -100,
        BlackKnight = -300,
        BlackBishop = -350,
        BlackRook = -500,
        BlackQueen = -900,
        BlackKing = -10000,
    }
    public class Piece : IPiece
    {
        public PieceType Type { get; }
        public Color Color { get; }

        public NumPieces GetNumPieces()
        {
            NumPieces numPiece = 0;
            switch (Type)
            {
                case PieceType.Pawn:
                    numPiece = (Color == Color.White) ? NumPieces.WhitePawn : NumPieces.BlackPawn;
                    break;
                case PieceType.Knight:
                    numPiece = (Color == Color.White) ? NumPieces.WhiteKnight : NumPieces.BlackKnight;
                    break;
                case PieceType.Bishop:
                    numPiece = (Color == Color.White) ? NumPieces.WhiteBishop : NumPieces.BlackBishop;
                    break;
                case PieceType.Rook:
                    numPiece = (Color == Color.White) ? NumPieces.WhiteRook : NumPieces.BlackRook;
                    break;
                case PieceType.Queen:
                    numPiece = (Color == Color.White) ? NumPieces.WhiteQueen : NumPieces.BlackQueen;
                    break;
                case PieceType.King:
                    numPiece = (Color == Color.White) ? NumPieces.WhiteKing : NumPieces.BlackKing;
                    break;
            }

            return numPiece;
        }

        public Piece(IPiece piece)
        {
            Color = piece.Color;
            Type = piece.Type;
        }

        public Piece(NumPieces piece)
        {
            switch (piece)
            {
                case NumPieces.WhitePawn:
                    Type = PieceType.Pawn;
                    Color = Color.White;
                    break;

                case NumPieces.WhiteKnight:
                    Type = PieceType.Knight;
                    Color = Color.White;
                    break;

                case NumPieces.WhiteKing:
                    Type = PieceType.King;
                    Color = Color.White;
                    break;

                case NumPieces.WhiteQueen:
                    Type = PieceType.Queen;
                    Color = Color.White;
                    break;

                case NumPieces.WhiteBishop:
                    Type = PieceType.Bishop;
                    Color = Color.White;
                    break;

                case NumPieces.WhiteRook:
                    Type = PieceType.Rook;
                    Color = Color.White;
                    break;

                case NumPieces.BlackPawn:
                    Type = PieceType.Pawn;
                    Color = Color.Black;
                    break;

                case NumPieces.BlackKnight:
                    Type = PieceType.Knight;
                    Color = Color.Black;
                    break;

                case NumPieces.BlackKing:
                    Type = PieceType.King;
                    Color = Color.Black;
                    break;

                case NumPieces.BlackQueen:
                    Type = PieceType.Queen;
                    Color = Color.Black;
                    break;

                case NumPieces.BlackBishop:
                    Type = PieceType.Bishop;
                    Color = Color.Black;
                    break;

                case NumPieces.BlackRook:
                    Type = PieceType.Rook;
                    Color = Color.Black;
                    break;
            }
        }

        #region ExecuteMove

        public static void MoveWhiteRook(Board board, int startRow, int startColumn)
        {
            if ((board.Data[(int) ChessEngineIntegerFlags.WhiteA1RookMoved] == 1) &&
                (board.Data[(int) ChessEngineIntegerFlags.WhiteH1RookMoved] == 1))
                return;

            if (startRow > 0)
                return;

            if (startRow == 0 && startColumn == 0)
            {
                board.Data[(int) ChessEngineIntegerFlags.WhiteA1RookMoved] = 1;
                board.Data[(int) ChessEngineIntegerFlags.WhiteKingLongCastle] = 0;
            }

            if (startRow == 0 && startColumn == 7)
            {
                board.Data[(int)ChessEngineIntegerFlags.WhiteH1RookMoved] = 1;
                board.Data[(int)ChessEngineIntegerFlags.WhiteKingCastle] = 0;
            }
        }

        public static void MoveBlackRook(Board board, int startRow, int startColumn)
        {
            if ((board.Data[(int)ChessEngineIntegerFlags.BlackA8RookMoved] == 1) &&
                (board.Data[(int)ChessEngineIntegerFlags.BlackH8RookMoved] == 1))
                return;

            if (startRow < 7)
                return;

            if (startRow == 7 && startColumn == 0)
            {
                board.Data[(int)ChessEngineIntegerFlags.BlackA8RookMoved] = 1;
                board.Data[(int)ChessEngineIntegerFlags.BlackKingLongCastle] = 0;
            }

            if (startRow == 7 && startColumn == 7)
            {
                board.Data[(int)ChessEngineIntegerFlags.BlackH8RookMoved] = 1;
                board.Data[(int)ChessEngineIntegerFlags.BlackKingCastle] = 0;
            }
        }

        public static void MoveWhiteKing(Board board, int endRow, int endColumn)
        {
            board.Data[(int)ChessEngineIntegerFlags.WhiteKingCastle] = 0;
            board.Data[(int)ChessEngineIntegerFlags.WhiteKingLongCastle] = 0;
            board.Data[(int)ChessEngineIntegerFlags.WhiteKingRow] = endRow;
            board.Data[(int)ChessEngineIntegerFlags.WhiteKingColumn] = endColumn;
        }

        public static void MoveBlackKing(Board board, int endRow, int endColumn)
        {
            board.Data[(int)ChessEngineIntegerFlags.BlackKingCastle] = 0;
            board.Data[(int)ChessEngineIntegerFlags.BlackKingLongCastle] = 0;
            board.Data[(int)ChessEngineIntegerFlags.BlackKingRow] = endRow;
            board.Data[(int)ChessEngineIntegerFlags.BlackKingColumn] = endColumn;
        }

        public static bool ExecuteMove(Board board, Move  move)
        {
            int startRow = move.StartRow;
            int startColumn = move.StartColumn;
            int endRow = move.EndRow;
            int endColumn = move.EndColumn;

            NumPieces piece = board[startRow, startColumn];
            board[endRow, endColumn] = piece;

            switch (piece)
            {
                case NumPieces.WhiteKing:
                    Piece.MoveWhiteKing(board, endRow, endColumn);
                    break;

                case NumPieces.BlackKing:
                    Piece.MoveBlackKing(board, endRow, endColumn);
                    break;

                case NumPieces.WhitePawn:
                    break;

                case NumPieces.BlackPawn:
                    break;

                case NumPieces.WhiteRook:
                    Piece.MoveWhiteRook(board,startRow, startColumn);
                    break;

                case NumPieces.BlackRook:
                    Piece.MoveWhiteRook(board, startRow, startColumn);
                    break;
            }

            return true;
        }

        #endregion
    }
}
