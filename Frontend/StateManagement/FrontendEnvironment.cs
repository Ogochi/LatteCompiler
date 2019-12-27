using System;
using System.Collections.Generic;
using System.Diagnostics;
using Antlr4.Runtime.Misc;
using Common.AST;
using Common.StateManagement;
using ParsingTools;

namespace Frontend.StateManagement
{
    public class FrontendEnvironment
    {
        private readonly Stack<IDictionary<string, VarDef>> _previousScopeVarDefs  = new Stack<IDictionary<string, VarDef>>();
        
        public IDictionary<string, FunctionDef> NameToFunctionDef { get; private set; } =
            new Dictionary<string, FunctionDef>();
        
        public IDictionary<string, VarDef> NameToVarDef { get; private set; } = new Dictionary<string, VarDef>();

        public string CurrentFunctionName { get; set; }
        
        public static FrontendEnvironment Instance { get; } = new FrontendEnvironment();

        private FrontendEnvironment() {}

        public FunctionDef CurrentFunction => NameToFunctionDef[CurrentFunctionName];
        
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
                NameToVarDef[item.Key] = new VarDef(item.Value.Type, item.Value.Name, false);
            }
        }

        public void AddTopDef(LatteParser.TopDefContext topDef)
        {
            switch (topDef)
            {
                case LatteParser.FunctionDefContext functionDef:
                    NameToFunctionDef[functionDef.ID().GetText()] = new FunctionDef(functionDef);
                    break;
                
                case LatteParser.ClassDefContext classDef:
                    throw new NotImplementedException();
            }
        }

        public void AddPredefinedFunctions()
        {
            NameToFunctionDef["printInt"] = PredefinedFunctions.PrintInt;
            NameToFunctionDef["readInt"] = PredefinedFunctions.ReadInt;
            NameToFunctionDef["printString"] = PredefinedFunctions.PrintString;
            NameToFunctionDef["readString"] = PredefinedFunctions.ReadString;
            NameToFunctionDef["error"] = PredefinedFunctions.Error;
        }
    }
}