using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SnapshotTests.Snapshots;

namespace SnapshotTests.Tests.Snapshots.TestSupportUtils
{
    internal class SnapshotObjectMother
    {
        #region Types that define tables

        [SnapshotDefinition("Table")]
        public static class TableDef
        {
            [Key]
            [Unpredictable]
            public static int Id { get; set; }
        }

        [SnapshotDefinition("Table2")]
        public static class TableDef2
        {
            [Key] 
            [Unpredictable]
            public static int Id { get; set; }
        }

        [SnapshotDefinition("ReferencingTable")]
        public static class RelatedTableDef
        {
            [Key] 
            [Unpredictable]
            public static int Id { get; set; }
            
            [References("Table", nameof(TableDef.Id))]
            public static int? ParentId { get; set; }
        }

        [SnapshotDefinition("ReferencingTable2")]
        public static class RelatedTableDef2
        {
            [Key] 
            [Unpredictable]
            public static int Id { get; set; }
            
            [References("Table2", nameof(TableDef2.Id))]
            public static int? ParentId { get; set; }
        }

        [SnapshotDefinition("GuidKeyTable")]
        public static class GuidKeyTableDef
        {
            [Key] 
            [Unpredictable]
            public static Guid GuidKeyId { get; set; }
        }

        [SnapshotDefinition("GuidRefTable")]
        public static class GuidRefTableDef
        {
            [Key] 
            [Unpredictable]
            public static Guid GuidKeyId { get; set; }
            
            [References("GuidKeyTable", nameof(GuidKeyTable.GuidKeyId))]
            public static Guid GuidKeyParentId { get; set; }
            
            [References("GuidKeyTable", nameof(GuidKeyTable.GuidKeyId))]
            public static Guid GuidKeyParentId2 { get; set; }
        }

        [SnapshotDefinition("Unpredictables")]
        public static class UnpredictablesTableDef
        {
            [Key] 
            [Unpredictable]
            public static Guid GuidKeyId { get; set; }
            
            [Unpredictable]
            public static int Int { get; set; }
            
            [Unpredictable]
            public static int? NullableInt { get; set; }
            
            [Unpredictable]
            public static DateTime TimeStamp { get; set; }
        }

        #endregion

        #region Tables to snapshot

        public class Table
        {
            public int Id { get; set; }
            public string Variable { get; set; }
            public string OtherVariable { get; set; }
        }

        public class Table2
        {
            public int Id { get; set; }
            public string Variable { get; set; }
            public string OtherVariable { get; set; }
        }

        public class ReferencingTable
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public int OtherId { get; set; }
            public string Variable { get; set; }
            public string OtherVariable { get; set; }
        }

        public class ReferencingTable2
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string Variable { get; set; }
            public string OtherVariable { get; set; }
        }

        public class GuidKeyTable
        {
            public Guid GuidKeyId { get; set; }
            public string Variable { get; set; }
            public string OtherVariable { get; set; }
        }

        public class GuidRefTable
        {
            public Guid GuidKeyId { get; set; }
            public Guid? GuidKeyParentId { get; set; }
            public string Variable { get; set; }
            public string OtherVariable { get; set; }
        }

        public class Unpredictables
        {
            public Guid GuidKeyId { get; set; }
            public int Int { get; set; }
            public int? NullableInt { get; set; }
            public DateTime TimeStamp { get; set; }
        }
        #endregion

        #region Test data generation

        internal List<T> Make<T>(params int[] keys)
        {
            return keys.Select(k => MakeInstance<T>(k)).ToList();
        }

        internal List<T> Make<T>(params Guid[] keys)
        {
            return keys.Select(k => MakeInstance<T>(k)).ToList();
        }

        internal List<T> Make<T>(params (int Id, int? ParentId)[] keys)
        {
            return keys.Select(k => MakeInstance<T>(k.Id, k.ParentId)).ToList();
        }

        internal List<T> Make<T>(params (Guid Guid, Guid? ParentId)[] keys)
        {
            return keys.Select(k => MakeInstance<T>(k.Guid, k.ParentId)).ToList();
        }

        private T MakeInstance<T>(int id, int? parentId = null)
        {
            var instance = (T) Activator.CreateInstance<T>();
            var idProp = typeof(T).GetProperty("Id");
            Debug.Assert(idProp != null, nameof(idProp) + " != null");
            idProp.SetValue(instance, id);

            var varProp = typeof(T).GetProperty("Variable");
            if (varProp != null)
                varProp.SetValue(instance, $"Default variable for Id {id}");

            if (parentId != null)
            {
                var parentProp = typeof(T).GetProperty("ParentId");
                Debug.Assert(parentProp != null, nameof(parentProp) + " != null");
                parentProp.SetValue(instance, parentId);
            }
            return instance;
        }

        private T MakeInstance<T>(Guid id, Guid? parentId = null)
        {
            var instance = (T) Activator.CreateInstance<T>();
            var idProp = typeof(T).GetProperty("GuidKeyId");
            if (idProp == null) throw new Exception($"GuidKeyId not found on table {typeof(T).Name}");
            idProp.SetValue(instance, id);

            var varProp = typeof(T).GetProperty("Variable");
            if (varProp != null)
                varProp.SetValue(instance, $"Default variable for Id {id}");

            if (parentId != null)
            {
                var parentProp = typeof(T).GetProperty("GuidKeyParentId");
                if (parentProp == null) throw new Exception($"GuidKeyParentId not found on table {typeof(T).Name}");
                parentProp.SetValue(instance, parentId);
            }
            return instance;
        }

        #endregion
        
        public SnapshotCollection Collection { get; }

        public SnapshotObjectMother()
        {
            Collection = new SnapshotCollection(typeof(SnapshotObjectMother));
        }

        internal SnapshotBuilder NewSnapshot(string name)
        {
            return Collection.NewSnapshot(name);
        }

        public Snapshot GetSnapshot(string name)
        {
            return Collection.GetSnapshot(name);
        }
    }
}