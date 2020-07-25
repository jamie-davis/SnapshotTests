namespace SnapshotTests.Snapshots
{
    internal class RowDifference
    {
        public SnapshotRowKey Key { get; }
        public DifferenceType DifferenceType { get; }
        public SnapshotRow Before { get; }
        public SnapshotRow After { get; }
        public RowDataCompareResult Differences { get; }

        public RowDifference(SnapshotRowKey key, DifferenceType differenceType, SnapshotRow before, SnapshotRow after)
        {
            Key = key;
            DifferenceType = differenceType;
            Before = before;
            After = after;
        }

        public RowDifference(SnapshotRowKey key, RowDataCompareResult differences, DifferenceType type)
        {
            Key = key;
            DifferenceType = type;
            Differences = differences;
            Before = differences.Before;
            After = differences.After;
        }
    }
}