using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Training.Reflection
{
    public static class ObjectExtensions
    {
        public static IList<MatchingProperties> GetMatchingProperties(
            this object self, object other)
        {
            var selfProperties = self.GetType()
                .GetProperties(
                    BindingFlags.Public
                    | BindingFlags.Instance
                    | BindingFlags.NonPublic
                );

            var otherProperties = other.GetType()
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
    }
}