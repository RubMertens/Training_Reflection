using System;
using System.Collections.Generic;
using System.Xml;

namespace Training.Reflection
{
    public class SimpleMapper
    {
        public void Register<TFrom, TTo>() where TTo : new()
        {
        }
        public TTo MapTo<TTo>(object instance) where TTo : new()
        {
           return new TTo();
        }
    }
}