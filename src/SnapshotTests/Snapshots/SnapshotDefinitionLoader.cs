using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SnapshotTests.Snapshots
{
    internal static class SnapshotDefinitionLoader
    {
        internal static DefinitionSet Load(Assembly assembly, Func<Type, bool> typeFilter = null)
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

            var definition = new TableDefinition(snapshotTableName);

            var propertyAttributes = type.GetProperties()
                .Select(p => new
                {
                    Prop = p,
                    Unpredictable = p.GetCustomAttribute<UnpredictableAttribute>(), 
                    Predictable = p.GetCustomAttribute<PredictableAttribute>(),
                    Required = p.GetCustomAttribute<RequiredAttribute>(),
                    Key = p.GetCustomAttribute<KeyAttribute>(),
                    References = p.GetCustomAttribute<ReferencesAttribute>(),
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
            }

            return definition;
        }
    }
}
