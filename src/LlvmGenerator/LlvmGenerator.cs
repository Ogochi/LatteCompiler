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
        private readonly FunctionsGlobalState _globalState = FunctionsGlobalState.Instance;
        
        private List<string> EmittedCode = new List<string>(); 
        
        private LlvmGenerator() {}

        public int GetCurrentLength()
        {
            return EmittedCode.Count;
        }

        public void ReplaceRegisters(int from, List<(string, string)> fromTo)
        {
            for (var i = from; i < EmittedCode.Count; i++)
            {
                var currentStr = EmittedCode[i];
                fromTo.ForEach(ft => currentStr = currentStr.Replace(ft.Item1, ft.Item2));
                EmittedCode[i] = currentStr;
            }
        }
        
        public void Emit(string s)
        {
            EmittedCode.Add(s);
        }

        public void Emit(IList<string> s)
        {
            s.ToList().ForEach(Emit);
        }

        public string LastEmitted()
        {
            return EmittedCode.FindLast(a => true);
        }

        public List<string> GenerateFromAst(Program program)
        {
            _globalState.AddFunctions(program.Functions);
            _globalState.AddFunctions(ExternalFunctions());
            EmitExternalFunctionsDeclarations();

            _globalState.AddClasses(program.Classes);
            _globalState.AddParentFields(program.Classes.ToList());

            var classGenerator = new ClassGenerator();
            program.Classes.ToList().ForEach(@class =>
            {
                classGenerator.GenerateType(@class);
                classGenerator.GenerateConstructor(@class);
            });
            program.Classes.ToList().ForEach(classGenerator.GenerateMethods);
            
            _globalState.AddParentMethods(program.Classes.ToList());
            
            program.Classes.ToList().ForEach(classGenerator.GenerateVTable);

            var functionGenerator = new FunctionGenerator();
            program.Functions.ToList().ForEach(function =>
            {
                if (function.IsMethod)
                {
                    return;
                }
                functionGenerator.GenerateFromAst(function);
            });

            EmitStringConsts();
            
            var result = EmittedCode;
            EmittedCode = new List<string>();

            return result;
        }

        private void EmitStringConsts()
        {
            foreach (var item in FunctionsGlobalState.Instance.LiteralToStringConstId)
            {
                Emit($"{item.Value} = private constant [{item.Key.Length + 1} x i8] c\"{item.Key}\\00\"");
            }
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