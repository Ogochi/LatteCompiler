using System.Collections.Generic;
using System.Linq;
using Common.AST;
using LlvmGenerator.Generators;

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
            EmitExternalFunctionsDeclarations();

            var functionGenerator = new FunctionGenerator();
            program.Functions.ToList().ForEach(function => Emit(functionGenerator.GenerateFromAst(function)));
            
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
                "declare i32 @strEq(i8*, i8*)",
                "declare i8* @strConcat(i8*, i8*)"
            });
        }
    }
}