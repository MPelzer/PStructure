using System;
using System.Data;
using System.Transactions;
using Optional;

namespace PStructure.Interfaces
{
    /// <summary>
    /// Kapselt dieverse Informationen, die für die Datenbanktransaktion benötigt werden.
    /// </summary>
    public class DbCom
    {
        /// <summary>
        /// Datenbankverbindung. 
        /// </summary>
        public IDbConnection  _dbConnection;

        /// <summary>
        /// Optionale Transaktion.
        /// </summary>
        public Option<Transaction> _transaction;

        /// <summary>
        /// Antwort darüber, ob der gewünschte Proess erfolgreich verlief.
        /// </summary>
        public bool requestAnswer;
        
        /// <summary>
        /// Optionale Exception.
        /// </summary>
        public Option<Exception> requestException;

    }
}