using System;

namespace SnapshotTests.Snapshots
{
    internal class NamedTimeRange
    {
        public NamedTimeRange(DateTime start, DateTime end, string name)
        {
            Start = start;
            End = end;
            Name = name;
        }

        public DateTime Start { get; }
        public DateTime End { get; }
        public string Name { get; }
    }
}