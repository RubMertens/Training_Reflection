using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Training.Reflection
{
    public class SimpleMapper
    {
        private Dictionary<string, IList<MatchingProperties>>
            matchingPropertiesByKey =
                new Dictionary<string, IList<MatchingProperties>>();

        public void Register<TFrom, TTo>() where TTo : new()
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            var key = GetKey(fromType, toType);
            if (matchingPropertiesByKey.ContainsKey(key))
                throw new InvalidOperationException(
                    $"You tried to register a mapping from {fromType.FullName} to {toType.FullName} twice");

            var matchingPropertiesByName = fromType.GetMatchingProperties(toType);
            var matchingPropertiesByAttribute =
                fromType.GetMatchingPropertiesByAttribute(toType);
            var allMatchingProperties = matchingPropertiesByName
                .Union(matchingPropertiesByAttribute).ToList();

            matchingPropertiesByKey.Add(key, allMatchingProperties);
        }

        private static string GetKey(Type FromType, Type ToType)
        {
            return $"From_{FromType.FullName}_to_{ToType.FullName}";
        }

        public TTo MapTo<TTo>(object instance) where TTo : new()
        {
            var fromType = instance.GetType();
            var toType = typeof(TTo);
            var key = GetKey(fromType, toType);
            if (!matchingPropertiesByKey.ContainsKey(key))
                throw new ArgumentException(
                    $"No mapping found from {fromType.FullName} to {toType.FullName}");
            
            var toInstance = Activator.CreateInstance<TTo>();
            foreach (var match in matchingPropertiesByKey[key])
            {
                var value =match.From.GetValue(instance);
                match.To.SetValue(toInstance, value);
            }
            return toInstance;
        }
    }
}