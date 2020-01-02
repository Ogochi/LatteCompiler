using System.Collections.Generic;
using System.Linq;
using Common.AST;

namespace LlvmGenerator.StateManagement
{
    public class FunctionGeneratorState
    {
        public Dictionary<string, List<RegisterLabelContext>> VarToRegister { get; private set; } = 
            new Dictionary<string, List<RegisterLabelContext>>();

        public string CurrentLabel = EntryLabel;

        public const string EntryLabel = "entry";
        
        private readonly Stack<Dictionary<string, List<RegisterLabelContext>>> _previousScopeVars = 
            new Stack<Dictionary<string, List<RegisterLabelContext>>>();

        private int _registerCounter, _labelCounter;

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

        public void GoToNextLabel(out string nextLabel)
        {
            nextLabel = NewLabel;
            CurrentLabel = nextLabel;
        }

        public string NewRegister => $"%r{_registerCounter++}";

        private string NewLabel => $"l{_labelCounter++}";
    }
}