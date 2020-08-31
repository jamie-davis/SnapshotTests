using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestDefinitionSetMerger
    {
        #region Types that define tables

        [SnapshotDefinition("LocalTable")]
        public static class A_TableDef
        {
            [Unpredictable] 
            public static int Id { get; set; }
        }

        [SnapshotDefinition("PredictableTable")]
        public static class A_PredictableTable
        {
            [Predictable] 
            public static int Id { get; set; }
        }

        [SnapshotDefinition("LocalTable")]
        public static class B_TableDef
        {
            [Unpredictable] 
            public static int Id { get; set; }
        }

        [SnapshotDefinition("PredictableTable")]
        public static class B_PredictableTable
        {
            [Predictable] 
            public static int Id { get; set; }
            [References("LocalTable", "Id")]
            public static int Ref { get; set; }
        }

        [SnapshotDefinition("AddedTable")]
        public static class B_AddedTable
        {
            [Predictable] 
            public static int Id { get; set; }
        }

        #endregion

        [Test]
        public void DefinitionsAreLoadedFromAssembly()
        {
            //Arrange
            var definition = SnapshotDefinitionLoader.Load(GetType(), t => t.Name.StartsWith("A_"));

            //Act
            var loaded = DefinitionSetMerger.Merge(definition, null).ToList();

            //Assert
            var output = new Output();
            TableDefinitionReporter.Report(loaded, output);
            output.Report.Verify();
        }

        [Test]
        public void DefinitionsAreMerged()
        {
            //Arrange
            var definition = SnapshotDefinitionLoader.Load(GetType(), t => t.Name.StartsWith("A_"));
            var definition2 = SnapshotDefinitionLoader.Load(GetType(), t => t.Name.StartsWith("B_"));
            var loaded = DefinitionSetMerger.Merge(definition, null).ToList();

            //Act
            var result = DefinitionSetMerger.Merge(definition2, loaded).ToList();

            //Assert
            var output = new Output();
            TableDefinitionReporter.Report(result, output);
            output.Report.Verify();
        }
    }
}
