using System;

namespace PStructure.PersistenceLayer.DatabaseStuff.DatenbankHandling
{
    public interface IFunctionFeedback
    {
        void SetRequestAnswer(bool value);
        bool GetRequestAnswer();
        void SetRequestException(Exception exception);
        Exception GetRequestException();
        void SetSilentThrow(bool value);
        bool ExceptionGetsThrownSilently();
        void SetBackToDefault();
    }
}