using System;
using System.Linq;
using Common.AST;
using Common.AST.Stmts;
using LlvmGenerator.StateManagement;
using LlvmGenerator.Utils;

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
            var expr = new ExpressionSimplifierVisitor().Visit(expStmt.Expr);
            new ExpressionGeneratorVisitor(_state).Visit(expr);
        }

        public override void Visit(Decl decl)
        {
            decl.Items.ToList().ForEach(item => Visit(new Ass(
                item.Id, 
                item.Expr ?? Common.AST.Exprs.Utils.DefaultValueForType(decl.Type))));
        }
        
        public override void Visit(Ass ass)
        {

        }
        
        public override void Visit(Ret ret)
        {
            string toEmit = "ret ";

            if (ret.Expr == null)
            {
                toEmit += "void";
            }
            else
            {
                var expr = new ExpressionSimplifierVisitor().Visit(ret.Expr);
                var (exprType, exprResult) = new ExpressionGeneratorVisitor(_state).Visit(expr);
                toEmit += $"{AstToLlvmString.Type(exprType)} {exprResult.Register}";
            }
            
            _llvmGenerator.Emit(toEmit);
        }
    }
}