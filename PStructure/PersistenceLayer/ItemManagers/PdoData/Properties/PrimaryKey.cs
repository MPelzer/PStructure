using System;

namespace PStructure.Models
{
    /// <summary>
    /// Indicates that the PDO property is part of the primary key for the table representation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PrimaryKey : Attribute
    {
    }
}