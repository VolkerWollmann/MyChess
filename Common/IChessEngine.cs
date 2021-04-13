using System;
using System.Collections.Generic;
using System.Text;

namespace MyChess.Common
{
    interface IChessEngine
    {
        IPiece[,] GetBoard();

        void New();

        void Clear();
    }
}
