using System;
using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots.ValueTrackers
{
    /// <summary>
    /// The default value tracker. This tracker will accept any data type.
    /// </summary>
    internal class DefaultValueTracker : ISubstitutableValueTracker
    {
        private List<(object Value, string Substitute)> _values = new List<(object Value, string Substitute)>();
        private string _columnName;

        public DefaultValueTracker(string columnName)
        {
            _columnName = columnName;
        }

        #region Implementation of ISubstitutableValueTracker

        public Type TrackedType => null;

        public string GetSubstitute(object value)
        {
            if (value == null) return null;

            var (_, substitute) = _values.SingleOrDefault(s => s.Value.Equals(value));
            if (substitute != null)
                return substitute;

            var newSubstitute = $"{_columnName}-{_values.Count + 1}";
            _values.Add((value, newSubstitute));
            return newSubstitute;
        }

        #endregion
    }
}