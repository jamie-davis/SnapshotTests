using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestSnapshotTable
    {
        private SnapshotTable _table;

        [SetUp]
        public void SetUp()
        {
            var tableDef = new TableDefinition("Test");
            var tableBuilder = new TableDefiner(tableDef);
            tableBuilder.CompareKey("Key");
            _table = new SnapshotTable(tableDef);
        }

        [Test]
        public void RowsCanBeAdded()
        {
            //Act
            var row = _table.AddRow();

            //Assert
            row.Should().BeOfType<SnapshotRow>();
        }

        [Test]
        public void RowsAreReported()
        {
            //Arrange
            var output = new Output();
            for (var i = 1; i <= 10; i++)
            {
                var row = _table.AddRow();
                row.SetField("Key", i);
                row.SetField("Surname", $"Surname {i}");
                row.SetField("FirstName", $"First Name {i}");
                row.SetField("TimeStamp", DateTime.Parse($"2020-03-21 11:06:{i:00}"));
                row.SetField("Double", (double)i / 3);
                row.SetField("Decimal", Math.Round((decimal)i / 3, 2));
                row.SetField("Bytes", Enumerable.Range(1, i).Select(n => (byte)i).ToArray());
            }

            //Act
            _table.ReportContents(output);
            
            //Assert
            output.Report.Verify();

        }
    }
}
