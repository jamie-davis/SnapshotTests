using System;
using System.Linq;
using NUnit.Framework;
using SnapshotTests.Snapshots.ValueTrackers;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots.ValueTrackers
{
    [TestFixture]
    public class TestDefaultValueTracker
    {
        [Test]
        public void ValuesAreSubstituted()
        {
            //Arrange
            var values = new object[]
            {
                "Value 1",
                2,
                DateTime.Parse("2020-07-12 10:38"),
                "Value 1",
                2,
                DateTime.Parse("2020-07-12 10:38"),
                "Value 2",
                3,
                DateTime.Parse("2020-07-12 10:39"),
                "Value 2",
                3,
                DateTime.Parse("2020-07-12 10:39"),
                Guid.Parse("F1684874-FF8C-4D0D-A767-E1EC90F36F8D"),
                null,
                null
            };

            var sub = new DefaultValueTracker("TestCol");

            //Act
            var result = values.Select(v => new { Value = v, Substitute = sub.GetSubstitute(v)});

            //Assert
            var output = new Output();

            output.WrapLine("Substitutions:");
            output.WriteLine();
            output.FormatTable(result);

            output.WriteLine();
            output.WrapLine("Unique Substitution Values:");
            output.WriteLine();
            output.FormatTable(result.Select(r => r.Substitute).Distinct().Select(s => new { UniqueValue = s }));

            output.Report.Verify();
        }
    }
}
