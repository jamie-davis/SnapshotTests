using System.Collections.Generic;

namespace SnapshotTests.Snapshots
{
    internal class SubstituteValues
    {
        public ColumnValueSet ColumnValueSet { get; }
        public List<ValueSubstitution> Substitutes { get; }

        public SubstituteValues(ColumnValueSet columnValueSet, List<ValueSubstitution> substitutes)
        {
            ColumnValueSet = columnValueSet;
            Substitutes = substitutes;
        }
    }
}