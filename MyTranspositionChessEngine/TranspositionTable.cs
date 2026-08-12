using System.Diagnostics;
using System.Threading;

namespace MyTranspositionChessEngine
{
    public enum Bound : byte
    {
        Exact,  // value is the searched value of the node
        Lower,  // search was cut off, true value is at least Value
        Upper   // node failed low, true value is at most Value
    }

    /// One stored search result. Immutable: the table publishes entries by
    /// swapping array references, so concurrent readers (the parallel root
    /// search shares one table) always see a complete entry, old or new.
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class TranspositionEntry
    {
        public readonly ulong Key;
        public readonly int Depth;
        public readonly int Value;
        public readonly Bound Bound;
        public readonly GameState State;
        public readonly string MoveLine;

        internal TranspositionEntry(ulong key, int depth, int value, Bound bound, GameState state, string moveLine)
        {
            Key = key;
            Depth = depth;
            Value = value;
            Bound = bound;
            State = state;
            MoveLine = moveLine;
        }

        /// A fresh Rating per hit: every rating object flows up exactly one
        /// search path and is mutated there (AddMove), so the stored entry
        /// must never be handed out directly.
        internal Rating ToRating()
        {
            return new Rating(Value, State) { MoveList = MoveLine };
        }

        /// Readable form for the debugger: hex key instead of a 20-digit
        /// decimal, plus depth, bound, value and the start of the line.
        private string DebuggerDisplay =>
            $"Key=0x{Key:X16} d{Depth} {Bound} Value={Value} {State} "
            + (MoveLine.Length > 24 ? MoveLine[..24] + "..." : MoveLine);
    }

    /// Fixed-size, always-replace transposition table keyed by Zobrist key AND
    /// remaining depth. The exact-depth match is deliberate: win/loss ratings
    /// contain the remaining depth (mate speed) and the threat-field term
    /// depends on the leaf parity, so a result may only be reused where the
    /// plain search would have computed the very same value.
    public sealed class TranspositionTable
    {
        private readonly TranspositionEntry?[] Entries;
        private readonly ulong Mask;

        private long _Probes;
        private long _Hits;
        private long _Stores;

        public long Probes => Interlocked.Read(ref _Probes);
        public long Hits => Interlocked.Read(ref _Hits);
        public long Stores => Interlocked.Read(ref _Stores);

        public TranspositionTable(int sizeExponent = 20)
        {
            Entries = new TranspositionEntry?[1 << sizeExponent];
            Mask = (ulong)Entries.Length - 1;
        }

        /// Mixing the depth into the slot index lets the same position coexist
        /// at different remaining depths instead of thrashing one slot.
        private int Index(ulong key, int depth)
        {
            return (int)((key ^ ((ulong)depth * 0x9E3779B97F4A7C15UL)) & Mask);
        }

        public TranspositionEntry? Probe(ulong key, int depth)
        {
            Interlocked.Increment(ref _Probes);

            TranspositionEntry? entry = Entries[Index(key, depth)];
            if (entry == null || entry.Key != key || entry.Depth != depth)
                return null;

            Interlocked.Increment(ref _Hits);
            return entry;
        }

        public void Store(ulong key, int depth, int value, Bound bound, GameState state, string moveLine)
        {
            Interlocked.Increment(ref _Stores);
            Entries[Index(key, depth)] = new TranspositionEntry(key, depth, value, bound, state, moveLine);
        }
    }
}
