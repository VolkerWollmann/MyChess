using System;

namespace MyChess.Helper
{
    public class ChessMenuEventArgs : EventArgs
    {
        public string Tag { get; }

        public ChessMenuEventArgs(string tag) 
        {
            Tag = tag;
        }
    }
}
