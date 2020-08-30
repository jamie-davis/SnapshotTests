using NUnit.Framework;
using SnapshotTests.Snapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestSnapshotDefinitionLoader
    {
        #region Types that define tables

        [SnapshotDefinition("LocalTable")]
        public static class TableDef
        {
            [Unpredictable] 
            public static int Id { get; set; }
        }

        [SnapshotDefinition("LocalParentTable")]
        public static class ParentTableDef
        {
            [Unpredictable] 
            public static int Id { get; set; }
        }

        [SnapshotDefinition("LocalReferencingTable")]
        public static class RelatedTableDef
        {
            [Unpredictable] public static int Id { get; set; }
            
            [References("ParentTable", nameof(ParentTableDef.Id))] //implied unpredictable because the referenced property is unpredictable
            public static int ParentId1 { get; set; }
        }

        [SnapshotDefinition("PredictableTable")]
        public static class PredictableTable
        {
            [Predictable] 
            public static int Id { get; set; }
        }

        #endregion

        [Test]
        public void DefinitionsAreLoadedFromAssembly()
        {
            //Arrange
            var assembly = GetType().Assembly;

            //Act
            var definition = SnapshotDefinitionLoader.Load(assembly);

            //Assert
            var output = new Output();
            TableDefinitionReporter.Report(definition.Tables, output);
            output.Report.Verify();
        }

        [Test]
        public void DefinitionsAreLoadedFromEnclosingType()
        {
            //Act
            var definition = SnapshotDefinitionLoader.Load(GetType());

            //Assert
            var output = new Output();
            TableDefinitionReporter.Report(definition.Tables, output);
            output.Report.Verify();
        }

        [Test]
        public void DefinitionsAreFilteredWhenFromAssembly()
        {
            //Arrange
            var assembly = GetType().Assembly;

            //Act
            var definition = SnapshotDefinitionLoader.Load(assembly, t => t != typeof(TestDefinitions.TableDef));

            //Assert
            var output = new Output();
            TableDefinitionReporter.Report(definition.Tables, output);
            output.Report.Verify();
        }

        [Test]
        public void DefinitionsAreFilteredWhenLoadedFromEnclosingType()
        {
            //Arrange
            var assembly = GetType().Assembly;

            //Act
            var definition = SnapshotDefinitionLoader.Load(GetType(), t => t != typeof(TableDef));

            //Assert
            var output = new Output();
            TableDefinitionReporter.Report(definition.Tables, output);
            output.Report.Verify();
        }
    }
}
