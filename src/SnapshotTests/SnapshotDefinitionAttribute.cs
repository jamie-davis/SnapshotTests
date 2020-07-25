using System;

namespace SnapshotTests
{
    /// <summary>
    /// Use this attribute to flag a class as a static configuration class for a snapshot table. The attributed class will be scanned
    /// for configuration information to allow the snapshot collection to replace unpredictable values in difference output to
    /// make subsequent test runs emit the same differences even when certain properties have non-deterministic values.
    /// <para/>
    /// This allows snapshot configuration to be declared statically and shared, rather than requiring complex configurations to be
    /// explicitly specified whenever a test needs to use snapshots to analyse changes made by a class under test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SnapshotDefinitionAttribute : Attribute
    {
        public SnapshotDefinitionAttribute(string snapshotTableName)
        {
            SnapshotTableName = snapshotTableName;
        }

        public string SnapshotTableName { get; }
    }
}