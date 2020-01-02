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

            new StmtGeneratorVisitor(state).Visit(function.Block);
            
            _llvmGenerator.Emit("}");
            foreach (var item in state.LiteralToStringConstId)
            {
                _llvmGenerator.Emit($"{item.Value} = private constant [{item.Key.Length + 1} x i8] c\"{item.Key}\\00\"");
            }
        }
    }
}