using System.Collections.Generic;
using System.Linq;
using Common.AST;
using LlvmGenerator.Generators;
using LlvmGenerator.StateManagement;
using ParsingTools;

namespace LlvmGenerator
{
    public class LlvmGenerator
    {
        public static LlvmGenerator Instance { get; } = new LlvmGenerator();
        
        private List<string> EmittedCode = new List<string>(); 
        
        private LlvmGenerator() {}

        public void Emit(string s)
        {
            EmittedCode.Add(s);
        }

        public void Emit(IList<string> s)
        {
            s.ToList().ForEach(Emit);
        }

        public List<string> GenerateFromAst(Program program)
        {
            FunctionsGlobalState.Instance.AddFunctions(program.Functions);
            FunctionsGlobalState.Instance.AddFunctions(ExternalFunctions());
            EmitExternalFunctionsDeclarations();

            var functionGenerator = new FunctionGenerator();
            program.Functions.ToList().ForEach(function => functionGenerator.GenerateFromAst(function));
            
            var result = EmittedCode;
            EmittedCode = new List<string>();
            return result;
        }

        private void EmitExternalFunctionsDeclarations()
        {
            Emit(new List<string>
            {
                "declare void @printInt(i32)",
                "declare void @printString(i8*)",
                "declare void @error()",
                "declare i32 @readInt()",
                "declare i8* @readString()",
                "declare i1 @strEq(i8*, i8*)",
                "declare i8* @strConcat(i8*, i8*)",
                "declare i8* @mmalloc(i32)"
            });
        }

        public static List<FunctionDef> ExternalFunctions()
        {
            return new List<FunctionDef>
            {
                new FunctionDef {Id = "printInt", Args = new List<Arg> {new Arg(new LatteParser.TIntContext(), "")}, Type = new LatteParser.TVoidContext()},
                new FunctionDef {Id = "printString", Args = new List<Arg> {new Arg(new LatteParser.TStringContext(), "")}, Type = new LatteParser.TVoidContext()},
                new FunctionDef {Id = "error", Args = new List<Arg>(), Type = new LatteParser.TVoidContext()},
                new FunctionDef {Id = "readInt", Args = new List<Arg>(), Type = new LatteParser.TIntContext()},
                new FunctionDef {Id = "readString", Args = new List<Arg>(), Type = new LatteParser.TStringContext()}
            };
        }
    }
}