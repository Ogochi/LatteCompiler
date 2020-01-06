using System.Collections.Generic;
using Common.AST;
using ParsingTools;

namespace Common.StateManagement
{
    public static class PredefinedFunctions
    {
        public static FunctionDef PrintInt = new FunctionDef
        {
            Type = new LatteParser.TVoidContext(),
            Args = new List<Arg> {new Arg(new LatteParser.TIntContext(), "intToPrint")},
            Id = "printInt"
        };
        
        public static FunctionDef ReadInt = new FunctionDef
        {
            Type = new LatteParser.TIntContext(),
            Args = new List<Arg>(),
            Id = "readInt"
        };
        
        public static FunctionDef PrintString = new FunctionDef
        {
            Type = new LatteParser.TVoidContext(),
            Args = new List<Arg> {new Arg(new LatteParser.TStringContext(), "stringToPrint")},
            Id = "printString"
        };
        
        public static FunctionDef ReadString = new FunctionDef
        {
            Type = new LatteParser.TStringContext(),
            Args = new List<Arg>(),
            Id = "readString"
        };
        
        public static FunctionDef Error = new FunctionDef
        {
            Type = new LatteParser.TVoidContext(),
            Args = new List<Arg>(),
            Id = "error"
        };
    }
}