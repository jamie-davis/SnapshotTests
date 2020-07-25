using System.Collections.Generic;
using SnapshotTests.Snapshots.ValueTrackers;

namespace SnapshotTests.Snapshots
{
    internal static class TrackerMaker
    {
        public static List<ISubstitutableValueTracker> Make(ColumnValueSet columnValueSet)
        {
            return new List<ISubstitutableValueTracker> {new DefaultValueTracker(columnValueSet.Column.Name)};
        }
    }
}