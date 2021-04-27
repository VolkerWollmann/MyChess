using System;
using MyChessEngineBase;
using MyChessEngineBase.Rating;

namespace MyChessEngineInteger
{
    public class MyChessEngineInteger : IChessEngine
    {
        public IPiece GetPiece(Position position)
        {
            throw new NotImplementedException();
        }

        public Color ColorToMove { get; set; }
        public void New()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IPiece this[string position]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public BoardRating GetRating(Color color)
        {
            throw new NotImplementedException();
        }

        public void Test()
        {
            throw new NotImplementedException();
        }

        public MoveList GetMoveList()
        {
            throw new NotImplementedException();
        }

        public BoardRating GetBoardRating()
        {
            throw new NotImplementedException();
        }

        public bool ExecuteMove(Move move)
        {
            throw new NotImplementedException();
        }

        public Move CalculateMove()
        {
            throw new NotImplementedException();
        }

        public string Message { get; }
    }
}
