using System;

namespace PStructure.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; }

        public ColumnAttribute(string columnName)
        {
            ColumnName = columnName;
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
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PrimaryKeyAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ToStringForTestAttribute : Attribute
    {
    }
}