using System;
using System.Collections.Generic;
using System.Linq;
using SnapshotTests.Exceptions;

namespace SnapshotTests.Snapshots
{
    internal class SnapshotRowKey : IComparable<SnapshotRowKey>
    {
        private List<object> _keys;
        private List<string> _fields;

        internal SnapshotRowKey(SnapshotRow snapshotRow, TableDefinition tableDefinition)
        {
            if (tableDefinition.CompareKeys.Any())
                _keys = GetKeys(snapshotRow, tableDefinition.CompareKeys);
            else if (tableDefinition.PrimaryKeys.Any())
                _keys = GetKeys(snapshotRow, tableDefinition.PrimaryKeys);
            else
                _keys = GetKeys(snapshotRow, tableDefinition.Columns.Select(c => c.Name));
        }

        public IEnumerable<object> AllKeys => _keys.ToList();

        private List<object> GetKeys(SnapshotRow snapshotRow, IEnumerable<string> keyFields)
        {
            _fields = keyFields.ToList();
            return _fields.Select(snapshotRow.GetField).ToList();
        }

        internal object this[int index]
        {
            get { return _keys[index]; }
        }

        #region Relational members

        public int CompareTo(SnapshotRowKey other)
        {
            var myKeys = _keys;
            var theirKeys = other._keys;

            return KeyComparer.CompareKeySets(myKeys, theirKeys);
        }

        #endregion
        
        #region Equality members

        protected bool Equals(SnapshotRowKey other)
        {
            return _keys.SequenceEqual(other._keys);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SnapshotRowKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                foreach (var keyField in _keys)
                {
                    hash = hash * 31 + (keyField?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }

        #endregion

        public IEnumerable<string> GetFieldNames() => _fields;
    }
}