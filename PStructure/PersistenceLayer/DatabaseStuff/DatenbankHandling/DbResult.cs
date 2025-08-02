using System;

namespace PStructure.PersistenceLayer.DatabaseStuff.DatenbankHandling
{
    /// <summary>
    /// Liefert Feedbackinformationen zu einer Datenbankoperation.
    /// </summary>
    public class DbResult : IFunctionFeedback
    {
        public bool RequestAnswer { get; set; }
        public string Request { get; set; }
        public Exception RequestException { get; set; }
        public bool SilentThrow { get; set; }

        public void SetRequestAnswer(bool value)
        {
            throw new NotImplementedException();
        }

        public bool GetRequestAnswer()
        {
            throw new NotImplementedException();
        }

        public void SetRequestException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public Exception GetRequestException()
        {
            throw new NotImplementedException();
        }

        public void SetSilentThrow(bool value)
        {
            throw new NotImplementedException();
        }

        public bool ExceptionGetsThrownSilently()
        {
            throw new NotImplementedException();
        }

        public void SetBackToDefault()
        {
            RequestAnswer = false;
            RequestException = null;
            SilentThrow = false;
        }
    }
}