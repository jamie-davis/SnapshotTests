using System;
using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots.ValueTrackers
{
    /// <summary>
    /// The default value tracker. This tracker will accept any data type.
    /// </summary>
    internal class DateTimeValueTracker : ISubstitutableValueTracker
    {
        private readonly DefaultValueTracker _default;
        private string _columnName;
        private readonly List<NamedTimeRange> _namedTimeRanges;

        public DateTimeValueTracker(string columnName, List<NamedTimeRange> namedTimeRanges)
        {
            _columnName = columnName;
            _namedTimeRanges = namedTimeRanges;
            _default = new DefaultValueTracker(columnName);
        }

        #region Implementation of ISubstitutableValueTracker

        public Type TrackedType => typeof(DateTime);

        public string GetSubstitute(object value)
        {
            if (value == null) return null;

            if (value is DateTime valueDate && GetRange(valueDate, out var range))
            {
                return $"{_columnName}@{range.Name}";
            }

            return _default.GetSubstitute(value);
        }

        private bool GetRange(DateTime valueDate, out NamedTimeRange namedTimeRange)
        {
            namedTimeRange = _namedTimeRanges.FirstOrDefault(t => valueDate > t.Start && valueDate <= t.End);
            return namedTimeRange != null;
        }

        #endregion
    }
}