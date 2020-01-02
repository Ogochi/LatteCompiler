using System;
using System.Linq;
using System.Text;
using Common.AST;
using Common.AST.Exprs;
using LlvmGenerator.StateManagement;
using LlvmGenerator.Utils;
using ParsingTools;

namespace LlvmGenerator.Generators
{
    public class ExpressionGeneratorVisitor : BaseExprAstVisitor<RegisterLabelContext>
    {
        private LlvmGenerator _llvmGenerator = LlvmGenerator.Instance;
        private FunctionGeneratorState _state;
        private FunctionsGlobalState _globalState = FunctionsGlobalState.Instance;

        public ExpressionGeneratorVisitor(FunctionGeneratorState state)
        {
            _state = state;
        }

        public override RegisterLabelContext Visit(AddOp addOp)
        {
            return base.Visit(addOp);
        }

        public override RegisterLabelContext Visit(And and)
        {
            var c1 = Visit(and.Lhs);
            var c2 = Visit(and.Rhs);

            var nextRegister = _state.NewRegister;
            _llvmGenerator.Emit($"{nextRegister} = and i1 {c1.Register}, {c2.Register}");

            return new RegisterLabelContext(nextRegister, null, new LatteParser.TBoolContext());
        }

        public override RegisterLabelContext Visit(Bool @bool)
        {
            return new RegisterLabelContext(@bool.Value ? "1" : "0", null, new LatteParser.TBoolContext());
        }

        public override RegisterLabelContext Visit(FunCall funCall)
        {
            var function = _globalState.NameToFunction[AstToLlvmString.FunctionName(funCall.Id)];
            
            var toEmit = new StringBuilder();
            var nextRegister = "";
            if (!(function.Type is LatteParser.TVoidContext))
            {
                nextRegister = _state.NewRegister;
                toEmit = new StringBuilder($"{nextRegister} = ");
            }

            toEmit.Append($"call {AstToLlvmString.Type(function.Type)} @{function.Id}(");

            var isFirstArg = true;
            foreach (var expr in funCall.Exprs)
            {
                if (!isFirstArg)
                {
                    toEmit.Append(", ");
                }

                var exprResult = Visit(expr);
                toEmit.Append($"{AstToLlvmString.Type(exprResult.Type)} {exprResult.Register}");
                
                isFirstArg = false;
            }
            
            toEmit.Append(")");
            _llvmGenerator.Emit(toEmit.ToString());
            return new RegisterLabelContext(nextRegister, null, function.Type);
        }

        public override RegisterLabelContext Visit(ID id)
        {
            return base.Visit(id);
        }

        public override RegisterLabelContext Visit(Int @int)
        {
            return new RegisterLabelContext(@int.Value.ToString(), null, new LatteParser.TIntContext());
        }

        public override RegisterLabelContext Visit(MulOp mulOp)
        {
            var c1 = Visit(mulOp.Lhs);
            var c2 = Visit(mulOp.Rhs);

            var nextRegister = _state.NewRegister;
            _llvmGenerator.Emit($"{nextRegister} = {AstToLlvmString.Mul(mulOp.Mul)} i32 {c1.Register}, {c2.Register}");

            return new RegisterLabelContext(nextRegister, null, new LatteParser.TIntContext());
        }

        public override RegisterLabelContext Visit(Or or)
        {
            var c1 = Visit(or.Lhs);
            var c2 = Visit(or.Rhs);

            var nextRegister = _state.NewRegister;
            _llvmGenerator.Emit($"{nextRegister} = or i1 {c1.Register}, {c2.Register}");

            return new RegisterLabelContext(nextRegister, null, new LatteParser.TBoolContext());
        }

        public override RegisterLabelContext Visit(RelOp relOp)
        {
            return base.Visit(relOp);
        }

        public override RegisterLabelContext Visit(Str str)
        {
            return base.Visit(str);
        }

        public override RegisterLabelContext Visit(UnOp unOp)
        {
            var c = Visit(unOp.Expr);

            var nextRegister = _state.NewRegister;
            switch (unOp.Operator)
            {
                case Unary.Minus:
                    _llvmGenerator.Emit($"{nextRegister} = mul i32 {c.Register}, -1");
                    break;
                case Unary.Neg:
                    _llvmGenerator.Emit($"{nextRegister} = xor i1 {c.Register}, 1");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new RegisterLabelContext(nextRegister, null, new LatteParser.TBoolContext());
        }
    }
}