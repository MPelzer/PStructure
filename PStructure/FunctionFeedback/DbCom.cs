using System;
using System.Data;
using Optional;
using PStructure.Interfaces;

namespace PStructure.FunctionFeedback
{
    /// <summary>
    /// Kapselt diverse Informationen, welche einerseits vom Konsumenten an die Datenbank-Abstraktionsebene gegeben wird,
    /// so wie die Antwort, die man von ihr erhält.
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
        /// Antwort darüber, ob der gewünschte Prozess erfolgreich verlief.
        /// </summary>
        public bool requestAnswer;
        
        /// <summary>
        /// Optionale Exception.
        /// </summary>
        public Option<Exception> requestException;

    }
}