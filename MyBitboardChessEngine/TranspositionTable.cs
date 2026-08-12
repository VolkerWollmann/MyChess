namespace MyBitboardChessEngine
{
    public enum Bound : byte
    {
        None = 0,
        Exact,   // value is the searched value of the node
        Lower,   // search was cut off, true value is at least Value
        Upper    // node failed low, true value is at most Value
    }

    /// Fixed-size transposition table. Because the evaluation is a pure
    /// function of the position and mate scores are stored node-relative, an
    /// entry searched to depth D answers any later probe needing depth <= D -
    /// unlike the TranspositionChessEngine, which had to match depths exactly.
    /// Entries also carry the best move, which iterative deepening feeds back
    /// as the first move to search - the main source of alpha-beta cutoffs.
    public sealed class TranspositionTable
    {
        private struct Entry
        {
            public ulong Key;
            public int Value;      // node-relative mate scores (see Search To/FromTable)
            public ushort Move;    // packed best move, 0 = none
            public short Depth;
            public Bound Bound;
            public byte Age;       // search counter, older entries are replaced first
        }

        private readonly Entry[] Entries;
        private readonly ulong Mask;
        private byte Age;

        public long Probes { get; private set; }
        public long Hits { get; private set; }
        public long Stores { get; private set; }

        public TranspositionTable(int sizeExponent = 20)
        {
            Entries = new Entry[1 << sizeExponent];
            Mask = (ulong)Entries.Length - 1;
        }

        /// Marks the begin of a new search: existing entries stay probeable
        /// (position-keyed results remain valid across searches), but they
        /// lose against fresh entries in the replacement decision.
        public void NewSearch()
        {
            Age++;
        }

        public void Clear()
        {
            Array.Clear(Entries);
            Probes = Hits = Stores = 0;
        }

        public bool Probe(ulong key, out int value, out ushort move, out int depth, out Bound bound)
        {
            Probes++;

            ref Entry entry = ref Entries[key & Mask];
            if (entry.Bound != Bound.None && entry.Key == key)
            {
                Hits++;
                value = entry.Value;
                move = entry.Move;
                depth = entry.Depth;
                bound = entry.Bound;
                return true;
            }

            value = 0;
            move = 0;
            depth = -1;
            bound = Bound.None;
            return false;
        }

        public void Store(ulong key, int depth, int value, Bound bound, ushort move)
        {
            ref Entry entry = ref Entries[key & Mask];

            // Same position: always update, but keep the known best move if the
            // new result has none. Different position: prefer fresh and deeper.
            if (entry.Bound != Bound.None && entry.Key != key
                && entry.Age == Age && entry.Depth > depth)
                return;

            Stores++;
            if (move == 0 && entry.Key == key)
                move = entry.Move;

            entry.Key = key;
            entry.Value = value;
            entry.Move = move;
            entry.Depth = (short)depth;
            entry.Bound = bound;
            entry.Age = Age;
        }
    }
}
