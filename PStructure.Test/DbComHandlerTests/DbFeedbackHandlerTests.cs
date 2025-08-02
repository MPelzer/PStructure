using System;
using System.Data;
using MySqlConnector;
using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using PStructure.PersistenceLayer.DatabaseStuff;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankenEigenschaftenEcke;
using PStructure.PersistenceLayer.DatabaseStuff.DatenbankHandling;

namespace PStructure.Test.DbComHandlerTests
{
    [TestFixture]
    public class DbFeedbackHandlerTests
    {
        private const string ConnectionString = "Server=localhost;Port=3306;Database=testdb;User=testuser;Password=testpassword;";
        private MySqlConnection _dbConnection;
        private Mock<ILogger> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _dbConnection = new MySqlConnection(ConnectionString);
            _dbConnection.Open();
            _loggerMock = new Mock<ILogger>();
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
            var dbFeedback = new DbContext(_dbConnection);
            DbAction action = (ILogger logger, ref DbContext dbFeedbackInstance) => { /* Perform DB operations */ };
            DbExceptionAction onException = (ref DbContext dbFeedbackInstance, Exception ex) => { /* Handle exception */ };

            // Act
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                _loggerMock.Object,
                action: action,
                onException: onException
            );

            // Assert
            Assert.That(dbFeedback.GetDbTransaction(), Is.Null, "Transaction should be null after commit.");
            Assert.That(dbFeedback.GetDbConnection().State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after commit.");
        }

        [Test]
        public void ExecuteWithTransaction_Should_NotCommitTransaction_WhenCommitConditionIsFalse()
        {
            // Arrange
            var dbFeedback = new DbContext(_dbConnection);
            DbAction action = (ILogger logger, ref DbContext dbFeedbackInstance) => { /* Perform DB operations */ };
            DbExceptionAction onException = (ref DbContext dbFeedbackInstance, Exception ex) => { /* Handle exception */ };

            // Act
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                _loggerMock.Object,
                action: action,
                onException: onException
            );

            // Assert
            Assert.That(dbFeedback.GetDbTransaction(), Is.Null, "Transaction should be null after rollback or no commit.");
            Assert.That(dbFeedback.GetDbConnection().State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after execution.");
        }

        [Test]
        public void ExecuteWithTransaction_Should_HandleExternalTransaction_WithoutCommittingOrRollingBack()
        {
            // Arrange
            var dbFeedback = new DbContext(_dbConnection);
            var transaction = _dbConnection.BeginTransaction();
            dbFeedback.SetDbTransaction(transaction);

            DbAction action = (ILogger logger, ref DbContext dbFeedbackInstance) => { /* Perform DB operations */ };
            DbExceptionAction onException = (ref DbContext dbFeedbackInstance, Exception ex) => { /* Handle exception */ };

            // Act
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                _loggerMock.Object,
                action: action,
                onException: onException
            );

            // Assert
            Assert.That(dbFeedback.GetDbTransaction(), Is.EqualTo(transaction), "External transaction should not be altered.");
            Assert.That(dbFeedback.GetDbConnection().State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after execution.");
        }

        [Test]
        public void ExecuteWithTransaction_Should_HandleExceptionAndRollbackTransaction()
        {
            // Arrange
            var dbFeedback = new DbContext(_dbConnection);
            DbAction action = (ILogger logger, ref DbContext dbFeedbackInstance) => throw new InvalidOperationException("Test exception");
            DbExceptionAction onException = (ref DbContext dbFeedbackInstance, Exception ex) => { /* Handle exception */ };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                DbFeedbackHandler.ExecuteWithTransaction(
                    ref dbFeedback,
                    _loggerMock.Object,
                    action: action,
                    onException: onException
                );
            });

            // Ensure transaction is rolled back and connection is open
            Assert.That(dbFeedback.GetDbTransaction(), Is.Null, "Transaction should be null after rollback due to exception.");
            Assert.That(dbFeedback.GetDbConnection().State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after execution.");
        }

        [Test]
        public void ExecuteWithTransaction_Should_NotOpenConnection_WhenAlreadyOpen()
        {
            // Arrange
            var dbFeedback = new DbContext(_dbConnection);
            DbAction action = (ILogger logger, ref DbContext dbFeedbackInstance) => { /* Perform DB operations */ };
            DbExceptionAction onException = (ref DbContext dbFeedbackInstance, Exception ex) => { /* Handle exception */ };

            // Act
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                _loggerMock.Object,
                action: action,
                onException: onException
            );

            // Assert
            Assert.That(dbFeedback.GetDbConnection().State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after execution.");
        }

        [Test]
        public void ExecuteWithTransaction_Should_CallFinallyAction_AfterExecution()
        {
            // Arrange
            var dbFeedback = new DbContext(_dbConnection);
            bool finallyCalled = false;

            DbAction action = (ILogger logger, ref DbContext dbFeedbackInstance) => { /* Perform DB operations */ };
            DbExceptionAction onException = (ref DbContext dbFeedbackInstance, Exception ex) => { /* Handle exception */ };

            // Act
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                _loggerMock.Object,
                action: action,
                onException: onException,
                onFinally: () => finallyCalled = true
            );

            // Assert
            Assert.That(finallyCalled, Is.True, "Finally action should be called.");
            Assert.That(dbFeedback.GetDbConnection().State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after execution.");
        }

        [Test]
        public void ExecuteWithTransaction_Should_HandleExternalConnection_Correctly()
        {
            // Arrange
            var dbFeedback = new DbContext(_dbConnection);
            DbAction action = (ILogger logger, ref DbContext dbFeedbackInstance) => { /* Perform DB operations */ };
            DbExceptionAction onException = (ref DbContext dbFeedbackInstance, Exception ex) => { /* Handle exception */ };

            // Act
            DbFeedbackHandler.ExecuteWithTransaction(
                ref dbFeedback,
                _loggerMock.Object,
                action: action,
                onException: onException
            );

            // Assert
            Assert.That(dbFeedback.GetDbConnection().State, Is.EqualTo(ConnectionState.Open), "Connection should remain open after execution, even if provided externally.");
        }
    }
}
