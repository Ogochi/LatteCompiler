using System.Collections.Generic;
using Common.AST;

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

        public List<string> GenerateFromAst(Program program)
        {
            Emit("");
            // TODO - generate
            
            var result = EmittedCode;
            EmittedCode = new List<string>();
            return result;
        }
    }
}