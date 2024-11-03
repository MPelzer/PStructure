using System;
using PStructure.Models;

// Ensure the namespace is included for the attributes

namespace PStructure.Test.DBTestEnvironment;

public class TestEntry
{
    [PrimaryKey] // Marking this as the primary key
    [Column("GuidValue")]
    public Guid GuidValue { get; set; } // GUID (Globally Unique Identifier)
    
    [Column("IntegerValue")]
    public int IntegerValue { get; set; } // Integer type
    
    [Column("LongValue")]
    public long LongValue { get; set; } // Long type
    
    [Column("ShortValue")]
    public short ShortValue { get; set; } // Short type
    
    [Column("ByteValue")]
    public byte ByteValue { get; set; } // Byte type
    
    [Column("FloatValue")]
    public float FloatValue { get; set; } // Floating-point type
    
    [Column("DoubleValue")]
    public double DoubleValue { get; set; } // Double precision floating-point type
    
    [Column("DecimalValue")]
    public decimal DecimalValue { get; set; } // High precision decimal type
    
    [Column("BooleanValue")]
    public bool BooleanValue { get; set; } // Boolean type (true/false)
    
    [Column("CharValue")]
    public char CharValue { get; set; } // Character type
    
    [Column("StringValue")]
    public string StringValue { get; set; } // String type
    
    [Column("DateTimeValue")]
    public DateTime DateTimeValue { get; set; } // DateTime type for date and time values
    
    [Column("ByteArrayValue")]
    public byte[] ByteArrayValue { get; set; } // Array of bytes
}