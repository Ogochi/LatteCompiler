using System.Linq;
using System.Text;
using Common.AST;
using Common.AST.Exprs;
using LlvmGenerator.StateManagement;
using LlvmGenerator.Utils;
using ParsingTools;

namespace LlvmGenerator.Generators
{
    public class ExpressionGeneratorVisitor : BaseExprAstVisitor<(LatteParser.TypeContext, RegisterLabelContext)>
    {
        private LlvmGenerator _llvmGenerator = LlvmGenerator.Instance;
        private FunctionGeneratorState _state;
        private FunctionsGlobalState _globalState = FunctionsGlobalState.Instance;

        public ExpressionGeneratorVisitor(FunctionGeneratorState state)
        {
            _state = state;
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(AddOp addOp)
        {
            return base.Visit(addOp);
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(And and)
        {
            var (_, c1) = Visit(and.Lhs);
            var (_, c2) = Visit(and.Rhs);

            string nextRegister = _state.NewRegister;
            _llvmGenerator.Emit($"{nextRegister} = and i1 {c1.Register}, {c2.Register}");

            return (new LatteParser.TBoolContext(), new RegisterLabelContext(nextRegister, null));
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(Bool @bool)
        {
            return (new LatteParser.TBoolContext(), new RegisterLabelContext(@bool.Value ? "1" : "0", null));
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(FunCall funCall)
        {
            FunctionDef function = _globalState.NameToFunction[AstToLlvmString.FunctionName(funCall.Id)];
            
            StringBuilder toEmit = new StringBuilder();
            string nextRegister = "";
            if (!(function.Type is LatteParser.TVoidContext))
            {
                nextRegister = _state.NewRegister;
                toEmit = new StringBuilder($"{nextRegister} = ");
            }

            toEmit.Append($"call {AstToLlvmString.Type(function.Type)} @{function.Id}(");

            bool isFirstArg = true;
            foreach (var expr in funCall.Exprs)
            {
                if (!isFirstArg)
                {
                    toEmit.Append(", ");
                }

                var (exprType, exprResult) = Visit(expr);
                toEmit.Append($"{AstToLlvmString.Type(exprType)} {exprResult.Register}");
                
                isFirstArg = false;
            }
            
            toEmit.Append(")");
            _llvmGenerator.Emit(toEmit.ToString());
            return (function.Type, new RegisterLabelContext(nextRegister, null));
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(ID id)
        {
            return base.Visit(id);
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(Int @int)
        {
            return (new LatteParser.TIntContext(), new RegisterLabelContext(@int.Value.ToString(), null));
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(MulOp mulOp)
        {
            var (_, c1) = Visit(mulOp.Lhs);
            var (_, c2) = Visit(mulOp.Rhs);

            string nextRegister = _state.NewRegister;
            _llvmGenerator.Emit($"{nextRegister} = {AstToLlvmString.Mul(mulOp.Mul)} i32 {c1.Register}, {c2.Register}");

            return (new LatteParser.TIntContext(), new RegisterLabelContext(nextRegister, null));
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(Or or)
        {
            var (_, c1) = Visit(or.Lhs);
            var (_, c2) = Visit(or.Rhs);

            string nextRegister = _state.NewRegister;
            _llvmGenerator.Emit($"{nextRegister} = or i1 {c1.Register}, {c2.Register}");

            return (new LatteParser.TBoolContext(), new RegisterLabelContext(nextRegister, null));
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(RelOp relOp)
        {
            return base.Visit(relOp);
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(Str str)
        {
            return base.Visit(str);
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(UnOp unOp)
        {
            return base.Visit(unOp);
        }
    }
}