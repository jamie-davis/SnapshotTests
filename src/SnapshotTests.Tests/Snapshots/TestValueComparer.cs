using System;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Snapshots;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestValueComparer
    {
        #region Types for test

        class TestTypeBase
        {
            protected int Value { get; private set; }

            public TestTypeBase(int value)
            {
                Value = value;
            }

            #region Overrides of Object

            /// <summary>
            /// Return a comparable string
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Value.ToString("00000");
            }

            #endregion
        }


        class TypeNotComparable : TestTypeBase
        {
            public TypeNotComparable(int value) : base(value)
            {
            }
        }

        class TypeComparable : TestTypeBase, IComparable<TypeComparable>
        {
            public TypeComparable(int value) : base(value)
            {
            }

            #region Implementation of IComparable<in TypeComparable>

            public int CompareTo(TypeComparable other)
            {
                CompareToCalled = true;
                return Value.CompareTo(other.Value);
            }

            public bool CompareToCalled { get; set; }

            #endregion
        }

        #endregion

        private static object[] _equalValues =
        {
            //Primitives implement IComparable so we do not need to test all of them

            //Int
            1,
            0,
            (int?) null,

            //double
            1.5,
            (double?) null,

            //DateTime
            DateTime.MaxValue,
            (DateTime?)null,

            //String
            "XX",
            (string)null,

            //IComparable type
            new TypeComparable(5),

            //Non-IComparableType
            new TypeNotComparable(5),
        };

        [Test]
        [TestCaseSource(nameof(_equalValues))]
        public void EqualValuesCompareEqual(object value)
        {
            //Act
            var result = ValueComparer.Compare(value, value);

            //Assert
            result.Should().Be(0);
        }

        [Test]
        public void CompareToCalledForIComparableType()
        {
            //Arrange
            var comparable1 = new TypeComparable(1);
            var comparable2 = new TypeComparable(2);

            //Act
            ValueComparer.Compare(comparable1, comparable2);

            //Assert
            comparable1.CompareToCalled.Should().BeTrue();
        }

        private static object[] _unequalValues =
        {
            //Primitives implement IComparable so we do not need to test all of them

            //Int
            new object[] {5, 1},
            new object[] {-5, (int?) null},

            //double
            new object[] {2.5, 1.5},
            new object[] {-5.5, (double?) null},

            //DateTime
            new object[] {DateTime.MaxValue, DateTime.Parse("2020-04-12 09:50")},
            new object[] {DateTime.MinValue, (DateTime?)null},

            //String
            new object[] {"XX","XW"},
            new object[] {string.Empty, (string)null},

            //IComparable type
            new object[] {new TypeComparable(5),new TypeComparable(4)},

            //Non-IComparableType
            new object[] {new TypeNotComparable(5),new TypeNotComparable(4)},


            //Experiment
            new object[] { 1, 0.5 }
        };

        [Test]
        [TestCaseSource(nameof(_unequalValues))]
        public void GreaterValuesCompareGreater(object greater, object lesser)
        {
            //Act
            var result = ValueComparer.Compare(greater, lesser);

            //Assert
            result.Should().Be(1);
        }

        [Test]
        [TestCaseSource(nameof(_unequalValues))]
        public void LesserValuesCompareLesser(object greater, object lesser)
        {
            //Act
            var result = ValueComparer.Compare(lesser, greater);

            //Assert
            result.Should().Be(-1);
        }

        [Test]
        public void TypesThatCannotBeCoercedCompareDifferent()
        {
            //Act
            var result = ValueComparer.Compare(DateTime.Parse("2020-07-25 16:13"), 1.5);

            //Assert
            result.Should().Be(1);
        }

        [Test]
        public void TypesThatCannotBeCoercedCompareStringSame()
        {
            //Act
            var result = ValueComparer.Compare(1.5, "1.5");

            //Assert
            result.Should().Be(0);
        }

    }
}