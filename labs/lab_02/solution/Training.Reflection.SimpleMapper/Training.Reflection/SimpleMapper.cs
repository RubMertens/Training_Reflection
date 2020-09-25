using System;
using System.Collections.Generic;
using System.Xml;

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
            if(matchingPropertiesByKey.ContainsKey(key))
                throw new InvalidOperationException($"You tried to register a mapping from {fromType.FullName} to {toType.FullName} twice");
            
            var matchingProperties = fromType.GetMatchingProperties(toType);
            
            matchingPropertiesByKey.Add(key, matchingProperties);
        }

        private string GetKey(Type FromType, Type ToType)
        {
            return $"From_{FromType.FullName}_to_{ToType.FullName}";
        }

        public TTo MapTo<TTo>(object instance) where TTo : new()
        {
            var fromType = instance.GetType();
            var toType = typeof(TTo);
            var key = GetKey(fromType, toType);
            if(!matchingPropertiesByKey.ContainsKey(key))
                throw new ArgumentException($"No mapping found from {fromType.FullName} to {toType.FullName}");

            var matchingProperties = matchingPropertiesByKey[key];

            var toInstance = Activator.CreateInstance<TTo>();

            foreach (var match in matchingProperties)
            {
                var value =match.From.GetValue(instance);
                match.To.SetValue(toInstance, value);
            }
            return toInstance;
        }
    }
}