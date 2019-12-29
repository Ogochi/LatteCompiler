using Common.AST.Stmts;
using LlvmGenerator.StateManagement;

namespace LlvmGenerator.Generators
{
    public class StmtGeneratorVisitor
    {
        private LlvmGenerator _llvmGenerator = LlvmGenerator.Instance;
        private FunctionGeneratorState _state;
        
        public StmtGeneratorVisitor(FunctionGeneratorState state)
        {
            _state = state;
        }

        public void Visit(Block block)
        {
            
        }
        
        public void Visit(ExpStmt expStmt)
        {
            
        }

        public void Visit(Decl decl)
        {
            
        }
        
        public void Visit(Ass ass)
        {
            
        }
        
        public void Visit(Ret ret)
        {
            
        }
    }
}