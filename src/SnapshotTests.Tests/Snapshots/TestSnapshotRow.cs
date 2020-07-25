using NUnit.Framework;
using SnapshotTests.Snapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestSnapshotRow
    {
        private SnapshotCollection _collection;
        private SnapshotBuilder _snapshot;
        private RowBuilder _row;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
            _collection.DefineTable("Test").CompareKey("Key");
            _snapshot = _collection.NewSnapshot("TestSnapshot");
            _row = _snapshot.AddNewRow("Test");
        }

        [Test]
        public void FieldsAreSetInTheRow()
        {
            //Act
            _row["Key"] = "key";
            _row["Field"] = "value";

            //Assert
            var output = new Output();
            _snapshot.ReportContents(output, "Test");
            output.Report.Verify();
        }
    }
}
