using System;
using System.Collections.Generic;
using System.Text;

namespace EngineUnitTests
{
    internal class RandomListAccess
    {
        private static Random _random = new Random();

        public static void SetSeed(int i)
        {
            _random = new Random(i);
        }

        internal static T GetRandomElement<T>(List<T> list)
        {
            int index = _random.Next(0, list.Count);
            return list[index];
        }

        internal static List<T> GetShuffledList<T>(List<T> list)
        {
            List<T> result = new List<T>();
            List<T> work = new List<T>();
            list.ForEach(e => work.Add(e));
            while (work.Count > 0)
            {
                int index = _random.Next(0, work.Count);
                result.Add(work[index]);
                work.RemoveAt(index);
            }

            return result;
        }
    }
}
