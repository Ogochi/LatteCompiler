using System.Collections.Generic;
using System.Linq;
using Common.AST;

namespace LlvmGenerator.StateManagement
{
    public class FunctionGeneratorState
    {
        public Dictionary<string, Dictionary<string, RegisterLabelContext>> VarToLabelToRegister { get; private set; } = 
            new Dictionary<string, Dictionary<string, RegisterLabelContext>>();

        public string CurrentLabel = EntryLabel;

        public const string EntryLabel = "entry";
        
        private readonly Stack<Dictionary<string, Dictionary<string, RegisterLabelContext>>> _previousScopeVars = 
            new Stack<Dictionary<string, Dictionary<string, RegisterLabelContext>>>();

        private int _registerCounter, _labelCounter;

        public FunctionGeneratorState(FunctionDef function)
        {
            function.Args.ForEach(arg => VarToLabelToRegister.Add(
                arg.Id,
                new Dictionary<string, RegisterLabelContext> {{EntryLabel, new RegisterLabelContext(NewRegister, EntryLabel, arg.Type)}}));
        }
        
        public void RestorePreviousVarEnv()
        {
            VarToLabelToRegister = _previousScopeVars.Pop();
        }

        public void DetachVarEnv()
        {
            _previousScopeVars.Push(VarToLabelToRegister);
            
            VarToLabelToRegister = new Dictionary<string, Dictionary<string, RegisterLabelContext>>();
            _previousScopeVars.Peek().ToList().ForEach(var => 
                VarToLabelToRegister.Add(var.Key, var.Value));
        }

        public void GoToNextLabel(out string nextLabel)
        {
            nextLabel = NewLabel;
            CurrentLabel = nextLabel;
        }

        public string NewRegister => $"%r{_registerCounter++}";

        public string NewLabel => $"l{_labelCounter++}";
    }
}