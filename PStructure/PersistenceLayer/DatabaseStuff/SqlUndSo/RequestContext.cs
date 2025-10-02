using System;
using System.Collections.Generic;

namespace PStructure.PersistenceLayer.DatabaseStuff.SqlUndSo
{
    /// <summary>
    /// Carries SQL and parameter building logic for an execution.
    /// </summary>
    public class RequestContext
    {
        public string Sql { get; set; }

        /// <summary>
        /// Builds parameters for Dapper (can return an object or IEnumerable<object>).
        /// If null → SQL runs without parameters.
        /// </summary>
        public Func<IEnumerable<object>, object> ParameterFactory { get; set; }

        /// <summary>
        /// Flag indicating if this represents a batch execution.
        /// </summary>
        public bool IsBatch { get; set; } = false;
    }
}