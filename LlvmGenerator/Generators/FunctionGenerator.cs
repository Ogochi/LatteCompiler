using Common.AST;
using LlvmGenerator.StateManagement;
using ParsingTools;

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

            if (function.Type is LatteParser.TVoidContext && _llvmGenerator.LastEmitted() != "ret void")
            {
                _llvmGenerator.Emit("ret void");
            }
            
            _llvmGenerator.Emit("}");
        }
    }
}