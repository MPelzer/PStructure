using System;
using PStructure.Interfaces;

namespace PStructure.FunctionFeedback
{
    public class FunctionFeedback : IFunctionFeedback
    {
        public bool RequestAnswer { get; set; } = false;
        public Exception RequestException { get; set; } = null;
        public bool SilentThrow { get; set; } = false;
    }
}