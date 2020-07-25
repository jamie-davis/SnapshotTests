using NUnit.Framework;
using SnapshotTests.Snapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestSnapshotExtensions
    {
        private SnapshotCollection _collection;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
        }

        [Test]
        public void CollectionsCanBePlacedInSnapshots()
        {
            //Arrange
            var data = new[]
            {
                new {A = 1, B = "Bee", C = 5.5},
                new {A = 2, B = "Cee", C = 6.6},
                new {A = 3, B = "Dee", C = 7.7},
                new {A = 4, B = "Eee", C = 8.8},
                new {A = 5, B = "Eff", C = 9.9},
            };

            var snapshot = _collection.NewSnapshot("Test");

            //Act
            data.ToSnapshotTable(snapshot, "Data");

            //Assert
            var output = new Output();
            _collection.GetSnapshot("Test").ReportContents(output);
            output.Report.Verify();
        }

        [Test]
        public void TablesCanBeAutomaticallyDefined()
        {
            //Arrange
            var data = new[]
            {
                new {A = 1, B = "Bee", C = 5.5},
                new {A = 2, B = "Cee", C = 6.6},
                new {A = 3, B = "Dee", C = 7.7},
                new {A = 4, B = "Eee", C = 8.8},
                new {A = 5, B = "Eff", C = 9.9},
            };

            var snapshot = _collection.NewSnapshot("Test");

            //Act
            data.ToSnapshotTable(snapshot, "Data", "A", "B");

            //Assert
            var output = new Output();
            var table = _collection.GetTableDefinition("Data");
            _collection.GetSchemaReport(output);
            output.Report.Verify();
        }
    }
}
