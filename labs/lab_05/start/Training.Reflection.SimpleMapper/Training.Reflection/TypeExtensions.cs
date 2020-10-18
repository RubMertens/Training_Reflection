using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Training.Reflection
{
    public static class TypeExtensions
    {
        public static IList<MatchingProperties> GetMatchingProperties(
            this Type self, Type other)
        {
            var selfProperties = self
                .GetProperties(
                    BindingFlags.Public
                    | BindingFlags.Instance
                    | BindingFlags.NonPublic
                );

            var otherProperties = other
                .GetProperties(
                    BindingFlags.Public
                    | BindingFlags.Instance
                    | BindingFlags.NonPublic
                );


            var matchingProperties =
                selfProperties
                    .SelectMany(s => otherProperties
                        .Where(o =>
                            s.Name == o.Name
                            && s.PropertyType == o.PropertyType
                            && s.CanRead == o.CanRead
                            && s.CanWrite == o.CanWrite)
                        .Select(o =>
                            new MatchingProperties()
                            {
                                From = s,
                                To = o
                            }));

            return matchingProperties.ToList();
        }

        public static IList<MatchingProperties> GetMatchingPropertiesByAttribute(
            this Type self, Type other
        )
        {
            var selfProperties = self.GetProperties(
                BindingFlags.Public | BindingFlags.Instance |
                BindingFlags.NonPublic
            );
            var otherProperties = other.GetProperties(
                BindingFlags.Public | BindingFlags.Instance |
                BindingFlags.NonPublic
            );

            var matchingProperties = selfProperties
                .SelectMany(s =>
                    otherProperties.Where(
                            o => o
                                     .GetCustomAttribute<MappedFromAttribute
                                     >()?
                                     .Name == s.Name
                                 && o.CanRead && o.CanWrite
                                 && s.CanRead && s.CanWrite
                                 && o.PropertyType == s.PropertyType
                                 )
                        .Select(o => new MatchingProperties()
                        {
                            From = s, To = o
                        })
                );

            return matchingProperties.ToList();
        }
    }
}