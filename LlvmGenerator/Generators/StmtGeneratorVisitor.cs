using System;
using System.Linq;
using Common.AST;
using Common.AST.Stmts;
using LlvmGenerator.StateManagement;

namespace LlvmGenerator.Generators
{
    public class StmtGeneratorVisitor : BaseAstVisitor
    {
        private LlvmGenerator _llvmGenerator = LlvmGenerator.Instance;
        private FunctionGeneratorState _state;
        
        public StmtGeneratorVisitor(FunctionGeneratorState state)
        {
            _state = state;
        }

        public override void Visit(Block block)
        {
            _state.DetachVarEnv();
            
            block.Stmts.ToList().ForEach(Visit);
            
            _state.RestorePreviousVarEnv();
        }
        
        public override void Visit(ExpStmt expStmt)
        {
            
        }

        public override void Visit(Decl decl)
        {

        }
        
        public override void Visit(Ass ass)
        {
            
        }
        
        public override void Visit(Ret ret)
        {
            
        }
    }
}