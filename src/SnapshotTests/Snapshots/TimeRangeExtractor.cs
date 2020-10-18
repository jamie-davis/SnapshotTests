using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal static class TimeRangeExtractor
    {
        internal static List<NamedTimeRange> Extract(SnapshotCollection collection)
        {
            var start = collection.InitialisationTime;
            return collection
                .GetSnapshots()
                .Select(s =>
                {
                    var snapshotEnd = s.SnapshotTimestamp;
                    var range = new NamedTimeRange(start, snapshotEnd, s.Name);
                    start = snapshotEnd;
                    return range;
                }).ToList();
        }
    }
}