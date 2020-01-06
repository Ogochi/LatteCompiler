using System.Collections.Generic;
using System.Linq;
using Common.AST;

namespace LlvmGenerator.StateManagement
{
    public class FunctionsGlobalState
    {
        public Dictionary<string, FunctionDef> NameToFunction { get; } = new Dictionary<string, FunctionDef>();
        
        public readonly Dictionary<string, string> LiteralToStringConstId= new Dictionary<string, string>();

        public static FunctionsGlobalState Instance { get; } = new FunctionsGlobalState();
        
        private int _stringCounter;
        
        private FunctionsGlobalState() {}

        public void AddFunctions(IList<FunctionDef> functions)
        {
            functions.ToList().ForEach(function => NameToFunction[function.Id] = function);
        }
        
        public string NewString => $"@.str{_stringCounter++}";
    }
}