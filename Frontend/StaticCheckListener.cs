using System;
using System.Linq;
using System.Net.NetworkInformation;
using Frontend.StateManagement;
using Common.StateManagement;
using Frontend.ContextVisitor;
using Frontend.Utils;
using ParsingTools;

namespace Frontend
{
    public class StaticCheckListener : LatteBaseListener
    {
        private readonly FrontendEnvironment _environment = FrontendEnvironment.Instance;
        private readonly ErrorState _errorState = ErrorState.Instance;
        private bool _skipNextDecl;

        public override void EnterProgram(LatteParser.ProgramContext context)
        {
            context.topDef().ToList().ForEach(topDef =>
            {
                switch (topDef)
                {
                    case LatteParser.FunctionDefContext fDef:
                        var id = fDef.ID().GetText();
                        
                        if (_environment.NameToFunctionDef.ContainsKey(id))
                        {
                            _errorState.AddErrorMessage(new ErrorMessage(
                                topDef.start.Line,
                                ErrorMessages.FuncAlreadyDefined(id)));
                        }
                        break;
                    case LatteParser.ClassDefContext cDef:
                        throw new NotImplementedException();
                }

                FrontendEnvironment.Instance.AddTopDef(topDef);
            });
        }

        public override void EnterFunctionDef(LatteParser.FunctionDefContext context)
        {
            _environment.DetachVarEnv();
            _environment.CurrentFunctionName = context.ID().GetText();
            
            if (!context.type().Equals(new LatteParser.TVoidContext()) &&
                !new ReturnsCheckVisitor().Visit(context.block()))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    context.start.Line,
                    ErrorMessages.FunctionBranchWithoutRet(_environment.CurrentFunctionName)));
            }

            if (context.arg() == null)
            {
                return;
            }

            var args = context.arg().type().Zip(context.arg().ID(), (a, b) => (a, b));
            foreach (var (type, id) in args)
            {
                var ident = id.GetText();
                
                if (_environment.NameToVarDef.ContainsKey(ident))
                {
                    _errorState.AddErrorMessage(new ErrorMessage(
                        context.start.Line,
                        ErrorMessages.VarAlreadyDefined(ident)));
                }
                
                _environment.NameToVarDef[ident] = new VarDef(type, ident);
            }
        }

        public override void ExitFunctionDef(LatteParser.FunctionDefContext context)
        {
            _environment.RestorePreviousVarEnv();
        }

        public override void EnterBlockStmt(LatteParser.BlockStmtContext context)
        {
            _environment.DetachVarEnv();
        }

        public override void ExitBlockStmt(LatteParser.BlockStmtContext context)
        {
           _environment.RestorePreviousVarEnv();
        }

        public override void EnterAss(LatteParser.AssContext context)
        {
            var id = context.ID().GetText();

            if (!_environment.NameToVarDef.ContainsKey(id))
            {
                StateUtils.InterruptWithMessage(
                    context.start.Line,
                    context.ID().Symbol.Column,
                    ErrorMessages.VarNotDefined(id));
            }

            var variable = _environment.NameToVarDef[id];
            var exprType = new ExpressionTypeVisitor().Visit(context.expr());

            if (!variable.Type.Equals(exprType))
            {
                StateUtils.InterruptWithMessage(
                    context.start.Line,
                    context.ID().Symbol.Column,
                    ErrorMessages.VarExprTypesMismatch(id));
            }
        }

        public override void EnterRet(LatteParser.RetContext context)
        {
            var func = _environment.CurrentFunction;
            var exprType = new ExpressionTypeVisitor().Visit(context.expr());
            
            if (!func.Type.Equals(exprType))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    context.start.Line,
                    context.start.Column,
                    ErrorMessages.WrongReturn(
                        exprType.GetType().ToString(),
                        func.Type.GetType().ToString(),
                        func.Id)));
            }
        }

        public override void EnterCond(LatteParser.CondContext context)
        {
            _environment.DetachVarEnv();

            var exprType = new ExpressionTypeVisitor().Visit(context.expr());
            if (!exprType.Equals(new LatteParser.TBoolContext()))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    context.expr().start.Line,
                    context.expr().start.Line,
                    ErrorMessages.IfWrongCondition));
            }
        }

        public override void ExitCond(LatteParser.CondContext context)
        {
            _environment.RestorePreviousVarEnv();
        }

        public override void EnterCondElse(LatteParser.CondElseContext context)
        {
            _environment.DetachVarEnv();
            if (context.stmt()[0] is LatteParser.DeclContext)
            {
                _skipNextDecl = true;
            }

            var exprType = new ExpressionTypeVisitor().Visit(context.expr());
            if (!exprType.Equals(new LatteParser.TBoolContext()))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    context.expr().start.Line,
                    context.expr().start.Line,
                    ErrorMessages.IfWrongCondition));
            }
        }
        
        public override void ExitCondElse(LatteParser.CondElseContext context)
        {
            _environment.RestorePreviousVarEnv();
        }

        public override void EnterVRet(LatteParser.VRetContext context)
        {
            var func = _environment.CurrentFunction;
            if (!func.Type.Equals(new LatteParser.TVoidContext()))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    context.start.Line,
                    context.start.Column,
                    ErrorMessages.WrongReturn(
                        new LatteParser.TVoidContext().GetType().ToString(),
                        func.Type.GetType().ToString(),
                        func.Id)));
            }
        }

        public override void EnterDecl(LatteParser.DeclContext context)
        {
            foreach (var decl in context.item())
            {
                var id = decl.ID().GetText();
                if (_environment.NameToVarDef.ContainsKey(id) && 
                    _environment.NameToVarDef[id].IsDefinedInCurrentBlock)
                {
                    _errorState.AddErrorMessage(new ErrorMessage(
                        decl.start.Line,
                        decl.start.Column,
                        ErrorMessages.VarAlreadyDefined(id)));
                }

                if (_skipNextDecl)
                {
                    _skipNextDecl = false;
                }
                else
                {
                    _environment.NameToVarDef[id] = new VarDef(context.type(), id);
                }

                if (decl.expr() == null) continue;
                
                var exprType = new ExpressionTypeVisitor().Visit(decl.expr());
                if (!context.type().Equals(exprType))
                {
                    StateUtils.InterruptWithMessage(
                        decl.start.Line,
                        decl.start.Column,
                        ErrorMessages.VarExprTypesMismatch(exprType.GetType() + ", " + context.type().GetType()));
                }
            }
        }

        public override void EnterWhile(LatteParser.WhileContext context)
        {
            _environment.DetachVarEnv();

            var exprType = new ExpressionTypeVisitor().Visit(context.expr());
            if (!exprType.Equals(new LatteParser.TBoolContext()))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    context.expr().start.Line,
                    context.expr().start.Line,
                    ErrorMessages.WhileWrongCondition));
            }
        }
        
        public override void ExitWhile(LatteParser.WhileContext context)
        {
            _environment.RestorePreviousVarEnv();
        }
        

        public override void EnterDecr(LatteParser.DecrContext context)
        {
            var id = context.ID().GetText();
            if (!_environment.NameToVarDef.ContainsKey(id))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    context.start.Line,
                    context.start.Column,
                    ErrorMessages.VarNotDefined(id)));
            }
            
            if (!_environment.NameToVarDef[id].Type.Equals(new LatteParser.TIntContext()))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    context.start.Line,
                    context.start.Column,
                    ErrorMessages.DecrOnlyOnInt(id)));
            }
        }

        public override void EnterIncr(LatteParser.IncrContext context)
        {
            var id = context.ID().GetText();
            if (!_environment.NameToVarDef.ContainsKey(id))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    context.start.Line,
                    context.start.Column,
                    ErrorMessages.VarNotDefined(id)));
            }
            
            if (!_environment.NameToVarDef[id].Type.Equals(new LatteParser.TIntContext()))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    context.start.Line,
                    context.start.Column,
                    ErrorMessages.IncrOnlyOnInt(id)));
            }
        }
    }
}