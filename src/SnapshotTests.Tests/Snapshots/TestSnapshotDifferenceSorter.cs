using System.Configuration;
using System.Linq;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestSnapshotDifferenceSorter
    {
        [Test]
        public void DifferenceDataIsSorted()
        {
            //Arrange
            var collection = new SnapshotCollection();
            collection.DefineTable("TheTable").PrimaryKey("Id").Sort("Name");

            void AddRow(SnapshotBuilder snapshot, int id, string name)
            {
                var add = snapshot.AddNewRow("TheTable");
                add["Id"] = id;
                add["Name"] = name;
            }

            var beforeBuilder = collection.NewSnapshot("Before");
            AddRow(beforeBuilder, 1, "Zed");
            AddRow(beforeBuilder, 2, "Xavier");
            AddRow(beforeBuilder, 3, "Alice");

            var afterBuilder = collection.NewSnapshot("After");
            AddRow(afterBuilder, 1, "Zed+");
            AddRow(afterBuilder, 2, "Xavier+");
            AddRow(afterBuilder, 3, "Alice+");

            var before = collection.GetSnapshot("Before");
            var after = collection.GetSnapshot("After");
            var diffs = SnapshotDifferenceAnalyser.ExtractDifferences(collection, before, after);

            //Act
            var result = SnapshotDifferenceSorter.SortDifferences(collection, diffs.TableDifferences.ToList());

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();

        }

        [Test]
        public void DifferenceDataIsSortedDescending()
        {
            //Arrange
            var collection = new SnapshotCollection();
            collection.DefineTable("TheTable").PrimaryKey("Id").SortDescending("Name");

            void AddRow(SnapshotBuilder snapshot, int id, string name)
            {
                var add = snapshot.AddNewRow("TheTable");
                add["Id"] = id;
                add["Name"] = name;
            }

            var beforeBuilder = collection.NewSnapshot("Before");
            AddRow(beforeBuilder, 1, "Alice");
            AddRow(beforeBuilder, 2, "Xavier");
            AddRow(beforeBuilder, 3, "Zed");

            var afterBuilder = collection.NewSnapshot("After");
            AddRow(afterBuilder, 1, "Alice+");
            AddRow(afterBuilder, 2, "Xavier+");
            AddRow(afterBuilder, 3, "Zed+");

            var before = collection.GetSnapshot("Before");
            var after = collection.GetSnapshot("After");
            var diffs = SnapshotDifferenceAnalyser.ExtractDifferences(collection, before, after);

            //Act
            var result = SnapshotDifferenceSorter.SortDifferences(collection, diffs.TableDifferences.ToList());

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }

        [Test]
        public void DifferenceDataIsSortedByMultipleColumns()
        {
            //Arrange
            var collection = new SnapshotCollection();
            collection.DefineTable("TheTable")
                .PrimaryKey("Id")
                .Sort("ColAsc1")
                .SortDescending("ColDesc1")
                .Sort("ColAsc2")
                .SortDescending("ColDesc2");

            void AddRow(SnapshotBuilder snapshot, int id, string asc1, string desc1, string asc2, string desc2)
            {
                var add = snapshot.AddNewRow("TheTable");
                add["Id"] = id;
                add["ColAsc1"] = asc1;
                add["ColDesc1"] = desc1;
                add["ColAsc2"] = asc2;
                add["ColDesc2"] = desc2;
            }

            void AddData(SnapshotBuilder snapshot, string ascData, string descData)
            {
                var id = 1;
                foreach (var colAsc1 in ascData.Split(','))
                {
                    foreach (var colDesc1 in descData.Split(','))
                    {
                        foreach (var colAsc2 in ascData.Split(','))
                        {
                            foreach (var colDesc2 in descData.Split(','))
                            {
                                AddRow(snapshot, id++, colAsc1, colDesc1, colAsc2, colDesc2);
                            }
                        }
                    }
                }
            }

            var beforeBuilder = collection.NewSnapshot("Before");
            AddData(beforeBuilder, "A,B,C", "C,B,A");

            var afterBuilder = collection.NewSnapshot("After");
            AddData(afterBuilder, "A+,B+,C+", "C+,B+,A+");

            var before = collection.GetSnapshot("Before");
            var after = collection.GetSnapshot("After");
            var diffs = SnapshotDifferenceAnalyser.ExtractDifferences(collection, before, after);

            //Act
            var result = SnapshotDifferenceSorter.SortDifferences(collection, diffs.TableDifferences.ToList());

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }
    }
}