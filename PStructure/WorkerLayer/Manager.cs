using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer;

namespace PStructure.WorkerLayer
{
    public class Manager : ClassCore, IDisposable
    {
        internal ILogger _logger;

        internal Dictionary<Type, IPersistenceLayer> _persistenceLayers;

        internal IDbTransaction _dbTransaction;

        public Manager(ILogger logger, IDbTransaction dbTransaction, List<IPersistenceLayer> persistenceLayers)
        {
            _logger = logger;
            _dbTransaction = dbTransaction;
            InitializePersistenceLayers(persistenceLayers);
        }

        private void InitializePersistenceLayers(List<IPersistenceLayer> persistenceLayers)
        {
            _logger.LogDebug($"{GetLoggingClassName()}Registrierung der Speicherschichten des Managers");
            foreach (IPersistenceLayer layer in persistenceLayers)
            {
                if (_persistenceLayers.ContainsKey(layer.GetType()))
                {
                    throw new ManagerException($"{GetLoggingClassName()}Doppelter Schlüsselwert. Ein Objekt vom Typ {layer.GetType()} existiert schon");
                }
                _logger.LogDebug($"{GetLoggingClassName()}Speicherschichten vom Typ {layer.GetType()} registriert");
                _persistenceLayers.Add(layer.GetType(),layer);
            }
            _logger.LogDebug($"{GetLoggingClassName()}Registrierung beendet");
        }
        
        #region Exceptions
         public class ManagerException : Exception
        {
            public ManagerException()
            {
            }

            public ManagerException(string message) : base(message)
            {
            }

            public ManagerException(string message, Exception inner) : base(message, inner)
            {
            }
        }
        #endregion

        public void Dispose()
        {
            _dbTransaction?.Dispose();
        }
    }
}