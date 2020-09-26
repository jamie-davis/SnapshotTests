using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Snapshots.ValueTrackers;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots.ValueTrackers
{
    [TestFixture]
    public class TestDateTimeValueTracker
    {
        [Test]
        public void ValuesAreSubstituted()
        {
            //Arrange
            var values = new object[]
            {
                DateTime.Parse("2020-05-12 11:10"),
                DateTime.Parse("2020-05-12 11:11"),
                DateTime.Parse("2020-05-12 11:12"),
                DateTime.Parse("2020-07-12 10:38"),
                DateTime.Parse("2020-07-12 10:38"),
                DateTime.Parse("2020-07-12 10:39"),
                DateTime.Parse("2020-07-12 10:39"),
                DateTime.Parse("2020-07-12 10:38:01"),
                DateTime.Parse("2020-07-12 10:38:02"),
                DateTime.Parse("2020-07-12 10:39:03"),
                DateTime.Parse("2020-07-12 10:39:04"),
                null,
                55
            };


            var ranges = new List<NamedTimeRange>
            {
                new NamedTimeRange(DateTime.Parse("2020-07-12 10:37"), DateTime.Parse("2020-07-12 10:38"), "First"),
                new NamedTimeRange(DateTime.Parse("2020-07-12 10:38:00.001"), DateTime.Parse("2020-07-12 10:39"), "Second"),
                new NamedTimeRange(DateTime.Parse("2020-07-12 10:39:00.001"), DateTime.Parse("2020-07-12 10:55"), "Third"),
            };
            var sub = new DateTimeValueTracker(new SnapshotColumnInfo("TestCol"), ranges);

            //Act
            var result = values.Select(v => new { Value = v ?? "<NULL>", Substitute = sub.GetSubstitute(v) ?? "<NULL>"});

            //Assert
            var output = new Output();

            output.WrapLine("Ranges:");
            output.WriteLine();
            output.FormatTable(ranges);

            output.WriteLine();
            output.WrapLine("Substitutions:");
            output.WriteLine();
            output.FormatTable(result);

            output.Report.Verify();
        }

        [Test]
        public void LocalValuesAreSubstituted()
        {
            //Arrange
            var values = new DateTime?[]
            {
                DateTime.Parse("2020-05-12 11:10"),
                DateTime.Parse("2020-05-12 11:11"),
                DateTime.Parse("2020-05-12 11:12"),
                DateTime.Parse("2020-07-12 10:38"),
                DateTime.Parse("2020-07-12 10:38"),
                DateTime.Parse("2020-07-12 10:39"),
                DateTime.Parse("2020-07-12 10:39"),
                DateTime.Parse("2020-07-12 10:38:01"),
                DateTime.Parse("2020-07-12 10:38:02"),
                DateTime.Parse("2020-07-12 10:39:03"),
                DateTime.Parse("2020-07-12 10:39:04"),
                null
            };


            var ranges = new List<NamedTimeRange>
            {
                new NamedTimeRange(DateTime.Parse("2020-07-12 10:37"), DateTime.Parse("2020-07-12 10:38"), "First"),
                new NamedTimeRange(DateTime.Parse("2020-07-12 10:38:00.001"), DateTime.Parse("2020-07-12 10:39"), "Second"),
                new NamedTimeRange(DateTime.Parse("2020-07-12 10:39:00.001"), DateTime.Parse("2020-07-12 10:55"), "Third"),
            };
            var sub = new DateTimeValueTracker(new SnapshotColumnInfo("TestCol") { DateType = DateTimeKind.Local }, ranges);

            //Act
            var result = values
                .Select(v => new {UTC = (object) v, Value = (object) v?.ToLocalTime()})
                .Select(v => new
                {
                    UTC = v.UTC ?? "<NULL>", 
                    Value = v.Value == null ? "<NULL>" : "(localised date/time)",
                    Substitute = sub.GetSubstitute(v.Value) ?? "<NULL>"
                });

            //Assert
            var output = new Output();

            output.WrapLine("Ranges:");
            output.WriteLine();
            output.FormatTable(ranges);

            output.WriteLine();
            output.WrapLine("Substitutions:");
            output.WriteLine();
            output.FormatTable(result);

            output.Report.Verify();
        }
    }
}