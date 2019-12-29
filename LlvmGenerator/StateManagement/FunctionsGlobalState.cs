using System.Collections.Generic;
using System.Linq;
using Common.AST;

namespace LlvmGenerator.StateManagement
{
    public class FunctionsGlobalState
    {
        public Dictionary<string, FunctionDef> NameToFunction { get; } = new Dictionary<string, FunctionDef>();

        public static FunctionsGlobalState Instance { get; } = new FunctionsGlobalState();
        
        private FunctionsGlobalState() {}

        public void AddFunctions(IList<FunctionDef> functions)
        {
            functions.ToList().ForEach(function => NameToFunction[function.Id] = function);
        }
    }
}