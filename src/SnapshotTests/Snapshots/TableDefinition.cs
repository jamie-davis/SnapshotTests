using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    public class TableDefinition
    {
        public class Reference
        {
            internal Reference(string ourField, string targetTable, string targetField)
            {
                OurField = ourField;
                TargetTable = targetTable;
                TargetField = targetField;
            }

            public string TargetTable { get; }
            public string TargetField { get; }
            public string OurField { get; }
        }

        public class SortField
        {
            public string Field { get; }
            public SortOrder SortOrder { get; }
            public int? SortIndex { get; }

            public SortField(string field, SortOrder sortOrder, int? sortIndex)
            {
                Field = field;
                SortOrder = sortOrder;
                SortIndex = sortIndex;
            }
        }

        private readonly List<string> _primaryKeys = new List<string>();
        private readonly List<string> _compareKeys = new List<string>();
        private readonly List<string> _unpredictableColumns = new List<string>();
        private readonly List<string> _predictableColumns = new List<string>();
        private readonly List<Reference> _references = new List<Reference>();
        private readonly List<string> _requiredColumns = new List<string>();
        private readonly List<string> _excludedColumns = new List<string>();
        private readonly List<string> _includedColumns = new List<string>();
        private readonly List<SortField> _sortColumns = new List<SortField>();
        private readonly List<string> _utcDateFields = new List<string>();
        private readonly List<string> _localDateFields = new List<string>();
        private List<SnapshotColumnInfo> _columns = new List<SnapshotColumnInfo>();

        /// <summary>
        /// Note, this can be null if the table was not defined using a type
        /// </summary>
        public ReadOnlyCollection<Type> DefiningTypes { get; }

        public bool ExcludeFromComparison { get; internal set; }
        public bool IncludeInComparison { get; internal set; }
        public string TableName { get; }
        public IEnumerable<string> PrimaryKeys => _primaryKeys;
        public IEnumerable<string> CompareKeys => _compareKeys;
        public IEnumerable<SnapshotColumnInfo> Columns => _columns;
        public IEnumerable<string> Unpredictable => _unpredictableColumns;
        public IEnumerable<string> Predictable => _predictableColumns;
        public IEnumerable<Reference> References => _references;
        public IEnumerable<string> RequiredColumns => _requiredColumns;
        public IEnumerable<string> IncludedColumns => _includedColumns;
        public IEnumerable<string> ExcludedColumns => _excludedColumns;
        public IEnumerable<SortField> SortColumns => _sortColumns;
        public IEnumerable<string> UtcDates => _utcDateFields;
        public IEnumerable<string> LocalDates => _localDateFields;

        public TableDefinition(string tableName, IEnumerable<Type> definingTypes = null)
        {
            TableName = tableName;
            DefiningTypes = new ReadOnlyCollection<Type>(definingTypes?.ToList() ?? new List<Type>());
        }

        internal void SetPrimaryKey(string fieldName)
        {
            var col = ColumnAdded(fieldName);
            col.IsPrimaryKey = true;
            if (!_primaryKeys.Contains(fieldName))
                _primaryKeys.Add(fieldName);
        }

        internal void SetCompareKey(string fieldName)
        {
            var col = ColumnAdded(fieldName);
            col.IsCompareKey = true;
            if (!_compareKeys.Contains(fieldName))
                _compareKeys.Add(fieldName);
        }

        internal void SetUnpredictable(string fieldName)
        {
            var col = ColumnAdded(fieldName);
            col.IsUnpredictable = true;
            col.IsPredictable = false;
            _unpredictableColumns.Add(fieldName);
        }

        internal void SetPredictable(string fieldName)
        {
            var col = ColumnAdded(fieldName);
            col.IsPredictable = true;
            col.IsUnpredictable = false;
            _predictableColumns.Add(fieldName);
        }

        internal void SetReference(string columnName, string referencedTableName, string referencedPropertyName)
        {
            var col = ColumnAdded(columnName);
            if (col.References(referencedTableName, referencedPropertyName))
            {
                var reference = new Reference(columnName, referencedTableName, referencedPropertyName);
                _references.Add(reference);
            }
        }

        internal void SetRequired(string fieldName)
        {
            var col = ColumnAdded(fieldName);
            col.IsRequired = true;
            _requiredColumns.Add(fieldName);
        }

        internal void ExcludeColumnFromComparison(string fieldName)
        {
            if (_excludedColumns.Contains(fieldName)) return;
            var col = ColumnAdded(fieldName);
            col.IsExcluded = true;
            _excludedColumns.Add(fieldName);
        }

        internal void IncludeColumnInComparison(string fieldName)
        {
            if (_includedColumns.Contains(fieldName)) return;
            var col = ColumnAdded(fieldName);
            col.IsIncluded = true;
            _includedColumns.Add(fieldName);
        }

        internal SnapshotColumnInfo ColumnAdded(string columnName, Type observedType = null)
        {
            var existing = _columns.SingleOrDefault(c => c.Name == columnName);
            if (existing == null)
            {
                existing = new SnapshotColumnInfo(columnName);
                _columns.Add(existing);
            }

            if (observedType != null)
                existing.TypeObserved(observedType);

            return existing;
        }

        internal void SetSortColumn(string fieldName, SortOrder order, int? sortSequencer = null)
        {
            var col = ColumnAdded(fieldName);
            col.SortDirection = order;
            col.SortIndex = sortSequencer;
            _sortColumns.Add(new SortField(fieldName, order, sortSequencer));
        }

        internal void SetDateType(string fieldName, DateTimeKind dateType)
        {
            var col = ColumnAdded(fieldName);
            col.DateType = dateType;
            if (dateType == DateTimeKind.Local)
            {
                _localDateFields.Add(fieldName);
                _utcDateFields.Remove(fieldName);
            }
            else if (dateType == DateTimeKind.Utc)
            {
                _utcDateFields.Add(fieldName);
                _localDateFields.Remove(fieldName);
            }
        }

        
    }
}