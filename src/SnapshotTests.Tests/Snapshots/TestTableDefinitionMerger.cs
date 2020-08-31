using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Exceptions;
using SnapshotTests.Snapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestTableDefinitionMerger
    {
        private const string Table = "TestTable";

        [Test]
        public void DifferentTableNamesShouldThrow()
        {
            //Arrange
            var left = new TableDefinition("Left");
            var right = new TableDefinition("Right");
            
            //Act/Assert
            Action merge = () => TableDefinitionMerger.Merge(left, right);
            merge.Should()
                .Throw<MergeDefinitionsTargetDifferentTables>()
                .Where(e => e.Merge == "Right" && e.Main == "Left");
        }

        [Test]
        public void CorrectTableNameIsPreserved()
        {
            //Arrange
            var left = new TableDefinition(Table);
            var right = new TableDefinition(Table);

            //Act
            var result = TableDefinitionMerger.Merge(left, right);

            //Assert
            result.TableName.Should().Be(Table);
        }

        [Test]
        public void PrimaryKeyIsPreserved()
        {
            //Arrange
            var left = new TableDefinition(Table);
            var right = new TableDefinition(Table);

            left.SetPrimaryKey("A");
            left.SetPrimaryKey("B");
            left.SetPrimaryKey("C");

            //Act
            var result = TableDefinitionMerger.Merge(left, right);

            //Assert
            string.Join(", ", result.PrimaryKeys).Should().Be("A, B, C");
        }

        [Test]
        public void PrimaryKeyIsOverridden()
        {
            //Arrange
            var left = new TableDefinition(Table);
            var right = new TableDefinition(Table);

            left.SetPrimaryKey("A");
            left.SetPrimaryKey("B");
            left.SetPrimaryKey("C");

            right.SetPrimaryKey("X");
            right.SetPrimaryKey("Y");
            right.SetPrimaryKey("Z");

            //Act
            var result = TableDefinitionMerger.Merge(left, right);

            //Assert
            string.Join(", ", result.PrimaryKeys).Should().Be("X, Y, Z");
        }

        [Test]
        public void CompareKeyIsPreserved()
        {
            //Arrange
            var left = new TableDefinition(Table);
            var right = new TableDefinition(Table);

            left.SetCompareKey("A");
            left.SetCompareKey("B");
            left.SetCompareKey("C");

            //Act
            var result = TableDefinitionMerger.Merge(left, right);

            //Assert
            string.Join(", ", result.CompareKeys).Should().Be("A, B, C");
        }

        [Test]
        public void CompareKeyIsOverridden()
        {
            //Arrange
            var left = new TableDefinition(Table);
            var right = new TableDefinition(Table);

            left.SetCompareKey("A");
            left.SetCompareKey("B");
            left.SetCompareKey("C");

            right.SetCompareKey("X");
            right.SetCompareKey("Y");
            right.SetCompareKey("Z");

            //Act
            var result = TableDefinitionMerger.Merge(left, right);

            //Assert
            string.Join(", ", result.CompareKeys).Should().Be("X, Y, Z");
        }

        [Test]
        public void ReferencesAreMerged()
        {
            //Arrange
            var left = new TableDefinition(Table);
            var right = new TableDefinition(Table);

            left.SetReference("RefB", "B", "BKey");
            left.SetReference("RefD", "D", "DKey");

            right.SetReference("RefC", "C", "CKey");
            right.SetReference("RefD", "D", "DKey");

            //Act
            var result = TableDefinitionMerger.Merge(left, right);

            //Assert
            var output = new Output();
            TableDefinitionReporter.Report(result, output);
            output.Report.Verify();
        }

        [Test]
        public void RequiredColumnsAreMerged()
        {
            //Arrange
            var left = new TableDefinition(Table);
            var right = new TableDefinition(Table);

            left.SetRequired("Req1");

            right.SetRequired("Req2");

            //Act
            var result = TableDefinitionMerger.Merge(left, right);

            //Assert
            string.Join(", ", result.RequiredColumns).Should().Be("Req1, Req2");
        }

        [Test]
        public void UnpredictableColumnsAreMerged()
        {
            //Arrange
            var left = new TableDefinition(Table);
            var right = new TableDefinition(Table);

            left.SetUnpredictable("A");

            right.SetUnpredictable("B");

            //Act
            var result = TableDefinitionMerger.Merge(left, right);

            //Assert
            string.Join(", ", result.Unpredictable).Should().Be("A, B");
        }

        [Test]
        public void UnpredictableColumnsCanBeReversed()
        {
            //Arrange
            var left = new TableDefinition(Table);
            var right = new TableDefinition(Table);

            left.SetUnpredictable("A");
            left.SetUnpredictable("B");
            left.SetUnpredictable("C");

            right.SetPredictable("B");

            //Act
            var result = TableDefinitionMerger.Merge(left, right);

            //Assert
            string.Join(", ", result.Unpredictable).Should().Be("A, C");
        }
    }
}
