using System;

namespace SnapshotTests.Exceptions
{
    public class AmbiguosReferenceInSnapshotColumn : Exception
    {
        public string FieldName { get; }
        public string ReferencedTableName { get; }
        public string ReferencedPropertyName { get; }
        public string ConflictingTableName { get; }
        public string ConflictingPropertyName { get; }

        public AmbiguosReferenceInSnapshotColumn(string fieldName, string referencedTableName, string referencedPropertyName, string conflictingTableName, string conflictingPropertyName)
        : base("Field reference is already set to a different table and field")
        {
            FieldName = fieldName;
            ReferencedTableName = referencedTableName;
            ReferencedPropertyName = referencedPropertyName;
            ConflictingTableName = conflictingTableName;
            ConflictingPropertyName = conflictingPropertyName;
        }
    }
}
