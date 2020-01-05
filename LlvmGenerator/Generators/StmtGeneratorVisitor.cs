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
        private readonly LlvmGenerator _llvmGenerator = LlvmGenerator.Instance;
        private readonly FunctionGeneratorState _state;
        
        public StmtGeneratorVisitor(FunctionGeneratorState state)
        {
            _state = state;
        }

        public override void Visit(CondElse condElse)
        {
            var expr = new ExpressionSimplifierVisitor().Visit(condElse.Expr);
            if (expr is Bool b)
            {
                Visit(b.Value ? condElse.TBlock : condElse.FBlock);
                return;
            }

            var exprResult = new ExpressionGeneratorVisitor(_state).Visit(expr);
            _state.ConsolidateVariables();

            var startLabel = _state.CurrentLabel;
            _state.GoToNextLabel(out var trueLabel);
            string falseLabel = _state.NewLabel, endIfLabel = _state.NewLabel;
            _llvmGenerator.Emit($"br i1 {exprResult.Register}, label %{trueLabel}, label %{falseLabel}");
            
            _llvmGenerator.Emit($"{trueLabel}:");
            VisitBlockWithoutRestore(condElse.TBlock);
            
            _state.RestorePreviousVarEnvWithMerge();
            _state.ConsolidateVariables();
            _llvmGenerator.Emit($"br label %{endIfLabel}");
            
            _state.CurrentLabel = falseLabel;
            _llvmGenerator.Emit($"{falseLabel}:");
            VisitBlockWithoutRestore(condElse.FBlock);
            
            _state.ConsolidateVariables();
            _state.RestorePreviousVarEnvWithMerge();
            _llvmGenerator.Emit($"br label %{endIfLabel}");
            
            _state.CurrentLabel = endIfLabel;
            _llvmGenerator.Emit($"{endIfLabel}:");
        }

        public override void Visit(While @while)
        {
            var expr = new ExpressionSimplifierVisitor().Visit(@while.Expr);
            if (expr is Bool b && b.Value == false)
            {
                return;
            }
            _state.ConsolidateVariables();

            _state.GoToNextLabel(out var startWhileLabel);
            _llvmGenerator.Emit($"br label %{startWhileLabel}");
            
            _state.DetachVarEnv();
            _state.DetachVarEnv();
            var reservedRegisters = _state.ReserveRegisterForCurrentVars(startWhileLabel);

            _state.GoToNextLabel(out var whileLabel);
            _llvmGenerator.Emit($"{whileLabel}:");
            var startLength = _llvmGenerator.GetCurrentLength();
            VisitBlock(@while.Block);
            
            _state.ConsolidateVariables();
            _state.RestorePreviousVarEnvWithMerge();
            _llvmGenerator.Emit($"br label %{startWhileLabel}");

            _state.CurrentLabel = startWhileLabel;
            _llvmGenerator.Emit($"{startWhileLabel}:");
            _state.RestorePreviousVarEnvWithMerge();
            
            _state.RemoveReservedRegisters(reservedRegisters, out var removedRegisters);
            _state.ConsolidateVariables(reservedRegisters);

            var removedRegs = removedRegisters.ToHashSet();
            var reservedToReplace = reservedRegisters.ToList().Where(reg => removedRegs.Contains(reg.Value.Register)).ToList();

            _llvmGenerator.ReplaceRegisters(startLength, reservedToReplace.Select(res => 
                (res.Value.Register, _state.VarToLabelToRegister[res.Key][_state.CurrentLabel].Register)).ToList());
            
            var exprResult = new ExpressionGeneratorVisitor(_state).Visit(expr);
            var endWhileLabel = _state.NewLabel;
            _llvmGenerator.Emit($"br i1 {exprResult.Register}, label %{whileLabel}, label %{endWhileLabel}");
            
            _state.CurrentLabel = endWhileLabel;
            _llvmGenerator.Emit($"{endWhileLabel}:");
        }

        public override void Visit(Cond cond)
        {
            var expr = new ExpressionSimplifierVisitor().Visit(cond.Expr);
            if (expr is Bool b)
            {
                if (b.Value)
                {
                    Visit(cond.Block);
                }
                return;
            }
            
            var exprResult = new ExpressionGeneratorVisitor(_state).Visit(expr);
            _state.ConsolidateVariables();
            
            _state.GoToNextLabel(out var trueLabel);
            var falseLabel = _state.NewLabel;
            _llvmGenerator.Emit($"br i1 {exprResult.Register}, label %{trueLabel}, label %{falseLabel}");

            _llvmGenerator.Emit($"{trueLabel}:");
            VisitBlockWithoutRestore(cond.Block);
            
            _state.ConsolidateVariables();
            _state.RestorePreviousVarEnvWithMerge();
            
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
            VisitBlockWithoutRestore(block);
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
                var expr = item.Expr != null
                    ? new ExpressionSimplifierVisitor().Visit(item.Expr) 
                    : Common.AST.Exprs.Utils.DefaultValueForType(decl.Type);
                var exprResult = new ExpressionGeneratorVisitor(_state).Visit(expr);
                
                _state.VarToLabelToRegister[item.Id] = 
                    new Dictionary<string, RegisterLabelContext> {{exprResult.Label, exprResult}};

                if (_state.VarToLabelToRegister.ContainsKey(item.Id))
                {
                    _state.RedefinedVars.Add(item.Id);
                }
            });
        }
        
        public override void Visit(Ass ass)
        {
            var expr = new ExpressionSimplifierVisitor().Visit(ass.Expr);
            var exprResult = new ExpressionGeneratorVisitor(_state).Visit(expr);

            _state.VarToLabelToRegister[ass.Id] = 
                new Dictionary<string, RegisterLabelContext> {{exprResult.Label, exprResult}};
        }
        
        public override void Visit(Ret ret)
        {
            var toEmit = "ret ";

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

        private void VisitBlockWithoutRestore(Block block)
        {
            _state.DetachVarEnv();
            VisitBlock(block);
        }

        private void VisitBlock(Block block)
        {
            block.Stmts.ToList().ForEach(Visit);
        }
    }
}