using System;

namespace PStructure.FunctionFeedback
{
    public class DbResult : IFunctionFeedback
    {
        private bool _requestAnswer;
        private string _request;
        private Exception _requestException;
        private bool _silentThrow;


        public void SetRequest(string request)
        {
            _request = request;
        }

        public string GetRequest()
        {
            return _request;
        }
        
        public void SetRequestAnswer(bool value)
        {
            _requestAnswer = value;
        }

        public bool GetRequestAnswer()
        {
            return _requestAnswer;
        }

        public void SetRequestException(Exception exception)
        {
            _requestException = exception;
        }

        public Exception GetRequestException()
        {
            return _requestException;
        }

        public void SetSilentThrow(bool value)
        {
            _silentThrow = value;
        }

        public bool ExceptionGetsThrownSilently()
        {
            return _silentThrow;
        }

        public void SetBackToDefault()
        {
            _requestAnswer = false;
            _requestException = null;
            _silentThrow = false;
        }
    }
}