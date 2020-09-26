using System.Collections.Generic;
using SnapshotTests.Snapshots.ValueTrackers;

namespace SnapshotTests.Snapshots
{
    internal static class TrackerMaker
    {
        public static List<ISubstitutableValueTracker> Make(ColumnValueSet columnValueSet,
            SnapshotCollection snapshotCollection)
        {
            return new List<ISubstitutableValueTracker>
            {
                new DateTimeValueTracker(columnValueSet.Column, TimeRangeExtractor.Extract(snapshotCollection)),
                new DefaultValueTracker(columnValueSet.Column.Name)
            };
        }
    }
}