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
            return base.VisitETrue(context);
        }

        public override LatteParser.TypeContext VisitEOr(LatteParser.EOrContext context)
        {
            return base.VisitEOr(context);
        }

        public override LatteParser.TypeContext VisitEInt(LatteParser.EIntContext context)
        {
            return base.VisitEInt(context);
        }

        public override LatteParser.TypeContext VisitEUnOp(LatteParser.EUnOpContext context)
        {
            return base.VisitEUnOp(context);
        }

        public override LatteParser.TypeContext VisitEStr(LatteParser.EStrContext context)
        {
            return base.VisitEStr(context);
        }

        public override LatteParser.TypeContext VisitEMethodCall(LatteParser.EMethodCallContext context)
        {
            return base.VisitEMethodCall(context);
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
            return base.VisitEObjectField(context);
        }

        public override LatteParser.TypeContext VisitEParen(LatteParser.EParenContext context)
        {
            return base.VisitEParen(context);
        }

        public override LatteParser.TypeContext VisitEFalse(LatteParser.EFalseContext context)
        {
            return base.VisitEFalse(context);
        }

        public override LatteParser.TypeContext VisitEAddOp(LatteParser.EAddOpContext context)
        {
            return base.VisitEAddOp(context);
        }

        public override LatteParser.TypeContext VisitENull(LatteParser.ENullContext context)
        {
            return base.VisitENull(context);
        }

        public override LatteParser.TypeContext VisitENewObject(LatteParser.ENewObjectContext context)
        {
            return base.VisitENewObject(context);
        }

        private void ValidateFunctionCall(LatteParser.FunctionDefContext fDef, LatteParser.EFunCallContext fCall)
        {
            var id = fCall.ID().GetText();

            if (fDef.arg() == null)
            {
                //if (fCall.expr())
            }
            Console.Write(fCall.expr().Length + " ");
            Console.WriteLine(fDef.arg().type().Length);
            
        }

        private void InterruptWithMessage(int line, string message)
        {
            _errorState.AddErrorMessage(new ErrorMessage(line, message));
            throw new InterruptedStaticCheckException();
        }
    }
}