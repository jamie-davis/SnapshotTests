using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal static class ValueSubstituter
    {
        public static List<SnapshotTableDifferences> Substitute(List<ColumnValueSet> unpredictableValues,
            List<SnapshotTableDifferences> tableDiffs, SnapshotCollection snapshotCollection)
        {
            var substitutes = unpredictableValues
                .Select(uv => new SubstituteValues( uv, SubstitutionMaker.Make(uv.Values, TrackerMaker.Make(uv, snapshotCollection))))
                .ToList();

            return DiffValueSubstituter.Substitute(tableDiffs, substitutes);
        }
    }
}