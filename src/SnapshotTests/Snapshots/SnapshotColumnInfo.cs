using System;
using System.Collections.Generic;
using System.Linq;
using SnapshotTests.Exceptions;
using TestConsoleLib.Exceptions;

namespace SnapshotTests.Snapshots
{
    public class SnapshotColumnInfo
    {
        private readonly object _lock = new object();

        private readonly List<Type> _observedTypes = new List<Type>();

        /// <summary>
        /// The name of the field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// This field is part of the primary key for the object, and can reliably be used to identify a unique instance.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// This field is flagged as forming part of a key that will work for comparison. Generally, this is not the primary key, and is instead a
        /// field that will be unique within the scope of the test data only.
        /// </summary>
        public bool IsCompareKey { get; set; }

        /// <summary>
        /// The field has a value that is programmatically assigned and cannot be predicted under normal circumstances. For example, Guids, Timestamps and
        /// incrementing keys.
        /// </summary>
        public bool IsUnpredictable { get; set; }

        /// <summary>
        /// The field has a value that can be predicted for the purposes of unit testing, even if it is of a type that is normally unpredictable.
        /// <remarks>This is useful to override an automated or default setting when needed.</remarks>
        /// </summary>
        public bool IsPredictable { get; set; }

        /// <summary>
        /// The name of the table containing a field referenced by this column. If this is set, <see cref="ReferencedPropertyName"/> must also be set.
        /// </summary>
        public string ReferencedTableName { get; private set; }

        /// <summary>
        /// The name of a field referenced by this column. If this is set, <see cref="ReferencedTableName"/> must also be set.
        /// </summary>
        public string ReferencedPropertyName { get; private set; }

        /// <summary>
        /// The C# types containing this field. This is optionally recorded when snapshots are constructed automatically.
        /// </summary>
        public IEnumerable<Type> ObservedTypes
        {
            get
            {
                lock (_lock) 
                    return _observedTypes.ToList();
            }
        }

        /// <summary>
        /// Indicates whether this field is mandatory in difference output for the table.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Indicates that this field should be excluded from comparisons. By default all fields are included, this indicator will
        /// cause the column to be ignored in difference calculation. If an excluded column must later be included, set <see cref="IsIncluded"/>
        /// to override this indicator.
        /// <remarks>If <see cref="IsExcluded"/> and <see cref="IsIncluded"/> are both set, the field will be included.</remarks>
        /// </summary>
        public bool IsExcluded { get; set; }

        /// <summary>
        /// Indicates whether this field is explicitly included in comparisons. This is used to override a the exclusion of a column.
        /// If the column was not excluded, this will have no effect.
        /// <remarks>If <see cref="IsExcluded"/> and <see cref="IsIncluded"/> are both set, the field will be included.</remarks>
        /// </summary>
        public bool IsIncluded { get; set; }

        /// <summary>
        /// If this field is part of the sort order for the table
        /// </summary>
        public SortOrder? SortDirection { get; set; }

        /// <summary>
        /// If this field is part of the sort for the table, this indicates the order in which the field sorts should be applied.
        /// </summary>
        public int? SortIndex { get; set; }

        /// <summary>
        /// This indicates whether the field contains a UTC date or a local date. This will be null if no kind has been specified.
        /// </summary>
        public DateTimeKind? DateType { get; set; }

        public SnapshotColumnInfo(string columnName)
        {
            Name = columnName;
        }

        /// <summary>
        /// Called when the type from which the field receives a value is known.
        /// </summary>
        public void TypeObserved(Type observedType)
        {
            lock (_lock)
            {
                if (!_observedTypes.Contains(observedType))
                    _observedTypes.Add(observedType);
            }
        }

        /// <summary>
        /// Called to indicate a "foreign key" style reference to a field in another table. This is important when obfuscating unpredictable values, and for
        /// ensuring that comparisons are able to show rows that do not have changes but are referenced by an update or insert.
        /// </summary>
        /// <exception cref="AmbiguosReferenceInSnapshotColumn">Thrown if the reference is already set to a different table and field.</exception>
        /// <returns>True if a reference was added, false if it wasn't (indicating the reference is already set.</returns>
        public bool References(string referencedTableName, string referencedPropertyName)
        {
            if (ReferencedTableName == referencedTableName && ReferencedPropertyName == referencedPropertyName)
                return false;

            if (ReferencedTableName != null)
                throw new AmbiguosReferenceInSnapshotColumn(Name, ReferencedTableName, ReferencedPropertyName, referencedTableName, referencedPropertyName);

            ReferencedTableName = referencedTableName;
            ReferencedPropertyName = referencedPropertyName;

            return true;
        }
    }
}