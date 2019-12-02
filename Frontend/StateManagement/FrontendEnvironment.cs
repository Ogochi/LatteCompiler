using System.Collections.Generic;
using ParsingTools;

namespace Frontend.StateManagement
{
    public class FrontendEnvironment
    {
        private Stack<IDictionary<string, VarDef>> _previousScopeVarDefs  = new Stack<IDictionary<string, VarDef>>();
        
        public IDictionary<string, LatteParser.FunctionDefContext> NameToFunctionDef { get; private set; } =
            new Dictionary<string, LatteParser.FunctionDefContext>();


        public static FrontendEnvironment Instance { get; } = new FrontendEnvironment();

        private FrontendEnvironment() {}
        public IDictionary<string, VarDef> NameToVarDef { get; private set; } = new Dictionary<string, VarDef>();

        public void RestorePreviousVarEnv()
        {
            NameToVarDef = _previousScopeVarDefs.Pop();
        }

        public void DetachVarEnv()
        {
            _previousScopeVarDefs.Push(NameToVarDef);
            
            NameToVarDef = new Dictionary<string, VarDef>();
            foreach (var item in _previousScopeVarDefs.Peek())
            {
                NameToVarDef[item.Key] = new VarDef(item.Value.Type, item.Value.Name);
            }
        }
    }
}