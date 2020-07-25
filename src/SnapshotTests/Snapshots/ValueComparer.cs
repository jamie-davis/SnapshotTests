using System;
using System.Diagnostics;
using System.Reflection;

namespace SnapshotTests.Snapshots
{
    internal static class ValueComparer
    {
        private static readonly MethodInfo UseComparerMethod;

        static ValueComparer()
        {
            UseComparerMethod = typeof(ValueComparer).GetMethod(nameof(UseComparer), BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(UseComparerMethod != null);
        }

        internal static int? Compare(object mine, object other)
        {
            if (mine == null)
                return other == null ? 0 : -1;

            if (other == null)
            {
                return 1;
            }

            var myType = mine.GetType();
            var otherType = other.GetType();

            var method = UseComparerMethod.MakeGenericMethod(otherType);
            var (isComparable, result) = ((bool, int))method.Invoke(null, new[] {mine, other});
            if (isComparable)
                return result;

            if (myType != otherType)
            {
                if (TypeCoercer.TryCoerce(mine, other, out var mineCoerced, out var otherCoerced))
                    return Compare(mineCoerced, otherCoerced);
            }

            return string.Compare(mine.ToString(), other.ToString(), StringComparison.Ordinal);
        }

        private static (bool IsComparable, int CompareResult) UseComparer<T>(object mine, T other)
        {
            if (mine is IComparable<T> comparable)
            {
                return (true, comparable.CompareTo(other));
            }

            return (false, 0);
        }
    }
}