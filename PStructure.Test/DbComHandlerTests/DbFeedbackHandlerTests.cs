using System;
using System.Data;
using System.Data.Common;
using MySqlConnector;
using NUnit.Framework;
using PStructure.FunctionFeedback;

namespace PStructure.Test.DbComHandlerTests
{
    [TestFixture]
    public class DbFeedbackHandlerTests
    {
        private const string ConnectionString = "Server=localhost;Port=3306;Database=testdb;User=testuser;Password=testpassword;";
        private MySqlConnection _dbConnection;

        [SetUp]
        public void SetUp()
        {
            _dbConnection = new MySqlConnection(ConnectionString);
            _dbConnection.Open();
        }

        [TearDown]
        public void TearDown()
        {
            if (_dbConnection.State == ConnectionState.Open)
            {
                _dbConnection.Close();
            }
            _dbConnection.Dispose();
        }

        [Test]
        public void ExecuteWithTransaction_Should_CreateAndCommitTransaction_WhenNoTransactionProvided()
        {
            // Arrange
            var dbFeedback = new DbFeedback(_dbConnection);
            DbAction action = (ref DbFeedback dbFeedbackInstance) => { /* Perform DB operations */ };
            DbCondition commitCondition = (ref DbFeedback dbFeedbackInstance) => true;

            // Act
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: action,
                onException: null,
                commitCondition: commitCondition
            );

            // Assert
            Assert.That(dbFeedback.DbTransaction, Is.Null, "Transaction should be null after commit.");
            Assert.That(dbFeedback.DbConnection.State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after commit.");
        }

        [Test]
        public void ExecuteWithTransaction_Should_NotCommitTransaction_WhenCommitConditionIsFalse()
        {
            // Arrange
            var dbFeedback = new DbFeedback(_dbConnection);
            DbAction action = (ref DbFeedback dbFeedbackInstance) => { /* Perform DB operations */ };
            DbCondition commitCondition = (ref DbFeedback dbFeedbackInstance) => false;

            // Act
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: action,
                onException: null,
                commitCondition: commitCondition
            );

            // Assert
            Assert.That(dbFeedback.DbTransaction, Is.Null, "Transaction should be null after rollback or no commit.");
            Assert.That(dbFeedback.DbConnection.State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after execution.");
        }

        [Test]
        public void ExecuteWithTransaction_Should_HandleExternalTransaction_WithoutCommittingOrRollingBack()
        {
            // Arrange
            var dbFeedback = new DbFeedback(_dbConnection);
            var transaction = _dbConnection.BeginTransaction();
            

            DbAction action = (ref DbFeedback dbFeedbackInstance) => { /* Perform DB operations */ };
            DbCondition commitCondition = (ref DbFeedback dbFeedbackInstance) => true;

            // Act
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: action,
                onException: null,
                commitCondition: commitCondition
            );

            // Assert
            Assert.That(dbFeedback.DbTransaction, Is.EqualTo(transaction), "External transaction should not be altered.");
            Assert.That(dbFeedback.DbConnection.State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after execution.");
        }

        [Test]
        public void ExecuteWithTransaction_Should_HandleExceptionAndRollbackTransaction()
        {
            // Arrange
            var dbFeedback = new DbFeedback(_dbConnection);
            DbAction action = (ref DbFeedback dbFeedbackInstance) => throw new InvalidOperationException("Test exception");
            DbCondition commitCondition = (ref DbFeedback dbFeedbackInstance) => true;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                DbFeedbackHandler.ExecuteWithTransaction(
                    ref dbFeedback,
                    action: action,
                    onException: (ref DbFeedback dbFeedbackInstance, Exception ex) =>
                    {
                        // Custom exception handling logic
                    },
                    commitCondition: commitCondition
                );
            });

            // Ensure transaction is rolled back and connection is open
            Assert.That(dbFeedback.DbTransaction, Is.Null, "Transaction should be null after rollback due to exception.");
            Assert.That(dbFeedback.DbConnection.State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after execution.");
        }

        [Test]
        public void ExecuteWithTransaction_Should_NotOpenConnection_WhenAlreadyOpen()
        {
            // Arrange
            var dbFeedback = new DbFeedback(_dbConnection);
            DbAction action = (ref DbFeedback dbFeedbackInstance) => { /* Perform DB operations */ };

            // Act
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: action,
                onException: null,
                commitCondition: (ref DbFeedback dbFeedbackInstance) => true
            );

            // Assert
            Assert.That(dbFeedback.DbConnection.State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after execution.");
        }

        [Test]
        public void ExecuteWithTransaction_Should_CallFinallyAction_AfterExecution()
        {
            // Arrange
            var dbFeedback = new DbFeedback(_dbConnection);
            bool finallyCalled = false;

            DbAction action = (ref DbFeedback dbFeedbackInstance) => { /* Perform DB operations */ };

            // Act
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: action,
                onException: null,
                commitCondition: (ref DbFeedback dbFeedbackInstance) => true,
                onFinally: () => finallyCalled = true
            );

            // Assert
            Assert.That(finallyCalled, Is.True, "Finally action should be called.");
            Assert.That(dbFeedback.DbConnection.State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after execution.");
        }

        [Test]
        public void ExecuteWithTransaction_Should_HandleExternalConnection_Correctly()
        {
            // Arrange
            var dbFeedback = new DbFeedback(_dbConnection);
            DbAction action = (ref DbFeedback dbFeedbackInstance) => { /* Perform DB operations */ };

            // Act
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                action: action,
                onException: null,
                commitCondition: (ref DbFeedback dbFeedbackInstance) => true
            );

            // Assert
            Assert.That(dbFeedback.DbConnection.State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after execution, even if provided externally.");
        }
    }
}
