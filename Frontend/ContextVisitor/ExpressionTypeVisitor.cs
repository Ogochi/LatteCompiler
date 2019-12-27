using System;
using Common.AST;
using Common.StateManagement;
using Frontend.StateManagement;
using ParsingTools;

namespace Frontend.ContextVisitor
{
    public class ExpressionTypeVisitor : LatteBaseVisitor<LatteParser.TypeContext>
    {
        private readonly FrontendEnvironment _environment = FrontendEnvironment.Instance;
        private readonly ErrorState _errorState = ErrorState.Instance;

        public override LatteParser.TypeContext VisitEId(LatteParser.EIdContext context)
        {
            var id = context.ID().GetText();
            if (!_environment.NameToVarDef.ContainsKey(id))
            {
                Utils.StateUtils.InterruptWithMessage(context.start.Line, ErrorMessages.VarNotDefined(id));
            }

            return _environment.NameToVarDef[id].Type;
        }

        public override LatteParser.TypeContext VisitEFunCall(LatteParser.EFunCallContext context)
        {
            var id = context.ID().GetText();
            if (!_environment.NameToFunctionDef.ContainsKey(id))
            {
                Utils.StateUtils.InterruptWithMessage(context.start.Line, ErrorMessages.FuncNotDefined(id));
            }

            var function = _environment.NameToFunctionDef[id];
            ValidateFunctionCall(function, context);

            return function.Type;
        }

        public override LatteParser.TypeContext VisitERelOp(LatteParser.ERelOpContext context)
        {
            var lhs = Visit(context.expr()[0]);
            var rhs = Visit(context.expr()[1]);

            switch (context.relOp())
            {
                case LatteParser.LessThanContext _:
                case LatteParser.LessEqualsContext _:
                case LatteParser.GreaterThanContext _:
                case LatteParser.GreaterEqualsContext _:
                    if (!lhs.Equals(rhs) || !lhs.Equals(new LatteParser.TIntContext()))
                    {
                        Utils.StateUtils.InterruptWithMessage(context.start.Line, ErrorMessages.CompOpToNotInt);
                    }
                    break;
                case LatteParser.EqualsContext _:
                case LatteParser.NotEqualsContext _:
                    if (!lhs.Equals(rhs))
                    {
                        Utils.StateUtils.InterruptWithMessage(context.start.Line, ErrorMessages.EqOpWrongTypes);
                    }
                    break;
            }

            return new LatteParser.TBoolContext();
        }

        public override LatteParser.TypeContext VisitETrue(LatteParser.ETrueContext context)
        {
            return new LatteParser.TBoolContext();
        }

        public override LatteParser.TypeContext VisitEOr(LatteParser.EOrContext context)
        {
            var lhs = Visit(context.expr()[0]);
            var rhs = Visit(context.expr()[1]);

            if (!lhs.Equals(rhs) || !lhs.Equals(new LatteParser.TBoolContext()))
            {
                Utils.StateUtils.InterruptWithMessage(context.start.Line, ErrorMessages.LogicOpToNotBool);
            }

            return new LatteParser.TBoolContext();
        }

        public override LatteParser.TypeContext VisitEInt(LatteParser.EIntContext context)
        {
            return new LatteParser.TIntContext();
        }

        public override LatteParser.TypeContext VisitEUnOp(LatteParser.EUnOpContext context)
        {
            var expr = Visit(context.expr());

            switch (context.unOp())
            {
                case LatteParser.UnaryMinusContext _:
                    if (!expr.Equals(new LatteParser.TIntContext()))
                    {
                        Utils.StateUtils.InterruptWithMessage(context.start.Line, ErrorMessages.UnaryMinusToNotInt);
                    }
                    return new LatteParser.TIntContext();
                case LatteParser.UnaryNegContext _:
                    if (!expr.Equals(new LatteParser.TBoolContext()))
                    {
                        Utils.StateUtils.InterruptWithMessage(context.start.Line, ErrorMessages.UnaryNegToNotBool);
                    }
                    return new LatteParser.TBoolContext();
                default:
                    throw new NotSupportedException();
            }
        }

        public override LatteParser.TypeContext VisitEStr(LatteParser.EStrContext context)
        {
            return new LatteParser.TStringContext();
        }

        public override LatteParser.TypeContext VisitEMethodCall(LatteParser.EMethodCallContext context)
        {
            throw new NotImplementedException();
        }

        public override LatteParser.TypeContext VisitEParen(LatteParser.EParenContext context)
        {
            return Visit(context.expr());
        }

        public override LatteParser.TypeContext VisitEMulOp(LatteParser.EMulOpContext context)
        {
            var lhs = Visit(context.expr()[0]);
            var rhs = Visit(context.expr()[1]);

            if (!lhs.Equals(rhs) || !lhs.Equals(new LatteParser.TIntContext()))
            {
                Utils.StateUtils.InterruptWithMessage(context.start.Line, ErrorMessages.MulOpToNotInt);
            }

            return new LatteParser.TIntContext();
        }

        public override LatteParser.TypeContext VisitEAnd(LatteParser.EAndContext context)
        {
            var lhs = Visit(context.expr()[0]);
            var rhs = Visit(context.expr()[1]);

            if (!lhs.Equals(rhs) || !lhs.Equals(new LatteParser.TBoolContext()))
            {
                Utils.StateUtils.InterruptWithMessage(context.start.Line, ErrorMessages.LogicOpToNotBool);
            }

            return new LatteParser.TBoolContext();
        }

        public override LatteParser.TypeContext VisitEObjectField(LatteParser.EObjectFieldContext context)
        {
            throw new NotImplementedException();
        }

        public override LatteParser.TypeContext VisitEFalse(LatteParser.EFalseContext context)
        {
            return new LatteParser.TBoolContext();
        }

        public override LatteParser.TypeContext VisitEAddOp(LatteParser.EAddOpContext context)
        {
            var lhs = Visit(context.expr()[0]);
            var rhs = Visit(context.expr()[1]);

            switch (context.addOp())
            {
                case LatteParser.PlusContext _:
                    if (!lhs.Equals(rhs) || 
                        (!lhs.Equals(new LatteParser.TIntContext()) && !lhs.Equals(new LatteParser.TStringContext())))
                    {
                        Utils.StateUtils.InterruptWithMessage(context.start.Line, ErrorMessages.AddOpWrongType);
                    }
                    return lhs;
                case LatteParser.MinusContext _:
                    if (!lhs.Equals(rhs) || !lhs.Equals(new LatteParser.TIntContext()))
                    {
                        Utils.StateUtils.InterruptWithMessage(context.start.Line, ErrorMessages.MinusOpToNotInt);
                    }
                    return new LatteParser.TIntContext();
                default:
                    throw new NotSupportedException();
            }
        }

        public override LatteParser.TypeContext VisitENull(LatteParser.ENullContext context)
        {
            throw new NotImplementedException();
        }

        public override LatteParser.TypeContext VisitENewObject(LatteParser.ENewObjectContext context)
        {
            throw new NotImplementedException();
        }

        private void ValidateFunctionCall(FunctionDef fDef, LatteParser.EFunCallContext fCall)
        {
            var id = fCall.ID().GetText();

            if (fDef.Args.Count  != (fCall?.expr().Length ?? 0))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    fCall.start.Line,
                    ErrorMessages.WrongArgsCountFuncCall(id)));
            }

            if (fDef.Args.Count == 0)
            {
                return;
            }

            CheckFunctionCallArgsForEqualCount(fDef, fCall);
        }

        private void CheckFunctionCallArgsForEqualCount(FunctionDef fDef, LatteParser.EFunCallContext fCall)
        {
            var id = fCall.ID().GetText();
            
            for (int i = 0; i < fCall.expr().Length; i++)
            {
                
                var paramType = fDef.Args[i].Type;
                var paramId = fDef.Args[i].Id;
                var argType = Visit(fCall.expr()[i]);

                if (!paramType.Equals(argType))
                {
                    _errorState.AddErrorMessage(new ErrorMessage(
                        fCall.start.Line,
                        fCall.expr()[i].start.Column,
                        ErrorMessages.WrongArgTypeFuncCall(id, paramId, paramType.GetText())));
                }
            }
        }
    }
}