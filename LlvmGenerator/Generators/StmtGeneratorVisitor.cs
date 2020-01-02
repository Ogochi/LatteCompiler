using System;
using System.Collections.Generic;
using System.Linq;
using Common.AST;
using Common.AST.Exprs;
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

        public override void Visit(Cond cond)
        {
            var expr = new ExpressionSimplifierVisitor().Visit(cond.Expr);
            var exprResult = new ExpressionGeneratorVisitor(_state).Visit(expr);
            
            _state.GoToNextLabel(out var trueLabel);
            var falseLabel = _state.NewLabel;

            _llvmGenerator.Emit($"br i1 {exprResult.Register}, label %{trueLabel}, label %{falseLabel}");
            _llvmGenerator.Emit($"{trueLabel}:");
            Visit(cond.Block);
            
            _llvmGenerator.Emit($"br label %{falseLabel}");
            _state.CurrentLabel = falseLabel;
            _llvmGenerator.Emit($"{falseLabel}:");
        }

        public override void Visit(Decr decr)
        {
            Visit(new Ass(decr.Id, new AddOp {Add = Add.Minus, Lhs = new ID {Id = decr.Id}, Rhs = new Int {Value = 1}}));
        }

        public override void Visit(Incr incr)
        {
            Visit(new Ass(incr.Id, new AddOp {Add = Add.Plus, Lhs = new ID {Id = incr.Id}, Rhs = new Int {Value = 1}}));
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
            decl.Items.ToList().ForEach(item =>
            {
                _state.VarToRegister[item.Id] = new List<RegisterLabelContext>();
                Visit(new Ass(
                    item.Id,
                    item.Expr ?? Common.AST.Exprs.Utils.DefaultValueForType(decl.Type)));
            });
        }
        
        public override void Visit(Ass ass)
        {
            var expr = new ExpressionSimplifierVisitor().Visit(ass.Expr);
            var exprResult = new ExpressionGeneratorVisitor(_state).Visit(expr);

            _state.VarToRegister[ass.Id].Add(exprResult);
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
                var exprResult = new ExpressionGeneratorVisitor(_state).Visit(expr);
                toEmit += $"{AstToLlvmString.Type(exprResult.Type)} {exprResult.Register}";
            }
            
            _llvmGenerator.Emit(toEmit);
        }
    }
}