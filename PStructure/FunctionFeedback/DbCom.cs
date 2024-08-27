using System;
using System.Data;
using System.Transactions;
using Optional;

namespace PStructure.Interfaces
{
    /// <summary>
    /// Kapselt dieverse Informationen, die für die Datenbanktransaktion benötigt werden.
    /// </summary>
    public class DbCom : IFunctionFeedback
    {
        /// <summary>
        /// Datenbankverbindung. 
        /// </summary>
        public IDbConnection  _dbConnection;

        /// <summary>
        /// Optionale Transaktion.
        /// </summary>
        public Option<IDbTransaction> _transaction;

        /// <summary>
        /// Ein optionaler SQL als String, der entweder vin der Crud automatisch, oder vom Nutzer manuell gefüllt werden kann.
        /// </summary>
        public Option<string> injectedSql;
        
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