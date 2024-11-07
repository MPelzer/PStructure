using System;

namespace PStructure.Models
{
    /// <summary>
    /// Specifies a custom type handler for transformation between PDO properties and database table values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TypeHandler : Attribute
    {
        public Type HandlerType { get; }

        public TypeHandler(Type handlerType)
        {
            HandlerType = handlerType;
        }
    }
}