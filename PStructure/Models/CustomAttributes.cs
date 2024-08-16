using System;

namespace PStructure.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; }

        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TypeHandlerAttribute : Attribute
    {
        public Type HandlerType { get; }

        public TypeHandlerAttribute(Type handlerType)
        {
            HandlerType = handlerType;
        }
    }
}