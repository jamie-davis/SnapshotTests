﻿using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Exceptions;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests
{
    [TestFixture]
    public class TestSnapshotCollection
    {
        #region Types that define tables

        [SnapshotDefinition("LocalTable")]
        public static class TableDef
        {
            [Unpredictable] 
            [Key] 
            public static int Id { get; set; }
        }

        [SnapshotDefinition("LocalParentTable")]
        public static class ParentTableDef
        {
            [Unpredictable] 
            [Key] 
            public static int Id { get; set; }
        }

        [SnapshotDefinition("LocalReferencingTable")]
        public static class RelatedTableDef
        {
            [Unpredictable]
            [Key] 
            public static int Id { get; set; }
            
            [References("ParentTable", nameof(ParentTableDef.Id))] //implied unpredictable because the referenced property is unpredictable
            public static int ParentId1 { get; set; }
        }

        #endregion

        [Test]
        public void TablesAreDefined()
        {
            //Arrange
            var snapshots = new SnapshotCollection();

            //Act
            snapshots.DefineTable("Customer").PrimaryKey("Id").CompareKey("Surname");
            snapshots.DefineTable("Address").PrimaryKey("AddressId");
            
            //Assert
            var output = new Output();
            snapshots.GetSchemaReport(output);
            output.Report.Verify();
        }

        [Test]
        public void TableDefinitionsAreLoadedFromAssembly()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType().Assembly);
            
            //Assert
            var output = new Output();
            snapshots.GetSchemaReport(output);
            output.Report.Verify();
        }

        [Test]
        public void TableDefinitionsAreFilteredLoadedFromAssembly()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType().Assembly, t => t != typeof(Snapshots.TestDefinitions.TableDef));
            
            //Assert
            var output = new Output();
            snapshots.GetSchemaReport(output);
            output.Report.Verify();
        }

        [Test]
        public void TableDefinitionsAreLoadedFromType()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType());
            
            //Assert
            var output = new Output();
            snapshots.GetSchemaReport(output);
            output.Report.Verify();
        }

        [Test]
        public void TableDefinitionsAreFilteredLoadedFromType()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType(), t => t != typeof(Snapshots.TestDefinitions.TableDef));
            
            //Assert
            var output = new Output();
            snapshots.GetSchemaReport(output);
            output.Report.Verify();
        }

        [Test]
        public void ReportChangesThrowsWhenBeforeSnapshotNotPresent()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType(), t => t != typeof(Snapshots.TestDefinitions.TableDef));
            snapshots.DefineTable("Customer")
                .PrimaryKey("Id")
                .CompareKey("Surname")
                .IsUnpredictable("Id");

            var customers = new[]
            {
                new { Id = 1, Surname = "S1", FirstName = "F1", Age = 40 },
            };
            var builder = snapshots.NewSnapshot("After");
            customers.ToSnapshotTable(builder, "Customer");
            
            var output = new Output();

            //Assert
            Action action = ()=> snapshots.ReportChanges("Before", "After", output);
            action.Should().Throw<SnapshotNotFoundException>().Where(x => x.SnapshotName == "Before");
        }

        [Test]
        public void ReportChangesThrowsWhenAfterSnapshotNotPresent()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType(), t => t != typeof(Snapshots.TestDefinitions.TableDef));
            snapshots.DefineTable("Customer")
                .PrimaryKey("Id")
                .CompareKey("Surname")
                .IsUnpredictable("Id");

            var customers = new[]
            {
                new { Id = 1, Surname = "S1", FirstName = "F1", Age = 40 },
            };
            var builder = snapshots.NewSnapshot("Before");
            customers.ToSnapshotTable(builder, "Customer");
            
            var output = new Output();

            //Assert
            Action action = ()=> snapshots.ReportChanges("Before", "After", output);
            action.Should().Throw<SnapshotNotFoundException>().Where(x => x.SnapshotName == "After");
        }

        [Test]
        public void SnapshotThrowsWhenUndefinedTableUsed()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType(), t => t != typeof(Snapshots.TestDefinitions.TableDef));
            snapshots.DefineTable("Customer")
                .PrimaryKey("Id")
                .CompareKey("Surname")
                .IsUnpredictable("Id");

            var builder = snapshots.NewSnapshot("Before");

            //Assert
            Action action = () => builder.AddNewRow("Wrong");
            action.Should().Throw<UndefinedTableInSnapshotException>().Where(x => x.TableName == "Wrong");
        }

        [Test]
        public void GetSnapshotReturnsNullIfSnapshotNotPresent()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType(), t => t != typeof(Snapshots.TestDefinitions.TableDef));
            snapshots.DefineTable("Customer")
                .PrimaryKey("Id")
                .CompareKey("Surname")
                .IsUnpredictable("Id");

            var builder = snapshots.NewSnapshot("Before");

            //Assert
            snapshots.GetSnapshot("Wrong").Should().BeNull();
        }

        [Test]
        public void SnapshotRowsInMissingTablesAreNull()
        {
            //Arrange
            var snapshots = new SnapshotCollection(GetType(), t => t != typeof(Snapshots.TestDefinitions.TableDef));
            snapshots.DefineTable("Customer")
                .PrimaryKey("Id")
                .CompareKey("Surname")
                .IsUnpredictable("Id");

            var builder = snapshots.NewSnapshot("Before");
            var rowBuilder = builder.AddNewRow("Customer");
            rowBuilder["Id"] = 1;
            rowBuilder["Surname"] = "surname";

            var snapshot = snapshots.GetSnapshot("Before");

            //Act 
            var row = snapshot.Rows("Customer").First();
            var result = snapshot.GetRow(new SnapshotRowKey(row, snapshots.GetTableDefinition("Customer")), "Wrong");

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public void GetSnapshotReturnsTheRequestedSnapshot()
        {
            //Arrange/Act
            var snapshots = new SnapshotCollection(GetType(), t => t != typeof(Snapshots.TestDefinitions.TableDef));
            snapshots.DefineTable("Customer")
                .PrimaryKey("Id")
                .CompareKey("Surname")
                .IsUnpredictable("Id");

            var builder = snapshots.NewSnapshot("Before");

            //Assert
            snapshots.GetSnapshot("Before").Name.Should().Be("Before");
        }

        [Test]
        public void ChangesCanBeReported()
        {
            //Arrange
            var snapshots = new SnapshotCollection();
            snapshots.DefineTable("Customer")
                .PrimaryKey("Id")
                .CompareKey("Surname")
                .IsUnpredictable("Id");
            snapshots.DefineTable("Address")
                    .PrimaryKey("AddressId")
                    .IsUnpredictable("AddressId")
                    .IsReference("CustomerId", "Customer", "Id");

            var customers = new[]
            {
                new { Id = 1, Surname = "S1", FirstName = "F1", Age = 40 },
                new { Id = 2, Surname = "S2", FirstName = "F2", Age = 45 },
                new { Id = 3, Surname = "S3", FirstName = "F3", Age = 50 },
            };

            var addresses = new[]
            {
                new { AddressId = 102, CustomerId = 1, House = 15, Street = "Line 1", PostCode = "code1" },
                new { AddressId = 193, CustomerId = 2, House = 99, Street = "Line 2", PostCode = "code2" },
                new { AddressId = 6985, CustomerId = 3, House = 8000, Street = "Line 3", PostCode = "code3" }
            };

            {
                var builder = snapshots.NewSnapshot("Before");
                customers.ToSnapshotTable(builder, "Customer");
                addresses.ToSnapshotTable(builder, "Address");
            }

            customers[1] = new { Id = 2, Surname = "S2", FirstName = "F2Edited", Age = 32 };
            addresses[2] = new { AddressId = 6985, CustomerId = 2, House = 8001, Street = "Line 3", PostCode = "code3" };

            {
                var builder2 = snapshots.NewSnapshot("After");
                customers.ToSnapshotTable(builder2, "Customer");
                addresses.ToSnapshotTable(builder2, "Address");
            }

            //Act/Assert
            var output = new Output();
            snapshots.ReportChanges("Before", "After", output);
            output.Report.Verify();
        }
    }
}
