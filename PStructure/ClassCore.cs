using System.Runtime.CompilerServices;

namespace PStructure
{
    public class ClassCore
    {
        public string PrintLocation([CallerMemberName] string functionName = null)
        {
            return $"{GetType().Name}.{functionName}:";
        }
    }
}