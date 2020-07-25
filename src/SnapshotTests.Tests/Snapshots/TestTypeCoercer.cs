using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestTypeCoercer
    {
        private object[] _testValues = new object[]
        {
            (sbyte)125,
            (byte)125,
            (short)125,
            (ushort)125,
            (int)125,
            (uint)125,
            (long)125,
            (ulong)125,
            (float)125,
            (double)125,
            (decimal)125,
        };

        private static bool DoCompare<T>(T left, T right) where T : IComparable<T>
        {
            return left.CompareTo(right) == 0;
        }

        [Test]
        public void DetectValidCoercions()
        {
            //Arrange
            var allCombinations = _testValues.Select(v => new { Value = v, Pairs = _testValues.Select(rhs => new {Value = v, Other = rhs})});

            //Act
            //Here we are looking to detect what coercion is allowed - so we should count any coercion that changes the left hand type into the right hand type
            //as a disallowed conversion.
            var result = allCombinations.Select(tt => new
            {
                Left = tt.Value,
                Valid = tt.Pairs
                    .Where(p => TypeCoercer.TryCoerceLeftOnly(p.Value, p.Other, out var newMine, out _) && newMine.GetType() == p.Other.GetType()) //i.e. where coercion works
                    .Select(p => p.Other.GetType())
            }); //i.e. all successful conversions where the left hand type was not changed.


            //Assert
            var output = new Output();
            var formatted = result.Select(r => new {Source = r.Left.GetType(), CanCoerceTo = string.Join(", ", r.Valid.Select(t => t.Name))});
            output.FormatTable(formatted);
            output.Report.Verify();
        }

        [Test]
        public void DetectInvalidCoercions()
        {
            //Arrange
            var allCombinations = _testValues.Select(v => new { Value = v, Pairs = _testValues.Select(rhs => new {Value = v, Other = rhs})});

            //Act
            //Here we are looking to detect what coercion is allowed - so we should count any coercion that changes the left hand type into the right hand type
            //as a disallowed conversion.
            var result = allCombinations.Select(tt => new
            {
                Left = tt.Value,
                Valid = tt.Pairs
                    .Where(p => !TypeCoercer.TryCoerceLeftOnly(p.Value, p.Other, out var newMine, out _) || newMine.GetType() != p.Other.GetType())
                    .Select(p => p.Other.GetType())
            });  //i.e. all failed conversions or conversions that coerced the left hand side to the right.

            //Assert
            var output = new Output();
            var formatted = result.Select(r => new {Source = r.Left.GetType(), CannotCoerceTo = string.Join(", ", r.Valid.Select(t => t.Name))});
            output.FormatTable(formatted);
            output.Report.Verify();
        }

        
        [Test]
        public void CoercionsProduceTheSameValue()
        {
            //Arrange
            bool InvalidPairing(object value, object other)
            {
                return (value is float && other is decimal)
                    || (value is decimal && other is float)
                    || (value is decimal && other is double)
                    || (value is double && other is decimal);
            }
            
            var allAcceptableCombinations = _testValues
                .Select(v => new {Value = v, Pairs = _testValues.Select(rhs => new {Value = v, Other = rhs}).Where(p => !InvalidPairing(p.Value, p.Other))});

            //Act
            var result = allAcceptableCombinations.SelectMany(tt => tt.Pairs
                    .Select(p =>
                    {
                        if (TypeCoercer.TryCoerce(p.Value, p.Other, out var left, out var right))
                        {
                            return new {Left = left, Right = right};
                        }

                        return new { Left = (object)null, Right = (object)null };
                    }));

            //Assert
            bool Compare(object left, object right)
            {
                var leftType = left.GetType();
                var rightType = right.GetType();
                if (leftType != rightType)
                    return false;

                var genericCompareMethod = typeof(TestTypeCoercer).GetMethod(nameof(DoCompare), BindingFlags.NonPublic | BindingFlags.Static);
                var compareMethod = genericCompareMethod.MakeGenericMethod(leftType);
                var compareResult = (bool)compareMethod.Invoke(null, new[] {left, right});
                return compareResult;
            }

            result.Select(r => Compare(r.Left, r.Right)).Should().AllBeEquivalentTo(true);
        }

        [Test]
        public void NullLeftInputResultsInNullLeftOutput()
        {
            //Act
            TypeCoercer.TryCoerce(null, 1, out var left, out var right);

            //Assert
            left.Should().BeNull();
        }

        [Test]
        public void NullRightInputResultsInNullLeftOutput()
        {
            //Act
            TypeCoercer.TryCoerce(null, 1, out var left, out var right);

            //Assert
            left.Should().BeNull();
        }

        [Test]
        public void NullLeftInputResultsInNullRightOutput()
        {
            //Act
            TypeCoercer.TryCoerce(null, 1, out var left, out var right);

            //Assert
            right.Should().BeNull();
        }

        [Test]
        public void NullRightInputResultsInNullRightOutput()
        {
            //Act
            TypeCoercer.TryCoerce(null, 1, out var left, out var right);

            //Assert
            right.Should().BeNull();
        }

        [Test]
        public void ImpossibleCoercionResultsInNullLeftOutput()
        {
            //Act
            TypeCoercer.TryCoerce(DateTime.Now, 1.5, out var left, out var right);

            //Assert
            left.Should().BeNull();
        }

        [Test]
        public void ImpossibleCoercionResultsInNullRightOutput()
        {
            //Act
            TypeCoercer.TryCoerce(DateTime.Now, 1.5, out var left, out var right);

            //Assert
            right.Should().BeNull();
        }
    }

}


