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
            public int Counter { get; set; }
        }
    }

    public class TypeExtensionTests
    {
        [Test]
        public void GetMatchingProperties_ReturnsMatchingNames()
        {
            var typeOne = typeof(TestClasses.MatchingNamesFrom);
            var typeTwo = typeof(TestClasses.MatchingNamesTo);

            var matchingProperties =
                typeOne.GetMatchingProperties(typeTwo);

            Assert.That(matchingProperties, Has.Count.EqualTo(1));
            Assert.That(matchingProperties[0].From.Name, Is.EqualTo("Count"));
            Assert.That(matchingProperties[0].To.Name, Is.EqualTo("Count"));

            Assert.That(matchingProperties[0].From.PropertyType,
                Is.EqualTo(typeof(int)));
            Assert.That(matchingProperties[0].To.PropertyType,
                Is.EqualTo(typeof(int)));
        }

        [Test]
        public void GetMatchingProperties_ReturnsOnlyMatchingNamesAndTypes()
        {
            var typeOne = typeof(TestClasses.MatchingTypesOne);
            var typeTwo = typeof(TestClasses.MatchingTypesTwo);

            var matchingProperties =
                typeOne.GetMatchingProperties(typeTwo);

            Assert.That(matchingProperties, Has.Count.EqualTo(1));
            Assert.That(matchingProperties[0].From.Name, Is.EqualTo("Count"));
            Assert.That(matchingProperties[0].To.Name, Is.EqualTo("Count"));

            Assert.That(matchingProperties[0].From.PropertyType,
                Is.EqualTo(typeof(int)));
            Assert.That(matchingProperties[0].To.PropertyType,
                Is.EqualTo(typeof(int)));
        }

        [Test]
        public void GetMatchingProperties_AlsoReturnsMatchingPrivateProperties()
        {
            var typeOne = typeof(TestClasses.PrivatesOne);
            var typeTwo = typeof(TestClasses.PrivatesTwo);

            var matchingProperties =
                typeOne.GetMatchingProperties(typeTwo);

            Assert.That(matchingProperties, Has.Count.EqualTo(1));
            Assert.That(matchingProperties[0].From.Name, Is.EqualTo("Count"));
            Assert.That(matchingProperties[0].To.Name, Is.EqualTo("Count"));

            Assert.That(matchingProperties[0].From.PropertyType,
                Is.EqualTo(typeof(int)));
            Assert.That(matchingProperties[0].To.PropertyType,
                Is.EqualTo(typeof(int)));
        }

        [Test]
        public void
            GetMatchingProperties_OnlyReturnsPropertiesWithGettersAndSetters()
        {
            var typeOne = typeof(TestClasses.CanReadOne);
            var typeTwo = typeof(TestClasses.CanReadTwo);

            var matchingProperties =
                typeOne.GetMatchingProperties(typeTwo);
            Assert.That(matchingProperties, Is.Empty);
        }
    }
}