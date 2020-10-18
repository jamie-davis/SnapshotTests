using System;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.TestDoubles;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestRowBuilder
    {
        private const string TestTableName = "Test";
        private SnapshotCollection _collection;
        private SnapshotBuilder _snapshot;
        private RowBuilder _row;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
            _collection.DefineTable("Test").CompareKey("Id");
            _snapshot = _collection.NewSnapshot("Test");
            _row = _snapshot.AddNewRow("Test");
        }

        [Test]
        public void ARowBuilderIsCreated()
        {
            //Assert
            _row.Should().BeOfType<RowBuilder>();
        }

        [Test]
        public void ValuesCanBeRetrieved()
        {
            //Act
            _row["Id"] = 100;

            //Assert
            _row["Id"].Should().Be(100);
        }

        [Test]
        public void PrimaryKeyCanBeSet()
        {
            //Act
            _row["Id"] = 100;

            //Assert
            var output = new Output();
            _snapshot.ReportContents(output, "Test");
            output.Report.Verify();
        }

        [Test]
        public void NonKeyFieldCanBeSet()
        {
            //Act
            _row["NotKey"] = 100;

            //Assert
            var output = new Output();
            _snapshot.ReportContents(output, "Test");
            output.Report.Verify();
        }
    }
    [TestFixture]
    public class TestSnapshot
    {
        private const string TestTableName = "Test";
        private SnapshotCollection _collection;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
            _collection.DefineTable("Test").CompareKey("Id");
        }

        [Test]
        public void SnapshotHasTimestamp()
        {
            //Arrange
            _collection.NewSnapshot("Test");

            //Act
            var snapshot = _collection.GetSnapshot("Test");

            //Assert
            snapshot.SnapshotTimestamp.Should().BeAfter(DateTime.MinValue);
        }

        [Test]
        public void SnapshotTimestampAwaitsClockTick()
        {
            //Arrange
            var start = DateTime.Parse("2020-09-19 20:19");
            var ticked = DateTime.Parse("2020-09-19 20:19:00.001");
            _collection.SetTimeSource(new FakeTimeSource(start, start, start, start, start, start, ticked));
            var builder = _collection.NewSnapshot("Test");

            //Act
            var snapshot = _collection.GetSnapshot("Test");

            //Assert
            snapshot.SnapshotTimestamp.Should().Be(ticked);
        }
    }
}