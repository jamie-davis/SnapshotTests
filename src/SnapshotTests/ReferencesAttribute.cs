using System;

namespace SnapshotTests
{
    /// <summary>
    /// Use this attribute to flag a field as referencing a key value in another table. If that key value in another table is flagged
    /// with the <see cref="UnpredictableAttribute"/>, values in the flagged property will be assigned the same replacement value as the
    /// key and the referenced row can be flagged to be included in the difference output as a "Referenced" row. This attribute should be
    /// used in a class flagged with the <see cref="SnapshotDefinitionAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ReferencesAttribute : Attribute
    {
        public ReferencesAttribute(string snapshotTableName, string keyPropertyName)
        {
            SnapshotTableName = snapshotTableName;
            KeyPropertyName = keyPropertyName;
        }

        public string SnapshotTableName { get; }
        public string KeyPropertyName { get; }
    }
}