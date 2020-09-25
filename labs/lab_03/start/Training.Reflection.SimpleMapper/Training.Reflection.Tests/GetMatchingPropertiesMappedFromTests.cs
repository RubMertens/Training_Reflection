using System;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace Training.Reflection.Tests
{
    public class MatchingPropertiesByAttributeTests
    {
        class TestClasses
        {
            public class FromType
            {
                public string FirstName { get; set; }
            }

            public class ToType
            {
                [MappedFrom("FirstName")]
                public string GivenName { get; set; }
            }
            
            public class ToTypeWithPrivateProp
            {
                [MappedFrom("FirstName")]
                private string GivenName { get; set; }

                public string GetGiveName()
                {
                    return GivenName;
                }
            }
            
            public class ToTypeWithUnMatchingType
            {
                [MappedFrom("FirstName")]
                public int Count { get; set; }
            }
            
            public class ToTypeWithWrongPropName
            {
                [MappedFrom("SomeNonExistantProp")]
                public string GiveName { get; set; }
            }
            
            public class ToTypeWithUnSettableProp
            {
                [MappedFrom("FirstName")]
                public string GivenName { get; }
            }
        }

        [Test]
        public void WithMappedFromAttribute_ReturnsMatchingProperties()
        {
            var matchingProperties = typeof(TestClasses.FromType).GetMatchingPropertiesByAttribute(
                typeof(TestClasses.ToType));
            
            Assert.That(matchingProperties, Has.Count.EqualTo(1));
            
            Assert.That(matchingProperties[0].From.Name, Is.EqualTo("FirstName"));
            Assert.That(matchingProperties[0].To.Name, Is.EqualTo("GivenName"));
        }

        [Test]
        public void WithMappedFromAttribute_ReturnsMatchingProperties_WhenPrivate()
        {
            var matchingProperties = typeof(TestClasses.FromType).GetMatchingPropertiesByAttribute(
                typeof(TestClasses.ToTypeWithPrivateProp));
            
            Assert.That(matchingProperties, Has.Count.EqualTo(1));
            
            Assert.That(matchingProperties[0].From.Name, Is.EqualTo("FirstName"));
            Assert.That(matchingProperties[0].To.Name, Is.EqualTo("GivenName"));
        }

        [Test]
        public void WithMappedFromAttribute_IgnoredWhenTypesDoesNotMatch()
        {
            var matchingProperties = typeof(TestClasses.FromType).GetMatchingPropertiesByAttribute(
                typeof(TestClasses.ToTypeWithUnMatchingType));
            
            Assert.That(matchingProperties, Is.Empty);
        }

        [Test]
        public void WithMappedAttribute_IgnoredWhenFromPropertyIsNotFound()
        {
            var matchingProperties = typeof(TestClasses.FromType).GetMatchingPropertiesByAttribute(
                typeof(TestClasses.ToTypeWithWrongPropName));
            
            Assert.That(matchingProperties, Is.Empty);
        }

        [Test]
        public void WithMappedAttribute_IgnoredWhenPropertyIsNotSettable()
        {
            var matchingProperties = typeof(TestClasses.FromType).GetMatchingPropertiesByAttribute(
                typeof(TestClasses.ToTypeWithUnSettableProp));
            
            Assert.That(matchingProperties, Is.Empty);
        }
        
    }
}