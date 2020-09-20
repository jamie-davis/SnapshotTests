using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using SnapshotTests.Tests.TestDoubles;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestTimeRangeExtractor
    {
        private SnapshotObjectMother _om;

        [SetUp]
        public void SetUp()
        {
            _om = new SnapshotObjectMother();
            _om.Collection.SetTimeSource(new FakeTimeSource(
                "2020-09-20 08:53:24",
                "2020-09-20 08:55:06",
                "2020-09-20 08:55:21",
                "2020-09-20 08:55:28",
                "2020-09-20 08:55:44",
                "2020-09-20 08:55:54",
                "2020-09-20 09:01:53"));
        }

        [Test]
        public void RangesAreExtractedFromSnapshots()
        {
            //Arrange
            var table1 = _om.Make<SnapshotObjectMother.Table>(1, 2, 3, 4);

            void TakeSnapshot(string name)
            {
                var builder = _om.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
            }

            TakeSnapshot("FirstSnapshot");
            TakeSnapshot("SecondSnapshot");
            TakeSnapshot("ThirdSnapshot");
            
            //Act
            var result = TimeRangeExtractor.Extract(_om.Collection);

            //Assert
            var output = new Output();
            output.FormatTable(result);
            output.Report.Verify();

        }
    }
}
