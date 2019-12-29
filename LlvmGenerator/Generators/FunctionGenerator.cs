using Common.AST;
using LlvmGenerator.StateManagement;

namespace LlvmGenerator.Generators
{
    public class FunctionGenerator
    {
        private LlvmGenerator _llvmGenerator = LlvmGenerator.Instance;
        
        public void GenerateFromAst(FunctionDef function)
        {
            var state = new FunctionGeneratorState(function);
            _llvmGenerator.Emit(Utils.AstToLlvmString.FunctionHeader(function, state));
            _llvmGenerator.Emit(FunctionGeneratorState.EntryLabel + ":");
            
            // TODO - generate content
            
            _llvmGenerator.Emit("}");
        }
    }
}