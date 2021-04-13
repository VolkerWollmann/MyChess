namespace MyChess.Common
{
    interface IChessEngine
    {
        IPiece GetPiece(Position position);

        void New();

        void Clear();

        public bool ExecuteMove(Move move);
    }
}
