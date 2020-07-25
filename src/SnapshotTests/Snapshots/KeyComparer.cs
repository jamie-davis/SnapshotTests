using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal static class KeyComparer
    {
        internal static int CompareKeySets(List<object> myKeys, List<object> theirKeys)
        {
            var myKeyCount = myKeys.Count;
            var otherKeyCount = theirKeys.Count();

            if (myKeyCount != otherKeyCount)
                return myKeyCount.CompareTo(otherKeyCount);

            var result = myKeys.Select<object, int?>((k, i) => ValueComparer.Compare(k, theirKeys[i])).FirstOrDefault(r => r != 0);
            return result ?? 0;
        }
    }
}