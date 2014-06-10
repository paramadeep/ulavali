using System;

namespace Ulavali
{
    public class ObjectProperty
    {
        public ObjectProperty(String propertyName, String propertyValue)
        {
            Property = propertyName;
            Value = propertyValue;
        }

        public string Property { get; set; }

        public string Value { get; set; }
    }
}