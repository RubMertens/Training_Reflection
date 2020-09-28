using System;
using NUnit.Framework;

namespace Training.Reflection.Tests
{
    public class SimpleMapperTestClasses
    {
        public class One
        {
            public int Count { get; set; }
        }

        public class Two
        {
            public int Count { get; set; }
        }

        public class PrivateOne
        {
            private int Count { get; set; } = 123;
        }

        public class PrivateTwo
        {
            private int Count { get; set; }

            public int GetPrivateCount()
            {
                return Count;
            }
        }

        public class MappedFrom
        {
            public string FirstName { get; set; }
        }

        public class MappedTo
        {
            [MappedFrom("FirstName")] public string GivenName { get; set; }
        }

        public class MappedToWithConflictingNames
        {
            public string FirstName { get; set; }
            [MappedFrom("FirstName")] public string GivenName { get; set; }
        }
    }

    public class SimpleMapperTests
    {
        [Test]
        public void WithMatchingProperties_CopiesValuesCorrectly()
        {
            var mapper = new SimpleMapper();

            mapper.Register<
                SimpleMapperTestClasses.One,
                SimpleMapperTestClasses.Two
            >();

            var instanceOne = new SimpleMapperTestClasses.One()
            {
                Count = 123
            };

            var instanceTwo =
                mapper.MapTo<SimpleMapperTestClasses.Two>(instanceOne);

            Assert.That(instanceTwo.Count, Is.EqualTo(123));
        }

        [Test]
        [Ignore("Can't use private with Roslyn")]
        public void WithMatchingPrivateProperties_CopiesValuesCorrectly()
        {
            var mapper = new SimpleMapper();
            mapper
                .Register<SimpleMapperTestClasses.PrivateOne,
                    SimpleMapperTestClasses.PrivateTwo>();

            var instance = new SimpleMapperTestClasses.PrivateOne();

            var instanceTwo =
                mapper.MapTo<SimpleMapperTestClasses.PrivateTwo>(instance);
            Assert.That(instanceTwo.GetPrivateCount(), Is.EqualTo(123));
        }

        [Test]
        [Ignore("Can't use private with Roslyn")]
        public void WithTypesWithSameShortName_CanRegisterTwice()
        {
            var mapper = new SimpleMapper();

            mapper
                .Register<SimpleMapperTestClasses.PrivateOne,
                    SimpleMapperTestClasses.PrivateTwo>();
            mapper.Register<TestClasses.PrivateOne, TestClasses.PrivateTwo>();

            var simpleMapperPrivateTwo =
                mapper.MapTo<SimpleMapperTestClasses.PrivateTwo>(
                    new SimpleMapperTestClasses.PrivateOne());
            var testClassesPrivateTwo =
                mapper.MapTo<TestClasses.PrivateTwo>(
                    new TestClasses.PrivateOne());

            Assert.That(simpleMapperPrivateTwo,
                Is.TypeOf<SimpleMapperTestClasses.PrivateTwo>());
            Assert.That(testClassesPrivateTwo,
                Is.TypeOf<TestClasses.PrivateTwo>());
        }

        [Test]
        public void WhenNotRegistered_ThrowsException()
        {
            var mapper = new SimpleMapper();
            var instance = new SimpleMapperTestClasses.One()
            {
                Count = 1325
            };
            Assert.Throws<ArgumentException>(() =>
                mapper.MapTo<SimpleMapperTestClasses.Two>(instance));
        }

        [Test]
        public void WhenDoubleMapping_ThrowsInvalidOperationException()
        {
            var mapper = new SimpleMapper();

            mapper
                .Register<SimpleMapperTestClasses.One, SimpleMapperTestClasses.Two
                >();

            Assert.Throws<InvalidOperationException>(() =>
                mapper
                    .Register<SimpleMapperTestClasses.One,
                        SimpleMapperTestClasses.Two>());
        }


        [Test]
        public void WithMappedFromAttribute_MapsValuesCorrectly()
        {
            var mapper = new SimpleMapper();
            mapper
                .Register<SimpleMapperTestClasses.MappedFrom,
                    SimpleMapperTestClasses.MappedTo>();

            var instance = new SimpleMapperTestClasses.MappedFrom()
            {
                FirstName = "My First Name"
            };

            var instanceTwo =
                mapper.MapTo<SimpleMapperTestClasses.MappedTo>(instance);

            Assert.That(instanceTwo.GivenName, Is.EqualTo("My First Name"));
        }

        [Test]
        public void
            WithMappedFromAttribute_WithConflictingPropertiesNames_CopiesAll()
        {
            var mapper = new SimpleMapper();
            mapper
                .Register<SimpleMapperTestClasses.MappedFrom,
                    SimpleMapperTestClasses.MappedToWithConflictingNames>();

            var instance = new SimpleMapperTestClasses.MappedFrom()
            {
                FirstName = "My First Name"
            };

            var instanceTwo =
                mapper
                    .MapTo<SimpleMapperTestClasses.MappedToWithConflictingNames>(
                        instance);

            Assert.That(instanceTwo.GivenName, Is.EqualTo("My First Name"));
            Assert.That(instanceTwo.FirstName, Is.EqualTo("My First Name"));
        }
    }
}