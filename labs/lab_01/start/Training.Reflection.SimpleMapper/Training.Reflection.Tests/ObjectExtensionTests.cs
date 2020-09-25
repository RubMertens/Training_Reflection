using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using NUnit.Framework;

namespace Training.Reflection.Tests
{
    public class TestClasses
    {
        public class MatchingNamesFrom
        {
            public int Count { get; set; }    
        }

        public class MatchingNamesTo
        {
            public int Count { get; set; }
        }
        
        public class MatchingTypesOne
        {
            public int Count { get; set; }
            public string Letter { get; set; }
        }
        public class MatchingTypesTwo    
        {
            public int Count { get; set; }
            public char Letter { get; set; }
        }
        
        public class PrivatesOne
        {
            private int Count { get; set; }
        }
        public class PrivatesTwo
        {
            public int Count { get; set; }
        }
        
        public class CanReadOne
        {
            public int Counter { get; }
        }
        public class CanReadTwo
        {
            public int Counter { get;set; }
        }
    }
    public class ObjectExtensionTests
    {
        [Test]
        public void GetMatchingProperties_ReturnsMatchingNames()
        {
            var instanceOne = new TestClasses.MatchingNamesFrom();
            var instanceTwo = new TestClasses.MatchingNamesTo();

            var matchingProperties =
                instanceOne.GetMatchingProperties(instanceTwo);
            
            Assert.That(matchingProperties, Has.Count.EqualTo(1));
            Assert.That(matchingProperties[0].From.Name, Is.EqualTo("Count"));
            Assert.That(matchingProperties[0].To.Name, Is.EqualTo("Count"));
            
            Assert.That(matchingProperties[0].From.PropertyType, Is.EqualTo(typeof(int)));
            Assert.That(matchingProperties[0].To.PropertyType, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void GetMatchingProperties_ReturnsOnlyMatchingNamesAndTypes()
        {
            var instanceOne = new TestClasses.MatchingTypesOne();
            var instanceTwo = new TestClasses.MatchingTypesTwo();

            var matchingProperties =
                instanceOne.GetMatchingProperties(instanceTwo);
            
            Assert.That(matchingProperties, Has.Count.EqualTo(1));
            Assert.That(matchingProperties[0].From.Name, Is.EqualTo("Count"));
            Assert.That(matchingProperties[0].To.Name, Is.EqualTo("Count"));
            
            Assert.That(matchingProperties[0].From.PropertyType, Is.EqualTo(typeof(int)));
            Assert.That(matchingProperties[0].To.PropertyType, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void GetMatchingProperties_AlsoReturnsMatchingPrivateProperties()
        {
            var instanceOne = new TestClasses.PrivatesOne();
            var instanceTwo = new TestClasses.PrivatesTwo();

            var matchingProperties =
                instanceOne.GetMatchingProperties(instanceTwo);
            
            Assert.That(matchingProperties, Has.Count.EqualTo(1));
            Assert.That(matchingProperties[0].From.Name, Is.EqualTo("Count"));
            Assert.That(matchingProperties[0].To.Name, Is.EqualTo("Count"));
            
            Assert.That(matchingProperties[0].From.PropertyType, Is.EqualTo(typeof(int)));
            Assert.That(matchingProperties[0].To.PropertyType, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void GetMatchingProperties_OnlyReturnsPropertiesWithGettersAndSetters()
        {
            var instanceOne = new TestClasses.CanReadOne();
            var instanceTwo = new TestClasses.CanReadTwo();

            var matchingProperties =
                instanceOne.GetMatchingProperties(instanceTwo);
            Assert .That(matchingProperties,Is.Empty);

        }
    }
}