using System;
using PStructure.DapperSqlDateTimeMappers;
using PStructure.Interfaces.DapperSqlDateTimeMappers;
using PStructure.Models;

public class Customer
{
    [PrimaryKey]
    [Column("Name")]
    public string Name { get; set; }
    
    [PrimaryKey] 
    [Column("Age")]
    public int Age { get; set; }

    [Column("IsActive")]
    public bool IsActive { get; set; }

    [Column("Date_yyyyMMdd"), TypeHandlerAttribute(typeof(CustomDateHandler_yyyyMMdd))]
    public DateTime DateIn_yyyyMMdd { get; set; }

    [Column("Date_yyyy_MM_dd"), TypeHandlerAttribute(typeof(CustomDateHandler_yyyy_MM_dd))]
    public DateTime DateIn_yyyy_MM_dd { get; set; }

    [Column("Date_yyyy_MM_ddTHH_mm_ss"), TypeHandlerAttribute(typeof(CustomDateHandler_yyyy_MM_ddTHH_mm_ss))]
    public DateTime DateIn_yyyy_MM_ddTHH_mm_ss { get; set; }

    [Column("Date_MM_dd_yyyy"), TypeHandlerAttribute(typeof(CustomDateHandler_MM_dd_yyyy))]
    public DateTime DateIn_MM_dd_yyyy { get; set; }

    [Column("Date_dd_MM_yyyy"), TypeHandlerAttribute(typeof(CustomDateHandler_dd_MM_yyyy))]
    public DateTime DateIn_dd_MM_yyyy { get; set; }

    [Column("Date_cyyMMdd"), TypeHandlerAttribute(typeof(CustomDateHandler_cyyMMdd))]
    public DateTime DateIn_cyyMMdd { get; set; }

    [Column("Salary")]
    public decimal Salary { get; set; }

    [Column("Latitude")]
    public double Latitude { get; set; }

    [Column("LastLogin")]
    public DateTime? LastLogin { get; set; }
    
    
}