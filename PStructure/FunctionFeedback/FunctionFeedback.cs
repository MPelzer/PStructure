using System;

namespace PStructure.FunctionFeedback
{
    public class FunctionFeedback : IFunctionFeedback
    {
        private bool _requestAnswer;
        private Exception _requestException;
        private bool _silentThrow;

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