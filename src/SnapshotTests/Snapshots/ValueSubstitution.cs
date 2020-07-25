namespace SnapshotTests.Snapshots
{
    internal class ValueSubstitution
    {
        public ValueSubstitution(object value, string substituteValue)
        {
            Value = value;
            SubstituteValue = substituteValue;
        }

        public object Value { get; }
        public string SubstituteValue { get; }
    }
}