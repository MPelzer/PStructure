using NUnit.Framework;
using MySqlConnector;
using System;
using System.Data;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Optional;
using Optional.Unsafe;
using PStructure.CRUDs;
using PStructure.FunctionFeedback;
using PStructure.Mapper;
using PStructure.root;
using PStructure.SqlGenerator;
using PStructure.TableLocation;
using NullLogger = Castle.Core.Logging.NullLogger;

namespace PStructure.Test
{
    [TestFixture]
    public class DefaultItemManagerTest
    {
        private const string ConnectionString = "Server=localhost;Port=3306;Database=testdb;User=testuser;Password=testpassword;";
        private MySqlConnection _dbConnection;

        [SetUp]
        public void SetUp()
        {
            try
            {
                _dbConnection = new MySqlConnection(ConnectionString);
                _dbConnection.Open();
                InitializeTable();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to initialize connection or table: {ex.Message}");
            }
        }

        private void InitializeTable()
        {
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Customer (
                        Name VARCHAR(255) NOT NULL,
                        Age INT NOT NULL,
                        IsActive BOOLEAN NOT NULL,
                        Date_yyyyMMdd VARCHAR(100),
                        Date_yyyy_MM_dd VARCHAR(100),
                        Date_yyyy_MM_ddTHH_mm_ss VARCHAR(100),
                        Date_MM_dd_yyyy VARCHAR(100),
                        Date_dd_MM_yyyy VARCHAR(100),
                        Date_cyyMMdd VARCHAR(100),
                        Salary DECIMAL(18,2),
                        Latitude DECIMAL(9,6),
                        LastLogin DATETIME,
                        PRIMARY KEY (Name, Age)
                    )";
                command.ExecuteNonQuery();
            }
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Product (
                        Name VARCHAR(255) NOT NULL,
                        Price INT NOT NULL,
                        IsActive DECIMAL(18,2) NOT NULL,
                        ExpiringDay VARCHAR(100),
                        PRIMARY KEY (Name, Price)
                    )";
                command.ExecuteNonQuery();
            }
        }

        [Test]
        public void InsertByInstance_Should_InsertDataCorrectly()
        {
            var customer = new Customer
            {
                Name = "Herbert",
                Age = 5,
                IsActive = true,
                DateIn_cyyMMdd = DateTime.Now,
                DateIn_dd_MM_yyyy = DateTime.Now,
                DateIn_MM_dd_yyyy = DateTime.Today,
                DateIn_yyyy_MM_dd = DateTime.Now,
                DateIn_yyyyMMdd = DateTime.Now,
                DateIn_yyyy_MM_ddTHH_mm_ss = DateTime.Now,
                Salary = 3,
                Latitude = 234.342,
                LastLogin = DateTime.Now
            };
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug); // Set the minimum log level
            });

            // Create an ILogger instance
            ILogger<Customer> logger = loggerFactory.CreateLogger<Customer>();
            var mapper = new MapperPdoQuery<Customer>();
            var sqlGenerator = new BaseSqlGenerator<Customer>();
            var tableLocation = new BaseTableLocation("","Customer");
            var itemFactory = new Option<IItemFactory<Customer>>();
            var extendedCrud = new ExtendedCrud<Customer>(sqlGenerator,mapper,tableLocation,logger);
            var itemManager = new DefaultItemManager<Customer>(extendedCrud, itemFactory);
            
            var dbCom = new DbCom
            {
                requestAnswer = false,
                _dbConnection = _dbConnection,
                _transaction = null,
                injectedSql = " ",
                requestException = null
            };


            itemManager.InsertByInstance(customer, ref dbCom);


            var tableValue = new Customer();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Customer";
    
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tableValue = new Customer
                        {
                            Name = reader["Name"].ToString(),
                            Age = Convert.ToInt32(reader["Age"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            DateIn_yyyyMMdd = reader["Date_yyyyMMdd"] as DateTime? ?? default,
                            DateIn_yyyy_MM_dd = reader["Date_yyyy_MM_dd"] as DateTime? ?? default,
                            DateIn_yyyy_MM_ddTHH_mm_ss = reader["Date_yyyy_MM_ddTHH_mm_ss"] as DateTime? ?? default,
                            DateIn_MM_dd_yyyy = reader["Date_MM_dd_yyyy"] as DateTime? ?? default,
                            DateIn_dd_MM_yyyy = reader["Date_dd_MM_yyyy"] as DateTime? ?? default,
                            DateIn_cyyMMdd = reader["Date_cyyMMdd"] as DateTime? ?? default,
                            Salary = reader["Salary"] as decimal? ?? 0,
                            Latitude = (double)(reader["Latitude"] as decimal? ?? 0),
                            LastLogin = reader["LastLogin"] as DateTime?,
                        };
                        
                    }
                }
            }
            Assert.That(tableValue.Name, Is.EqualTo("Herbert"));
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                using (var command = _dbConnection.CreateCommand())
                {
                    command.CommandText = "DROP TABLE IF EXISTS Product";
                    command.ExecuteNonQuery();
                }
                using (var command = _dbConnection.CreateCommand())
                {
                    command.CommandText = "DROP TABLE IF EXISTS Customer";
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to drop table: {ex.Message}");
            }
            finally
            {
                _dbConnection?.Close();
                _dbConnection?.Dispose();
            }
        }
    }
}
