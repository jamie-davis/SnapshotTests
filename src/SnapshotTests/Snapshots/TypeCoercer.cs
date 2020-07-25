using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SnapshotTests.Snapshots
{
    internal static class TypeCoercer
    {
        private static readonly Dictionary<object, Type[]> Coercions = MakeCoercions().ToDictionary(x => x.Source, x => x.ValidCoercions);

        private static IEnumerable<(object Source, Type[] ValidCoercions)> MakeCoercions()
        {
            yield return DefineCoercion(typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal));
            yield return DefineCoercion(typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal));
            yield return DefineCoercion(typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal));
            yield return DefineCoercion(typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal));
            yield return DefineCoercion(typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal));
            yield return DefineCoercion(typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal));
            yield return DefineCoercion(typeof(long), typeof(float), typeof(double), typeof(decimal));
            yield return DefineCoercion(typeof(ulong), typeof(float), typeof(double), typeof(decimal));
            yield return DefineCoercion(typeof(float), typeof(double));
        }

        private static (object, Type[]) DefineCoercion(Type source, params Type[] validCoercions)
        {
            return (source, validCoercions);
        }

        internal static bool TryCoerceLeftOnly(object mine, object other, out object mineCoerced, out object otherCoerced)
        {
            return TryCoerceLeftIntoRight(mine, other, out mineCoerced, out otherCoerced);
        }

        public static bool TryCoerce(object mine, object other, out object mineCoerced, out object otherCoerced)
        {
            if (mine == null || other == null)
            {
                mineCoerced = null;
                otherCoerced = null;
                return false;
            }

            if (mine.GetType() == other.GetType())
            {
                mineCoerced = mine;
                otherCoerced = other;
                return true;
            }

            if (TryCoerceLeftIntoRight(mine, other, out mineCoerced, out otherCoerced)
             || TryCoerceLeftIntoRight(other, mine, out otherCoerced, out mineCoerced))
                return true;

            if (TryCoerceBoth(mine, other, out mineCoerced, out otherCoerced))
                return true;

            return false;
        }

        private static bool TryCoerceLeftIntoRight(object mine, object other, out object mineCoerced, out object otherCoerced)
        {
            if (mine == null 
                || other == null
                || !Coercions.TryGetValue(mine.GetType(), out var validList) 
                || !validList.Contains(other.GetType())
                || !Coercer.TryCoerce(mine, other.GetType(), out var coercedValue))
            {
                mineCoerced = null;
                otherCoerced = null;
                return false;
            }

            mineCoerced = coercedValue;
            otherCoerced = other;
            return true;
        }

        private static bool TryCoerceBoth(object mine, object other, out object mineCoerced, out object otherCoerced)
        {
            //this will not be called with null in either value
            Type common;
            if (GetCommonType(mine, other, out common)
                || !Coercer.TryCoerce(mine, common, out mineCoerced)
                || !Coercer.TryCoerce(other, common, out otherCoerced))
            {
                mineCoerced = null;
                otherCoerced = null;
                return false;
            }

            return true;
        }

        private static bool GetCommonType(object mine, object other, out Type common)
        {
            if (!Coercions.TryGetValue(mine.GetType(), out var mineValid)
                || !Coercions.TryGetValue(other.GetType(), out var otherValid))
            {
                common = null;
                return false;
            }
            return (common = mineValid.Intersect(otherValid).FirstOrDefault()) == null;
        }
    }

    internal static class Coercer
    {
        private static readonly List<MethodInfo> ConversionFunctions = typeof(Convert).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.Name.StartsWith("To")
                        && f.GetParameters().Length == 1)
            .ToList();

        private static MethodInfo FindFunction(Type from, Type to)
        {
            return ConversionFunctions
                .FirstOrDefault(f => f.GetParameters()[0].ParameterType == from
                                     && f.ReturnType == to);
        }

        internal static bool TryCoerce(object value, Type requiredType, out object coercedValue)
        {
            var func = FindFunction(value?.GetType(), requiredType);
            if (func == null)
            {
                coercedValue = null;
                return false;
            }

            coercedValue = func.Invoke(null, new []{ value });
            return true;
        }
    }
}