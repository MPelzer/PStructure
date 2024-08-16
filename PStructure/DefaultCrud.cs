using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Transactions;
using Dapper;
using Optional;
using PStructure.FunctionFeedback;
using PStructure.Interfaces;
using PStructure.Models;

namespace PStructure
{
    public class DefaultCrud<T> : ICrud<T>
    {
        public T InsertByInstance(T item, ref DbCom dbCom)
        {

            try
            {
                DbComHandler.OpenConnectionIfNotAlready(dbCom);
                DbComHandler.StartTransactionIfNotAlready(dbCom);

                string insertSql =
                    "INSERT INTO TableName (Column1, Column2, Date_Column_1, Text_Column) VALUES (@Column1, @Column2, @Date_Column_1, @Text_Column)";

                var parameters = new DynamicParameters();
                MapPdoToTable(item, parameters);

                dbCom._dbConnection.Execute(insertSql, parameters,
                    dbCom._transaction.HasValue ? dbCom._transaction.Value : null);

                DbComHandler.SetAnswerValid(dbCom);
            }
            catch (Exception ex)
            {
                DbComHandler.SetException(dbCom, ex);
                DbComHandler.RollbackTransaction(dbCom);
            }
            finally
            {
                DbComHandler.CommitIfAnswerValid(dbCom);
                DbComHandler.CloseConnectionIfNotAlready(dbCom);
            }

            return item;
        }

        public T DeleteByPrimaryKey(ICompoundPrimaryKey compoundPrimaryKey, ref DbCom dbCom)
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<T> InsertRangeByInstances(IEnumerable<T> items, ref DbCom dbCom)
        {

            var insertedItems = new List<T>();
            try
            {
                DbComHandler.OpenConnectionIfNotAlready(dbCom);
                DbComHandler.StartTransactionIfNotAlready(dbCom);

                foreach (var item in items)
                {
                    // Custom logic and parameter mapping here
                    InsertByInstance(item, out dbCom); // Reusing the single insert logic
                    if (dbCom.requestAnswer)
                    {
                        insertedItems.Add(item);
                    }
                    else
                    {
                        throw dbCom.requestException.ValueOr(new Exception("Unknown error occurred during insertion."));
                    }
                }

                DbComHandler.SetAnswerValid(dbCom);
            }
            catch (Exception ex)
            {
                DbComHandler.SetException(dbCom, ex);
                DbComHandler.RollbackTransaction(dbCom);
            }
            finally
            {
                DbComHandler.CommitIfAnswerValid(dbCom);
                DbComHandler.CloseConnectionIfNotAlready(dbCom);
            }

            return insertedItems;
        }

        public T UpdateByInstance(T item, ref DbCom dbCom)
        {
            try
            {
                DbComHandler.OpenConnectionIfNotAlready(dbCom);
                DbComHandler.StartTransactionIfNotAlready(dbCom);

                string updateSql
                    = "UPDATE TableName SET Column1 = @Column1, Column2 = @Column2, Date_Column_1 = @Date_Column_1 WHERE Id = @Id";

                var parameters = new DynamicParameters();
                MapPdoToTable(item, parameters);

                // Ensure Id is mapped correctly
                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty != null)
                {
                    parameters.Add("@Id", idProperty.GetValue(item));
                }

                dbCom._dbConnection.Execute(updateSql, parameters, dbCom._transaction.HasValue ? dbCom._transaction.Value : null);

                DbComHandler.SetAnswerValid(dbCom);
            }
            catch (Exception ex)
            {
                DbComHandler.SetException(dbCom, ex);
                DbComHandler.RollbackTransaction(dbCom);
            }
            finally
            {
                DbComHandler.CommitIfAnswerValid(dbCom);
                DbComHandler.CloseConnectionIfNotAlready(dbCom);
            }

            return item;
            }

        public IEnumerable<T> UpdateRangeByInstances(IEnumerable<T> items, ref DbCom dbCom){
            var updatedItems = new List<T>();
            try
            {
                DbComHandler.OpenConnectionIfNotAlready(dbCom);
                DbComHandler.StartTransactionIfNotAlready(dbCom);

                foreach (var item in items)
                {
                    UpdateByInstance(item, out dbCom);
                    if (dbCom.requestAnswer)
                    {
                        updatedItems.Add(item);
                    }
                    else
                    {
                        throw dbCom.requestException.ValueOr(new Exception("Unknown error occurred during update."));
                    }
                }

                DbComHandler.SetAnswerValid(dbCom);
            }
            catch (Exception ex)
            {
                DbComHandler.SetException(dbCom, ex);
                DbComHandler.RollbackTransaction(dbCom);
            }
            finally
            {
                DbComHandler.CommitIfAnswerValid(dbCom);
                DbComHandler.CloseConnectionIfNotAlready(dbCom);
            }

            return updatedItems;
       }
        public IEnumerable<T> DeleteRangeByPrimaryKeys(IEnumerable<ICompoundPrimaryKey> keys, ref DbCom dbCom)
        {
            
            var deletedItems = new List<T>();
            try
            {
                DbComHandler.OpenConnectionIfNotAlready(dbCom);
                DbComHandler.StartTransactionIfNotAlready(dbCom);

                foreach (var key in keys)
                {
                    var deletedItem = DeleteByPrimaryKey(key, ref dbCom);
                    if (dbCom.requestAnswer)
                    {
                        deletedItems.Add(deletedItem);
                    }
                    else
                    {
                        throw dbCom.requestException.ValueOr(new Exception("Unknown error occurred during deletion."));
                    }
                }

                DbComHandler.SetAnswerValid(dbCom);
            }
            catch (Exception ex)
            {
                DbComHandler.SetException(dbCom, ex);
                DbComHandler.RollbackTransaction(dbCom);
            }
            finally
            {
                DbComHandler.CommitIfAnswerValid(dbCom);
                DbComHandler.CloseConnectionIfNotAlready(dbCom);
            }

            return deletedItems;
        }

        public IEnumerable<T> ReadAll(ref DbCom dbCom)
        {
            IEnumerable<T> items = null;
            try
            {
                DbComHandler.OpenConnectionIfNotAlready(dbCom);

                string selectSql = "SELECT * FROM TableName";

                items = dbCom._dbConnection.Query<T>(selectSql);

                DbComHandler.SetAnswerValid(dbCom);
            }
            catch (Exception ex)
            {
                DbComHandler.SetException(dbCom, ex);
            }
            finally
            {
                DbComHandler.CloseConnectionIfNotAlready(dbCom);
            }

            return items;
        }

        public IEnumerable<T> ReadRangeByPrimaryKey(ICompoundPrimaryKey compoundPrimaryKey, ref DbCom dbCom)
        {
            
            IEnumerable<T> items = null;
            try
            {
                DbComHandler.OpenConnectionIfNotAlready(dbCom);

                string selectSql = "SELECT * FROM TableName WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", compoundPrimaryKey.ToStringForTest());

                items = dbCom._dbConnection.Query<T>(selectSql, parameters);

                DbComHandler.SetAnswerValid(dbCom);
            }
            catch (Exception ex)
            {
                DbComHandler.SetException(dbCom, ex);
            }
            finally
            {
                DbComHandler.CloseConnectionIfNotAlready(dbCom);
            }

            return items;
        }

        public IEnumerable<T> ReadRangeByPrimaryKeys(IEnumerable<ICompoundPrimaryKey> compoundPrimaryKeys, ref DbCom dbCom)
        {
            throw new NotImplementedException();
        }

        private DateTime ParseDate(string dateValue)
        {
            DateTime parsedDate;

            // Attempt standard parsing first
            if (DateTime.TryParse(dateValue, out parsedDate))
            {
                return parsedDate;
            }

            // Custom parsing logic if standard parsing fails
            if (DateTime.TryParseExact(dateValue, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None,
                    out parsedDate))
            {
                return parsedDate;
            }

            // Add more custom date formats if needed, or throw an exception
            throw new FormatException($"The date format of '{dateValue}' is not supported.");
        }

        /// <summary>
        /// Präpariert SQL Parameter zum Übersetzen von PDO´s in Tabellenspalten
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parameters"></param>
        public void MapPdoToTable(T item, DynamicParameters parameters)
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var value = prop.GetValue(item);
                string columnName = prop.Name;

                // Get the column attribute, if it exists
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                if (columnAttr != null)
                {
                    columnName = columnAttr.Name;
                }

                // Get the type handler attribute, if it exists
                var handlerAttr = prop.GetCustomAttribute<TypeHandlerAttribute>();
                if (handlerAttr != null)
                {
                    var handler = (ICustomHandler)Activator.CreateInstance(handlerAttr.HandlerType);
                    value = handler.Format(value);
                }

                parameters.Add("@" + columnName, value);
            }
        }
        /// <summary>
        /// Verknüpft beim Laden der Datensätze die Tabellenspalten mit den Eigenschaften des PDO´s.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="reader"></param>
        private void MapTableColumnsToPdo(T entity, IDataReader reader)
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var columnName = prop.Name;
        
                // Get the column attribute, if it exists
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                if (columnAttr != null)
                {
                    columnName = columnAttr.Name;
                }

                var value = reader[columnName];

                if (value != DBNull.Value)
                {
                    var handlerAttr = prop.GetCustomAttribute<TypeHandlerAttribute>();
                    if (handlerAttr != null)
                    {
                        var handler = (ICustomHandler)Activator.CreateInstance(handlerAttr.HandlerType);
                        value = handler.Parse(value);
                    }

                    prop.SetValue(entity, value);
                }
            }
        }
    }
}
