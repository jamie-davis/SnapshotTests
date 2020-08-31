using System;
using System.Collections.Generic;
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

        private readonly List<string> _primaryKeys = new List<string>();
        private readonly List<string> _compareKeys = new List<string>();
        private readonly List<string> _unpredictableFields = new List<string>();
        private readonly List<string> _predictableFields = new List<string>();
        private readonly List<Reference> _references = new List<Reference>();
        private readonly List<string> _requiredColumns = new List<string>();
        private List<SnapshotColumnInfo> _columns = new List<SnapshotColumnInfo>();

        public string TableName { get; }
        public IEnumerable<string> PrimaryKeys => _primaryKeys;
        public IEnumerable<string> CompareKeys => _compareKeys;
        public IEnumerable<SnapshotColumnInfo> Columns => _columns;
        public IEnumerable<string> Unpredictable => _unpredictableFields;
        public IEnumerable<string> Predictable => _predictableFields;
        public IEnumerable<Reference> References => _references;
        public IEnumerable<string> RequiredColumns => _requiredColumns;

        public TableDefinition(string tableName)
        {
            TableName = tableName;
        }

        public void SetPrimaryKey(string fieldName)
        {
            var col = ColumnAdded(fieldName);
            col.IsPrimaryKey = true;
            if (!_primaryKeys.Contains(fieldName))
                _primaryKeys.Add(fieldName);
        }

        public void SetCompareKey(string fieldName)
        {
            var col = ColumnAdded(fieldName);
            col.IsCompareKey = true;
            if (!_compareKeys.Contains(fieldName))
                _compareKeys.Add(fieldName);
        }

        public void SetUnpredictable(string fieldName)
        {
            var col = ColumnAdded(fieldName);
            col.IsUnpredictable = true;
            col.IsPredictable = false;
            _unpredictableFields.Add(fieldName);
        }

        public void SetPredictable(string fieldName)
        {
            var col = ColumnAdded(fieldName);
            col.IsPredictable = true;
            col.IsUnpredictable = false;
            _predictableFields.Add(fieldName);
        }

        public void SetReference(string columnName, string referencedTableName, string referencedPropertyName)
        {
            var col = ColumnAdded(columnName);
            if (col.References(referencedTableName, referencedPropertyName))
            {
                var reference = new Reference(columnName, referencedTableName, referencedPropertyName);
                _references.Add(reference);
            }
        }

        public void SetRequired(string fieldName)
        {
            var col = ColumnAdded(fieldName);
            col.IsRequired = true;
            _requiredColumns.Add(fieldName);
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
    }
}