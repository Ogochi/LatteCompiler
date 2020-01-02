using System.Collections.Generic;
using System.Linq;
using Common.AST;

namespace LlvmGenerator.StateManagement
{
    public class FunctionGeneratorState
    {
        public Dictionary<string, List<RegisterLabelContext>> VarToRegister { get; private set; } = 
            new Dictionary<string, List<RegisterLabelContext>>();
        
        public Dictionary<string, string> LiteralToStringConstId= new Dictionary<string, string>();

        public bool HasGeneratedAnything = false;

        public const string EntryLabel = "entry";
        
        private readonly Stack<Dictionary<string, List<RegisterLabelContext>>> _previousScopeVars = 
            new Stack<Dictionary<string, List<RegisterLabelContext>>>();

        private int _registerCounter, _labelCounter, _stringCounter;

        public FunctionGeneratorState(FunctionDef function)
        {
            function.Args.ForEach(arg => VarToRegister.Add(
                arg.Id,
                new List<RegisterLabelContext> {new RegisterLabelContext(NewRegister, EntryLabel, arg.Type)}));
        }
        
        public void RestorePreviousVarEnv()
        {
            VarToRegister = _previousScopeVars.Pop();
        }

        public void DetachVarEnv()
        {
            _previousScopeVars.Push(VarToRegister);
            
            VarToRegister = new Dictionary<string, List<RegisterLabelContext>>();
            _previousScopeVars.Peek().ToList().ForEach(var => 
                VarToRegister.Add(var.Key, var.Value));
        }

        public string NewRegister => $"%r{_registerCounter++}";
        
        public string NewLabel => $"l{_labelCounter++}";

        public string NewString => $"@.str{_stringCounter++}";
    }
}