using System;
using PStructure.Interfaces.DapperSqlDateTimeMappers;
using PStructure.Models;

namespace PStructure.Test;

public class Product
{
    [PrimaryKey]
    [Column("Name")]
    public string Name { get; set; }
    
    [PrimaryKey] 
    [Column("Age")]
    public int Price { get; set; }

    [Column("IsActive")]
    public bool IsActive { get; set; }

    [Column("Date_yyyyMMdd"), TypeHandler(typeof(CustomDateHandler_yyyyMMdd))]
    public DateTime ExpiringDay { get; set; }

    
}