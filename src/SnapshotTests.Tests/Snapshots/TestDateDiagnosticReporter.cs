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
    public class TestDateDiagnosticReporter
    {
        [Test]
        public void DateDiagnosticsAreReported()
        {
            //Arrange
            var collection = new SnapshotCollection();

            var beforeDates = new[]
            {
                DateTime.Parse("2020-10-11 10:35"),
                DateTime.Parse("2020-10-11 10:36"),
                DateTime.Parse("2020-10-11 10:37"),
                DateTime.Parse("2020-10-11 10:38"),
            };

            var afterDates = new[]
            {
                DateTime.Parse("2020-10-11 10:35"),
                DateTime.Parse("2020-10-11 10:36"),
                DateTime.Parse("2020-10-11 10:39"),
                DateTime.Parse("2020-10-11 10:40"),
                DateTime.Parse("2020-10-11 10:41"),
            };

            var beforeBuilder = collection.NewSnapshot("before");

            collection.DefineTable("Dates").PrimaryKey("Key").IsUnpredictable("Date");

            beforeDates.Select((bd, ix) => new { Key = ix, Date = bd, Other = "o", OtherDate = DateTime.MinValue}).ToSnapshotTable(beforeBuilder, "Dates");
            var deleteRow = beforeBuilder.AddNewRow("Dates");
            deleteRow["Key"] = 100;
            deleteRow["Date"] = DateTime.Parse("2020-10-18 11:19");
            deleteRow["Other"] = "o";
            deleteRow["OtherDate"] = DateTime.MinValue;
            
            var afterBuilder = collection.NewSnapshot("after");
            
            afterDates.Select((bd, ix) => new { Key = ix, Date = bd, Other = "a", OtherDate = DateTime.MaxValue}).ToSnapshotTable(afterBuilder, "Dates");

            var before = collection.GetSnapshot("before");
            var after = collection.GetSnapshot("after");
            var differences = SnapshotDifferenceAnalyser.ExtractDifferences(collection, before, after);

            var output = new Output();
            
            //Act
            var ranges = new List<NamedTimeRange>
            {
                new NamedTimeRange(beforeDates[0], beforeDates[3], "before"),
                new NamedTimeRange(afterDates[2], afterDates[3], "after"),
            };
            DateDiagnosticReporter.Report(output, ranges, before, after, differences);

            //Assert
            output.Report.Verify();
        }
    }
}
