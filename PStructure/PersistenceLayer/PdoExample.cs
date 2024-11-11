using System;
using PStructure.DapperSqlDateTimeMappers;
using PStructure.Interfaces.DapperSqlDateTimeMappers;
using PStructure.PersistenceLayer;

// For the custom date handlers

// For ICustomHandler

namespace PStructure.Models
{
    [TableLocation(WorkMode.Test, Database.TestDb, "TestSchema", "TestTable")]
    [TableLocation(WorkMode.Live, Database.LiveDb, "LiveSchema", "LiveTable")]
    public class PdoExample
    {
        // Primary key, simple integer
        public int Id { get; set; }

        // Basic types
        public bool IsActive { get; set; } // Boolean type
        public char Status { get; set; } // Char type
        public byte Age { get; set; } // Byte type
        public short SmallNumber { get; set; } // Short type
        public int LargeNumber { get; set; } // Integer type
        public long VeryLargeNumber { get; set; } // Long type
        public float Height { get; set; } // Float type
        public double Weight { get; set; } // Double type
        public decimal Price { get; set; } // Decimal type
        public string Name { get; set; } // String type

        // DateTime properties with custom handlers

        // Using CustomDateHandler_yyyyMMdd
        [TypeHandler(typeof(CustomDateHandler_yyyyMMdd))]
        public DateTime DateInCompactFormat { get; set; }

        // Using CustomDateHandler_yyyy_MM_ddTHH_mm_ss
        [TypeHandler(typeof(CustomDateHandler_yyyy_MM_ddTHH_mm_ss))]
        public DateTime DateWithTimeFormat { get; set; }

        // Using CustomDateHandler_dd_MM_yyyy
        [TypeHandler(typeof(CustomDateHandler_dd_MM_yyyy))]
        public DateTime DateInEuropeanFormat { get; set; }

        // Using CustomDateHandler_MM_dd_yyyy
        [TypeHandler(typeof(CustomDateHandler_MM_dd_yyyy))]
        public DateTime DateInAmericanFormat { get; set; }

        // Using CustomDateHandler_cyyMMdd for Century-Representation Date Format
        [TypeHandler(typeof(CustomDateHandler_cyyMMdd))]
        public DateTime DateWithCenturyFormat { get; set; }
    }
}