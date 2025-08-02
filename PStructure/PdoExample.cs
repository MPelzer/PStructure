using System;
using PStructure.PersistenceLayer.Pdo.PdoCruds.PropertyConverters;
using PStructure.PersistenceLayer.Pdo.PdoData;
using PStructure.PersistenceLayer.Pdo.PdoData.Attributes;

namespace PStructure
{
    public class PdoExample
    {
        [PdoPropertyAttributes.PrimaryKey]
        [PdoPropertyAttributes.Column("ID")]
        public int Id { get; set; }

        [PdoPropertyAttributes.Column("IS_ACTIVE")]
        public bool IsActive { get; set; }

        [PdoPropertyAttributes.Column("STATUS")]
        public char Status { get; set; }

        [PdoPropertyAttributes.Column("AGE")]
        public byte Age { get; set; }

        [PdoPropertyAttributes.Column("SMALL_NUMBER")]
        public short SmallNumber { get; set; }

        [PdoPropertyAttributes.Column("LARGE_NUMBER")]
        public int LargeNumber { get; set; }

        [PdoPropertyAttributes.Column("VERY_LARGE_NUMBER")]
        public long VeryLargeNumber { get; set; }

        [PdoPropertyAttributes.Column("HEIGHT")]
        public float Height { get; set; }

        [PdoPropertyAttributes.Column("WEIGHT")]
        public double Weight { get; set; }

        [PdoPropertyAttributes.Column("PRICE")]
        public decimal Price { get; set; }

        [PdoPropertyAttributes.Column("NAME")]
        public string Name { get; set; }

        [PdoPropertyAttributes.Column("DATE_COMPACT")]
        [PdoPropertyAttributes.TypeHandler(typeof(CustomDateHandler_yyyyMMdd))]
        public DateTime DateInCompactFormat { get; set; }

        [PdoPropertyAttributes.Column("DATE_WITH_TIME")]
        [PdoPropertyAttributes.TypeHandler(typeof(CustomDateHandler_yyyy_MM_ddTHH_mm_ss))]
        public DateTime DateWithTimeFormat { get; set; }

        [PdoPropertyAttributes.Column("DATE_EU")]
        [PdoPropertyAttributes.TypeHandler(typeof(CustomDateHandler_dd_MM_yyyy))]
        public DateTime DateInEuropeanFormat { get; set; }

        [PdoPropertyAttributes.Column("DATE_US")]
        [PdoPropertyAttributes.TypeHandler(typeof(CustomDateHandler_MM_dd_yyyy))]
        public DateTime DateInAmericanFormat { get; set; }

        [PdoPropertyAttributes.Column("DATE_CENTURY")]
        [PdoPropertyAttributes.TypeHandler(typeof(CustomDateHandler_cyyMMdd))]
        public DateTime DateWithCenturyFormat { get; set; }

        public static PdoExample CreateSample()
        {
            return new PdoExample
            {
                Id = 1,
                IsActive = true,
                Status = 'A',
                Age = 30,
                SmallNumber = 123,
                LargeNumber = 100000,
                VeryLargeNumber = 9999999999,
                Height = 1.75f,
                Weight = 70.5,
                Price = 19.99m,
                Name = "Test Name",
                DateInCompactFormat = new DateTime(2024, 12, 25),
                DateWithTimeFormat = new DateTime(2024, 12, 25, 15, 30, 0),
                DateInEuropeanFormat = new DateTime(2024, 1, 1),
                DateInAmericanFormat = new DateTime(2024, 7, 4),
                DateWithCenturyFormat = new DateTime(2025, 6, 15)
            };
        }
    }
}
