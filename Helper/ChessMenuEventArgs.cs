using System;
using System.Collections.Generic;
using System.Text;

namespace MyChess.Helper
{
    public class ChessMenuEventArgs : EventArgs
    {
        public string Tag { get; private set; }

        public ChessMenuEventArgs(string tag) 
        {
            Tag = tag;
        }
    }
}
