using System.Collections.Generic;
using System.Linq;
using Common.AST;
using Common.AST.Exprs;
using LlvmGenerator.Generators;

namespace LlvmGenerator.StateManagement
{
    public class FunctionGeneratorState
    {
        public Dictionary<string, Dictionary<string, RegisterLabelContext>> VarToLabelToRegister { get; private set; } = 
            new Dictionary<string, Dictionary<string, RegisterLabelContext>>();
        
        public Dictionary<string, Dictionary<string, RegisterLabelContext>> RedefinedVars = 
            new Dictionary<string, Dictionary<string, RegisterLabelContext>>();

        public string CurrentLabel = EntryLabel;

        public const string EntryLabel = "entry";

        public string CurrentFunction;
        
        private readonly Stack<Dictionary<string, Dictionary<string, RegisterLabelContext>>> _previousScopeVars = 
            new Stack<Dictionary<string, Dictionary<string, RegisterLabelContext>>>();
        
        private readonly Stack<Dictionary<string, Dictionary<string, RegisterLabelContext>>> _previousScopeRedefined = 
            new Stack<Dictionary<string, Dictionary<string, RegisterLabelContext>>>();

        private int _registerCounter, _labelCounter;
        
        private readonly FunctionsGlobalState _globalState = FunctionsGlobalState.Instance;

        public FunctionGeneratorState(FunctionDef function)
        {
            CurrentFunction = function.Id;

            function.Args.ForEach(arg => VarToLabelToRegister.Add(
                arg.Id,
                new Dictionary<string, RegisterLabelContext> {{EntryLabel, new RegisterLabelContext(NewRegister, EntryLabel, arg.Type)}}));
        }
        
        public void RestorePreviousVarEnv()
        {
            VarToLabelToRegister = _previousScopeVars.Pop();
            RedefinedVars = _previousScopeRedefined.Pop();
        }

        public void RemoveReservedRegisters(Dictionary<string, RegisterLabelContext> reserved, out List<string> removedRegisters)
        {
            var toRemove = new List<RegisterLabelContext>();
            var reservedRegisters = reserved.Values.Select(reg => reg.Register).ToHashSet();

            foreach (var var in reserved.Keys)
            {
                var currentToRemove = VarToLabelToRegister[var].Values
                    .Where(reg => reservedRegisters.Contains(reg.Register))
                    .ToList();

                currentToRemove.ForEach(reg => VarToLabelToRegister[var].Remove(reg.Label));
                currentToRemove.ForEach(toRemove.Add);
            }

            removedRegisters = toRemove.Select(reg => reg.Register).ToList();
        }

        public Dictionary<string, RegisterLabelContext> ReserveRegisterForCurrentVars(string label)
        {
            var result = VarToLabelToRegister.ToDictionary(
                var => var.Key,
                var => new RegisterLabelContext(NewRegister, label, var.Value.Values.First().Type));
            
            result.ToList().ForEach(item => 
                VarToLabelToRegister[item.Key] = new Dictionary<string, RegisterLabelContext> {{item.Value.Label, item.Value}});

            return result;
        }

        public void RestorePreviousVarEnvWithMerge()
        {
            var currentEnv = VarToLabelToRegister;

            foreach (var var in currentEnv)
            {
                var.Value.ToList().ForEach(v =>
                {
                    if (!RedefinedVars.ContainsKey(var.Key))
                    {
                        _previousScopeVars.Peek()[var.Key][v.Key] = v.Value;
                    }
                    else
                    {
                        _previousScopeVars.Peek()[var.Key] = RedefinedVars[var.Key];
                    }
                });
            }
            
            RestorePreviousVarEnv();
        }

        public void DetachVarEnv()
        {
            _previousScopeVars.Push(VarToLabelToRegister);
            _previousScopeRedefined.Push(RedefinedVars);
            
            VarToLabelToRegister = new Dictionary<string, Dictionary<string, RegisterLabelContext>>();
            _previousScopeVars.Peek().ToList().ForEach(var => 
                VarToLabelToRegister.Add(var.Key, var.Value));
            RedefinedVars = new Dictionary<string, Dictionary<string, RegisterLabelContext>>();
        }

        public void ConsolidateVariables()
        {
            foreach (var var in VarToLabelToRegister)
            {
                var onlyRegister = var.Value.ContainsKey(CurrentLabel) 
                    ? var.Value[CurrentLabel] 
                    : new ExpressionGeneratorVisitor(this).Visit(new ID {Id = var.Key});

                var.Value.Clear();
                var.Value[CurrentLabel] = onlyRegister;
            }
        }
        
        public void ConsolidateVariables(Dictionary<string, RegisterLabelContext> reservedRegisters)
        {
            foreach (var var in VarToLabelToRegister)
            {
                var onlyRegister = var.Value.ContainsKey(CurrentLabel) 
                    ? var.Value[CurrentLabel] 
                    : new ExpressionGeneratorVisitor(this).VisitID(new ID {Id = var.Key}, reservedRegisters[var.Key].Register);

                var.Value.Clear();
                var.Value[CurrentLabel] = onlyRegister;
            }
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