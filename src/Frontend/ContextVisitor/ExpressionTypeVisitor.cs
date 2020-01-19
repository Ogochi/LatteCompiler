using System;
using System.Linq;
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
        
        public override LatteParser.TypeContext VisitENullCast(LatteParser.ENullCastContext context)
        {
            return context.type();
        }

        public override LatteParser.TypeContext VisitEId(LatteParser.EIdContext context)
        {
            var id = context.ID().GetText();

            if (id == "self")
            {
                if (_environment.CurrentClassName == null)
                {
                    Utils.StateUtils.InterruptWithMessage(context.start.Line, ErrorMessages.SelfOnlyInClassException);
                }
                
                return new LatteParser.TTypeNameContext(_environment.CurrentClassName);
            }
            
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
            ValidateFunctionCall(function, function.Id, context.expr(), context.start.Line);

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
            var exprType = Visit(context.expr()[0]);
            var classDef = _environment.NameToClassDef[exprType.GetText()];
            var methodId = context.ID().GetText();
            
            if (!ClassHasMethod(classDef, methodId, out var method))
            {
                Utils.StateUtils.InterruptWithMessage(
                    context.start.Line, 
                    ErrorMessages.MethodNotDefined(classDef.Id, methodId));
            }
            
            ValidateFunctionCall(method, methodId, context.expr().ToList().Skip(1).ToArray(), context.start.Line);
            return method.Type;
        }

        private bool ClassHasMethod(ClassDef classDef, string methodId, out FunctionDef method)
        {
            if (classDef.Methods.ContainsKey(methodId))
            {
                method = classDef.Methods[methodId];
                return true;
            }
            if (classDef.ParentId != null)
            {
                return ClassHasMethod(_environment.NameToClassDef[classDef.ParentId], methodId, out method);
            }

            method = null;
            return false;
        }
        
        private bool ClassHasField(ClassDef classDef, string fieldId, out Field field)
        {
            if (classDef.Fields.ContainsKey(fieldId))
            {
                field = classDef.Fields[fieldId];
                return true;
            }
            if (classDef.ParentId != null)
            {
                return ClassHasField(_environment.NameToClassDef[classDef.ParentId], fieldId, out field);
            }

            field = null;
            return false;
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
            return GetFieldType(context.expr(), context.ID().GetText(), context.start.Line);
        }

        public LatteParser.TypeContext GetFieldType(LatteParser.ExprContext objectExpr, string fieldId, int line)
        {
            var exprType = Visit(objectExpr);
            var classDef = _environment.NameToClassDef[exprType.GetText()];

            if (!ClassHasField(classDef, fieldId, out var field))
            {
                Utils.StateUtils.InterruptWithMessage(
                    line, 
                    ErrorMessages.ClassFieldNotExist(exprType.GetText(), fieldId));
            }

            return field.Type;
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
            return new LatteParser.TVoidContext();
        }

        public override LatteParser.TypeContext VisitENewObject(LatteParser.ENewObjectContext context)
        {
            switch (context.type())
            {
                case LatteParser.TTypeNameContext typeContext:
                    if (!_environment.NameToClassDef.ContainsKey(typeContext.GetText()))
                    {
                        _errorState.AddErrorMessage(new ErrorMessage(
                            context.start.Line,
                            ErrorMessages.ClassNotDefinedException(typeContext.GetText())));
                    }
                    break;
                
                default:
                    _errorState.AddErrorMessage(new ErrorMessage(
                        context.start.Line,
                        ErrorMessages.IncorrectNewException));
                    break;
            }
            
            return context.type();
        }

        public void ValidateFunctionCall(FunctionDef fDef, string functionId, LatteParser.ExprContext[] expr, int line)
        {
            if (fDef.Args.Count  != (expr?.Length ?? 0))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    line,
                    ErrorMessages.WrongArgsCountFuncCall(functionId)));
            }

            CheckFunctionCallArgsForEqualCount(fDef, expr, functionId, line);
        }

        private void CheckFunctionCallArgsForEqualCount(FunctionDef fDef, LatteParser.ExprContext[] fExpr, string id, int line)
        {
            var argsWithExpr = fDef.Args.Zip(fExpr, (arg, expr) => (arg, expr));
            foreach (var (arg, expr) in argsWithExpr)
            {
                if (!Visit(expr).Equals(arg.Type))
                {
                    _errorState.AddErrorMessage(new ErrorMessage(
                        line,
                        expr.start.Column,
                        ErrorMessages.WrongArgTypeFuncCall(id, arg.Id, arg.Type.GetText())));
                }
            }
        }
    }
}