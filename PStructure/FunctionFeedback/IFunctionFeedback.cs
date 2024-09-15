using System;

namespace PStructure.Interfaces
{
    public interface IFunctionFeedback
    {
        bool RequestAnswer { get; set; }
        Exception RequestException { get; set; }
        bool SilentThrow { get; set; }
    }
}