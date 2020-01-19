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
        
        private readonly Stack<IDictionary<string, FunctionDef>> _previousScopeFuncDefs = new Stack<IDictionary<string, FunctionDef>>();
        
        public IDictionary<string, FunctionDef> NameToFunctionDef { get; private set; } = new Dictionary<string, FunctionDef>();
        
        public IDictionary<string, ClassDef> NameToClassDef { get; } = new Dictionary<string, ClassDef>();
        
        public IDictionary<string, VarDef> NameToVarDef { get; private set; } = new Dictionary<string, VarDef>();

        public string CurrentFunctionName { get; set; }
        
        public string CurrentClassName { get; set; }
        
        public static FrontendEnvironment Instance { get; } = new FrontendEnvironment();

        private FrontendEnvironment() {}

        public FunctionDef CurrentFunction => NameToFunctionDef[CurrentFunctionName];
        
        public void RestorePreviousVarEnv()
        {
            NameToVarDef = _previousScopeVarDefs.Pop();
        }

        public void RestorePreviousFuncEnv()
        {
            NameToFunctionDef = _previousScopeFuncDefs.Pop();
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

        public void DetachFuncEnv()
        {
            _previousScopeFuncDefs.Push(NameToFunctionDef);
            
            NameToFunctionDef = new Dictionary<string, FunctionDef>();
            foreach (var item in _previousScopeFuncDefs.Peek())
            {
                NameToFunctionDef[item.Key] = 
                    new FunctionDef {Args = item.Value.Args, Block = item.Value.Block, Id = item.Value.Id, Type = item.Value.Type};
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
                    NameToClassDef[classDef.ID()[0].GetText()] = new ClassDef(classDef);
                    break;
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