using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SnapshotTests.Snapshots
{
    internal class RowDataCompareResult
    {
        private readonly ReadOnlyCollection<FieldDifference> _differences;

        public RowDataCompareResult(bool matched, IList<FieldDifference> differences, SnapshotRow before, SnapshotRow after)
        {
            Matched = matched;
            _differences = differences != null ? new ReadOnlyCollection<FieldDifference>(differences) : null;
            Before = before;
            After = after;
        }

        public bool Matched { get; }
        public IEnumerable<FieldDifference> Differences => _differences;
        public SnapshotRow Before { get; }
        public SnapshotRow After { get; }
    }
}