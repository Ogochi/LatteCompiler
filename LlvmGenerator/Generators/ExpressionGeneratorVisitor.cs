using Common.AST;
using Common.AST.Exprs;
using LlvmGenerator.StateManagement;
using ParsingTools;

namespace LlvmGenerator.Generators
{
    public class ExpressionGeneratorVisitor : BaseExprAstVisitor<(LatteParser.TypeContext, RegisterLabelContext)>
    {
        private FunctionGeneratorState _state;

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
            return base.Visit(and);
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(Bool @bool)
        {
            return base.Visit(@bool);
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(FunCall funCall)
        {
            return base.Visit(funCall);
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(ID id)
        {
            return base.Visit(id);
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(Int @int)
        {
            return base.Visit(@int);
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(MulOp mulOp)
        {
            return base.Visit(mulOp);
        }

        public override (LatteParser.TypeContext, RegisterLabelContext) Visit(Or or)
        {
            return base.Visit(or);
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