namespace MyChess.Common
{
    interface IChessEngine
    {
        IPiece[,] GetBoard();

        void New();

        void Clear();

        public bool ExecuteMove(int startRow, int startColumn, int endRow, int endColumn);
    }
}
