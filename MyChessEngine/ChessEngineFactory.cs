using System;
using MyChessEngine.Bitboard;
using MyChessEngineBase.Interfaces;

namespace MyChessEngine
{
    public static class ChessEngineFactory
    {
        private const string EngineModeVariable = "MYCHESS_ENGINE";
        private const string BitboardMode = "bitboard";

        public static IChessEngine CreateDefault()
        {
            string mode = Environment.GetEnvironmentVariable(EngineModeVariable);
            if (!string.IsNullOrWhiteSpace(mode) && mode.Equals(BitboardMode, StringComparison.OrdinalIgnoreCase))
                return new BitboardChessEngine();

            return new ChessEngine();
        }
    }
}
