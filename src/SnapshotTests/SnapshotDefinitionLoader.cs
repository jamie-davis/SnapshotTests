using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SnapshotTests.Snapshots;

namespace SnapshotTests
{
    public static class SnapshotDefinitionLoader
    {
        public static DefinitionSet Load(Assembly assembly, Func<Type, bool> typeFilter = null)
        {
            var types = assembly.GetTypes()
                .Where(t => !t.IsNested && CustomAttributeExtensions.GetCustomAttribute<SnapshotDefinitionAttribute>((MemberInfo) t) != null)
                .Where(t => typeFilter == null || typeFilter(t))
                .ToList();
            return LoadDefinitions(types);
        }

        public static DefinitionSet Load(Type containerType, Func<Type, bool> typeFilter = null)
        {
            var types = containerType.GetNestedTypes()
                .Where(t => t.GetCustomAttribute<SnapshotDefinitionAttribute>() != null)
                .Where(t => typeFilter == null || typeFilter(t))
                .ToList();
            return LoadDefinitions(types);
        }

        private static DefinitionSet LoadDefinitions(List<Type> types)
        {
            var attributed = types.Select(t => new
                    {Type = t, Attribute = t.GetCustomAttribute<SnapshotDefinitionAttribute>()})
                .Where(t => t.Attribute != null)
                .Select(t => ExtractDefinition(t.Type, t.Attribute.SnapshotTableName))
                .Where(d => d != null);
            return new DefinitionSet(attributed);
        }

        private static TableDefinition ExtractDefinition(Type type, string snapshotTableName)
        {
            if (snapshotTableName == null)
                return null;

            var definition = new TableDefinition(snapshotTableName, new Type[] {type});

            var propertyAttributes = type.GetProperties()
                .Select(p => new
                {
                    Prop = p,
                    Unpredictable = p.GetCustomAttribute<UnpredictableAttribute>(), 
                    Predictable = p.GetCustomAttribute<PredictableAttribute>(),
                    Required = p.GetCustomAttribute<RequiredAttribute>(),
                    Key = p.GetCustomAttribute<KeyAttribute>(),
                    References = p.GetCustomAttribute<ReferencesAttribute>(),
                    SortAsc = p.GetCustomAttribute<SortFieldAttribute>(),
                    SortDesc = p.GetCustomAttribute<DescendingSortFieldAttribute>(),
                    UtcDateTime = p.GetCustomAttribute<UTCAttribute>(),
                    LocalDateTime = p.GetCustomAttribute<LocalAttribute>(),
                    ExcludeProperty = p.GetCustomAttribute<ExcludeFromComparisonAttribute>(),
                })
                .ToList();

            foreach (var attributes in propertyAttributes)
            {
                if (attributes.Key != null) 
                    definition.SetPrimaryKey(attributes.Prop.Name);

                if (attributes.Unpredictable != null)
                    definition.SetUnpredictable(attributes.Prop.Name);

                if (attributes.References != null)
                    definition.SetReference(attributes.Prop.Name, attributes.References.SnapshotTableName, attributes.References.KeyPropertyName);

                if (attributes.Required != null)
                    definition.SetRequired(attributes.Prop.Name);

                if (attributes.Predictable != null)
                    definition.SetPredictable(attributes.Prop.Name);

                if (attributes.SortAsc != null)
                    definition.SetSortColumn(attributes.Prop.Name, SortOrder.Ascending, attributes.SortAsc.SortSequencer);

                if (attributes.SortDesc != null)
                    definition.SetSortColumn(attributes.Prop.Name, SortOrder.Descending, attributes.SortDesc.SortSequencer);

                if (attributes.UtcDateTime != null)
                    definition.SetDateType(attributes.Prop.Name, DateTimeKind.Utc);

                if (attributes.LocalDateTime != null)
                    definition.SetDateType(attributes.Prop.Name, DateTimeKind.Local);

                if (attributes.ExcludeProperty != null)
                    definition.ExcludeColumnFromComparison(attributes.Prop.Name);
            }

            if (type.GetCustomAttribute<ExcludeFromComparisonAttribute>() != null)
                definition.ExcludeFromComparison = true;

            return definition;
        }
    }
}
