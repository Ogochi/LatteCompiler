using System;
using System.Linq;
using Frontend.StateManagement;
using Common.StateManagement;
using ParsingTools;

namespace Frontend
{
    public class StaticCheckListener : LatteBaseListener
    {
        private readonly FrontendEnvironment _environment = FrontendEnvironment.Instance;
        private readonly ErrorState _errorState;
        
        public override void EnterProgram(LatteParser.ProgramContext context)
        {
            context.topDef().ToList().ForEach(FrontendEnvironment.Instance.AddTopDef);
        }

        public override void EnterFunctionDef(LatteParser.FunctionDefContext context)
        {
            _environment.DetachVarEnv();
            _environment.CurrentFunction = context.ID().GetText();

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
                        ErrorMessages.VarAlreadyDefinedMsg(ident)));
                }
                
                _environment.NameToVarDef[ident] = new VarDef(type, ident);
            }
        }

        public override void ExitFunctionDef(LatteParser.FunctionDefContext context)
        {
            base.ExitFunctionDef(context);
        }

        public override void EnterBlockStmt(LatteParser.BlockStmtContext context)
        {
            base.EnterBlockStmt(context);
        }

        public override void ExitBlockStmt(LatteParser.BlockStmtContext context)
        {
            base.ExitBlockStmt(context);
        }

        public override void EnterAss(LatteParser.AssContext context)
        {
            base.EnterAss(context);
        }

        public override void EnterRet(LatteParser.RetContext context)
        {
            base.EnterRet(context);
        }

        public override void EnterCond(LatteParser.CondContext context)
        {
            base.EnterCond(context);
        }

        public override void EnterCondElse(LatteParser.CondElseContext context)
        {
            base.EnterCondElse(context);
        }

        public override void EnterVRet(LatteParser.VRetContext context)
        {
            base.EnterVRet(context);
        }

        public override void EnterDecl(LatteParser.DeclContext context)
        {
            base.EnterDecl(context);
        }

        public override void EnterWhile(LatteParser.WhileContext context)
        {
            base.EnterWhile(context);
        }

        public override void EnterSExp(LatteParser.SExpContext context)
        {
            base.EnterSExp(context);
        }

        public override void EnterDecr(LatteParser.DecrContext context)
        {
            base.EnterDecr(context);
        }

        public override void EnterIncr(LatteParser.IncrContext context)
        {
            base.EnterIncr(context);
        }
    }
}