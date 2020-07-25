using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SnapshotTests.Exceptions;
using SnapshotTests.Snapshots;
using TestConsole.OutputFormatting;
using TestConsoleLib;

namespace SnapshotTests
{
    /// <summary>
    /// This class allows snapshots to be stored and compared. Create one of these and push snapshots of data into it.
    /// </summary>
    public class SnapshotCollection
    {
        private Dictionary<string, TableDefinition> _tableDefinitions = new Dictionary<string, TableDefinition>();
        private Dictionary<string, Snapshot> _snapshots = new Dictionary<string, Snapshot>();
        private List<TableDefinition> _tablesInDefinitionOrder = new List<TableDefinition>();
        internal IEnumerable<TableDefinition> TablesInDefinitionOrder => _tablesInDefinitionOrder;

        public SnapshotCollection()
        {
        }

        /// <summary>
        /// Load the collection configuration from the specified assembly. Optionally restrict the types considered using the filter expression.
        /// </summary>
        /// <param name="configAssembly">The assembly that contains the configuration</param>
        /// <param name="typeFilter">Optional expression used to limit the types included. For example, you may wish to split configurations
        /// by namespace, and use this filter to select only the namespace you intend to configure your collection. If no filter is supplied
        /// then all configuration types in the assembly will be included.</param>
        public SnapshotCollection(Assembly configAssembly, Func<Type, bool> typeFilter = null)
        {
            var loaded = SnapshotDefinitionLoader.Load(configAssembly, typeFilter);
            LoadTableDefinitionSet(loaded);
        }

        /// <summary>
        /// Load the collection configuration from nested types within the specified type. This is useful when you wish to configure a snapshot
        /// collection using a self contained set of configuration specifications. For example, to configure a collection privately for a test
        /// fixture, you could specify configuration types as nested types within the fixture type, and specify the fixture type as the <see cref="configScope"/>.
        /// </summary>
        /// <param name="configScope">The type that contains the nested configuration types.</param>
        /// <param name="typeFilter">Optional expression used to limit the types included.</param>
        public SnapshotCollection(Type configScope, Func<Type, bool> typeFilter = null)
        {
            var loaded = SnapshotDefinitionLoader.Load(configScope, typeFilter);
            LoadTableDefinitionSet(loaded);
        }

        private void LoadTableDefinitionSet(DefinitionSet loaded)
        {
            _tablesInDefinitionOrder = loaded.Tables.ToList();
            _tableDefinitions = _tablesInDefinitionOrder.ToDictionary(t => t.TableName, t => t);
        }

        /// <summary>
        /// Define the tables that will form part of the snapshots. 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public TableDefiner DefineTable(string tableName)
        {
            if (!_tableDefinitions.TryGetValue(tableName, out var tableDefinition))
            {
                tableDefinition = new TableDefinition(tableName);
                _tableDefinitions[tableName] = tableDefinition;
                _tablesInDefinitionOrder.Add(tableDefinition);
            }

            return new TableDefiner(tableDefinition);
        }

        public SnapshotBuilder NewSnapshot(string name)
        {
            if (!_snapshots.TryGetValue(name, out var snapshot))
            {
                snapshot = new Snapshot(name);
                _snapshots[name] = snapshot;
            }

            return new SnapshotBuilder(snapshot, this);
        }

        public void GetSchemaReport(Output output)
        {
            var report = _tableDefinitions.Values.AsReport(rep => rep
                .AddColumn(t => t.TableName)
                .AddColumn(t => string.Join((string) " + ", (IEnumerable<string>) t.PrimaryKeys), cc => cc.Heading("Primary Key"))
                .AddColumn(t => string.Join((string) " + ", (IEnumerable<string>) t.CompareKeys), cc => cc.Heading("Compare Key"))
                .Title("Tables Defined")
            );

            output.FormatTable(report);
        }

        internal TableDefinition GetTableDefinition(string tableName)
        {
            if (_tableDefinitions.TryGetValue(tableName, out var definition))
                return definition;

            return null;
        }

        internal Snapshot GetSnapshot(string snapshotName)
        {
            if (_snapshots.TryGetValue(snapshotName, out var snapshot))
                return snapshot;

            return null;
        }

        public void ReportChanges(string beforeSnapshot, string afterSnapshot, Output output)
        {
            if (!_snapshots.TryGetValue(beforeSnapshot, out var before))
            {
                throw new SnapshotNotFoundException(beforeSnapshot);
            }

            if (!_snapshots.TryGetValue(afterSnapshot, out var after))
            {
                throw new SnapshotNotFoundException(afterSnapshot);
            }

            SnapshotComparer.ReportDifferences(this, before, after, output);
        }

    }
}
