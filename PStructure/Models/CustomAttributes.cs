using System;

namespace PStructure.Models
{
    /// <summary>
    /// Attribut, um den Namen der Tabellenspalte zu hinterlegen.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; }

        public ColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
    /// <summary>
    /// Attribut, um eine Transformation von oder zu PDO-Eigenschaften und Datenbank-Tabellenwerten durchzuführen.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TypeHandlerAttribute : Attribute
    {
        public Type HandlerType { get; }

        public TypeHandlerAttribute(Type handlerType)
        {
            HandlerType = handlerType;
        }
    }
    /// <summary>
    /// Signalisiert, dass die PDO_Eigenschaft Teil des Primärschlüssels der Tabellenrepräsentation ist.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PrimaryKeyAttribute : Attribute
    {
    }
    
}