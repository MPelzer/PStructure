﻿using Dapper;

namespace PStructure.PersistenceLayer.Pdo.PdoInterfaces.CrudInterface
{
    /// <summary>
    ///     Interface, welches Funktionen definiert, um die PDO-Eigenschaften auf die Tabellenspalten abzubilden
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMapper<T>
    {
        void MapPropertiesToParameters(T item, DynamicParameters parameters);
        void MapPrimaryKeysToParameters(T item, DynamicParameters parameters);
    }
}