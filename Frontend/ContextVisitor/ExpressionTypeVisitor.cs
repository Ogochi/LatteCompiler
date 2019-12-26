using System;
using Common.StateManagement;
using Frontend.Exception;
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
                InterruptWithMessage(context.start.Line, ErrorMessages.VarNotDefined(id));
            }

            return _environment.NameToVarDef[id].Type;
        }

        public override LatteParser.TypeContext VisitEFunCall(LatteParser.EFunCallContext context)
        {
            var id = context.ID().GetText();
            if (!_environment.NameToFunctionDef.ContainsKey(id))
            {
                InterruptWithMessage(context.start.Line, ErrorMessages.FuncNotDefined(id));
            }

            var function = _environment.NameToFunctionDef[id];
            ValidateFunctionCall(function, context);

            return function.type();
        }

        public override LatteParser.TypeContext VisitERelOp(LatteParser.ERelOpContext context)
        {
            return base.VisitERelOp(context);
        }

        public override LatteParser.TypeContext VisitETrue(LatteParser.ETrueContext context)
        {
            return new TBoolContext();
        }

        public override LatteParser.TypeContext VisitEOr(LatteParser.EOrContext context)
        {
            return base.VisitEOr(context);
        }

        public override LatteParser.TypeContext VisitEInt(LatteParser.EIntContext context)
        {
            return new TIntContext();
        }

        public override LatteParser.TypeContext VisitEUnOp(LatteParser.EUnOpContext context)
        {
            return base.VisitEUnOp(context);
        }

        public override LatteParser.TypeContext VisitEStr(LatteParser.EStrContext context)
        {
            return new TStringContext();
        }

        public override LatteParser.TypeContext VisitEMethodCall(LatteParser.EMethodCallContext context)
        {
            throw new NotImplementedException();
        }

        public override LatteParser.TypeContext VisitEMulOp(LatteParser.EMulOpContext context)
        {
            return base.VisitEMulOp(context);
        }

        public override LatteParser.TypeContext VisitEAnd(LatteParser.EAndContext context)
        {
            return base.VisitEAnd(context);
        }

        public override LatteParser.TypeContext VisitEObjectField(LatteParser.EObjectFieldContext context)
        {
            throw new NotImplementedException();
        }

        public override LatteParser.TypeContext VisitEFalse(LatteParser.EFalseContext context)
        {
            return new TBoolContext();
        }

        public override LatteParser.TypeContext VisitEAddOp(LatteParser.EAddOpContext context)
        {
            return base.VisitEAddOp(context);
        }

        public override LatteParser.TypeContext VisitENull(LatteParser.ENullContext context)
        {
            throw new NotImplementedException();
        }

        public override LatteParser.TypeContext VisitENewObject(LatteParser.ENewObjectContext context)
        {
            throw new NotImplementedException();
        }

        private void ValidateFunctionCall(LatteParser.FunctionDefContext fDef, LatteParser.EFunCallContext fCall)
        {
            var id = fCall.ID().GetText();

            if ((fDef.arg()?.type().Length ?? 0) != (fCall?.expr().Length ?? 0))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    fCall.start.Line,
                    ErrorMessages.WrongArgsCountFuncCall(id)));
            }

            if (fDef.arg() == null)
            {
                return;
            }

            CheckFunctionCallArgsForEqualCount(fDef, fCall);
        }

        private void CheckFunctionCallArgsForEqualCount(
            LatteParser.FunctionDefContext fDef,
            LatteParser.EFunCallContext fCall)
        {
            var id = fCall.ID().GetText();
            
            for (int i = 0; i < fCall.expr().Length; i++)
            {
                var paramType = fDef.arg().type()[i];
                var paramId = fDef.arg().ID()[i];
                var argType = VisitExpr(fCall.expr()[i]);

                if (paramType != argType)
                {
                    _errorState.AddErrorMessage(new ErrorMessage(
                        fCall.start.Line,
                        argType.start.Column,
                        ErrorMessages.WrongArgTypeFuncCall(id, paramId.GetText(), paramType.GetText())));
                }
            }
        }

        private void InterruptWithMessage(int line, string message)
        {
            _errorState.AddErrorMessage(new ErrorMessage(line, message));
            throw new InterruptedStaticCheckException();
        }
    }
}