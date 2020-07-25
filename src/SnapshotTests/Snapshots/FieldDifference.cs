namespace SnapshotTests.Snapshots
{
    internal class FieldDifference
    {
        public string Name { get; }
        public object Before { get; }
        public object After { get; }

        public FieldDifference(string name, object before, object after)
        {
            Name = name;
            Before = before;
            After = after;
        }
    }
}