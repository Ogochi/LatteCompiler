using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Common.AST;
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
            _environment.AddPredefinedFunctions();
            
            context.topDef().ToList().ForEach(topDef =>
            {
                var id = topDef switch
                {
                    LatteParser.FunctionDefContext fDef => fDef.ID().GetText(),
                    LatteParser.ClassDefContext cDef => cDef.ID()[0].GetText() 
                };
                
                if (_environment.NameToFunctionDef.ContainsKey(id) || _environment.NameToClassDef.ContainsKey(id))
                {
                    _errorState.AddErrorMessage(new ErrorMessage(
                        topDef.start.Line,
                        ErrorMessages.FuncOrClassAlreadyDefined(id)));
                }

                if (topDef is LatteParser.ClassDefContext classDef)
                {
                    CheckClassDef(classDef);
                }

                FrontendEnvironment.Instance.AddTopDef(topDef);
            });
            
            _environment.NameToClassDef.Values.ToList().ForEach(classDef =>
            {
                if (classDef.ParentId != null && !_environment.NameToClassDef.ContainsKey(classDef.ParentId))
                {
                    _errorState.AddErrorMessage(
                        new ErrorMessage(ErrorMessages.ParentNorDefinedException(classDef.Id, classDef.ParentId)));
                }
            });
        }

        private void CheckClassDef(LatteParser.ClassDefContext classDef)
        {
            var fieldMethod = new HashSet<string>();
                                
            classDef.fieldOrMethodDef().ToList().ForEach(fm =>
            {
                switch (fm) {
                    case LatteParser.ClassFieldDefContext field:
                        field.fieldDef().ID().ToList().ForEach(f =>
                        {
                            if (fieldMethod.Contains(f.GetText()))
                            {
                                _errorState.AddErrorMessage(
                                    new ErrorMessage(fm.start.Line,
                                        ErrorMessages.FieldOrMethodAlreadyDefined(f.GetText())));
                            }
                            else
                            {
                                fieldMethod.Add(field.GetText());
                            }
                        });
                        break;
                    
                    case LatteParser.ClassMethodDefContext method:
                        var methodId = method.methodDef().ID().GetText();
                        if (fieldMethod.Contains(methodId))
                        {
                            _errorState.AddErrorMessage(
                                new ErrorMessage(fm.start.Line,
                                    ErrorMessages.FieldOrMethodAlreadyDefined(methodId)));
                        }
                        else
                        {
                            fieldMethod.Add(method.methodDef().ID().GetText());
                        }
                        break;
                }
            });
        }

        public override void EnterSExp(LatteParser.SExpContext context)
        {
            new ExpressionTypeVisitor().Visit(context.expr());
        }

        public override void EnterClassDef(LatteParser.ClassDefContext context)
        {
            _environment.DetachVarEnv();
            _environment.DetachFuncEnv();
            _environment.CurrentClassName = context.ID()[0].GetText();

            foreach (var fm in context.fieldOrMethodDef())
            {
                switch (fm)
                {
                    case LatteParser.ClassFieldDefContext fields:
                        fields.fieldDef().ID().ToList().ForEach(field => 
                            _environment.NameToVarDef[field.GetText()] = new VarDef(fields.fieldDef().type(), field.GetText()));
                        break;
                    
                    case LatteParser.ClassMethodDefContext method:
                        _environment.NameToFunctionDef[method.methodDef().ID().GetText()] = 
                            new FunctionDef(method.methodDef());
                        break;
                }
            }
        }

        public override void ExitClassDef(LatteParser.ClassDefContext context)
        {
            _environment.RestorePreviousVarEnv();
            _environment.RestorePreviousFuncEnv();
            _environment.CurrentClassName = null;
        }

        public override void EnterMethodDef(LatteParser.MethodDefContext context)
        {
            EnterFunctionDef(new FunctionDef(context), context.start.Line, context.block(), context.arg());
        }

        public override void ExitMethodDef(LatteParser.MethodDefContext context)
        {
            _environment.RestorePreviousVarEnv();
        }

        public override void EnterFunctionDef(LatteParser.FunctionDefContext context)
        {
            EnterFunctionDef(new FunctionDef(context), context.start.Line, context.block(), context.arg());
        }

        private void EnterFunctionDef(FunctionDef context, int line, LatteParser.BlockContext block, LatteParser.ArgContext arg)
        {
            _environment.DetachVarEnv();
            _environment.CurrentFunctionName = context.Id;
            
            if (!context.Type.Equals(new LatteParser.TVoidContext()) &&
                !new ReturnsCheckVisitor().Visit(block))
            {
                _errorState.AddErrorMessage(new ErrorMessage(
                    line,
                    ErrorMessages.FunctionBranchWithoutRet(_environment.CurrentFunctionName)));
            }

            if (arg == null)
            {
                return;
            }

            var args = arg.type().Zip(arg.ID(), (a, b) => (a, b));
            foreach (var (type, id) in args)
            {
                var ident = id.GetText();
                
                if (_environment.NameToVarDef.ContainsKey(ident))
                {
                    _errorState.AddErrorMessage(new ErrorMessage(
                        line,
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

            if (!variable.Type.Equals(exprType) && !IsTypeParent(exprType, variable.Type))
            {
                StateUtils.InterruptWithMessage(
                    context.start.Line,
                    context.ID().Symbol.Column,
                    ErrorMessages.VarExprTypesMismatch(id));
            }
        }

        public override void EnterStructAss(LatteParser.StructAssContext context)
        {
            var expressionVisitor = new ExpressionTypeVisitor();
            var objectType = expressionVisitor.GetFieldType(context.expr()[0], context.ID().GetText(), context.start.Line);
            var exprType = new ExpressionTypeVisitor().Visit(context.expr()[1]);

            if (!objectType.Equals(exprType) && !IsTypeParent(exprType, objectType))
            {
                StateUtils.InterruptWithMessage(
                    context.start.Line,
                    context.ID().Symbol.Column,
                    ErrorMessages.FieldExprTypesMismatch(objectType.GetText(), context.ID().GetText()));
            }
        }

        public override void EnterStructDecr(LatteParser.StructDecrContext context)
        {
            var objectType = new ExpressionTypeVisitor().GetFieldType(context.expr(), context.ID().GetText(), context.start.Line);

            if (!objectType.Equals(new LatteParser.TIntContext()))
            {
                StateUtils.InterruptWithMessage(
                    context.start.Line,
                    context.ID().Symbol.Column,
                    ErrorMessages.DecrFieldOnlyOnInt(objectType.GetText(), context.ID().GetText()));
            }
        } 

        public override void EnterStructIncr(LatteParser.StructIncrContext context)
        {
            var objectType = new ExpressionTypeVisitor().GetFieldType(context.expr(), context.ID().GetText(), context.start.Line);

            if (!objectType.Equals(new LatteParser.TIntContext()))
            {
                StateUtils.InterruptWithMessage(
                    context.start.Line,
                    context.ID().Symbol.Column,
                    ErrorMessages.DecrFieldOnlyOnInt(objectType.GetText(), context.ID().GetText()));
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
                        exprType.ToString(),
                        func.Type.ToString(),
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
                        new LatteParser.TVoidContext().ToString(),
                        func.Type.ToString(),
                        func.Id)));
            }
        }

        public override void EnterDecl(LatteParser.DeclContext context)
        {
            if (context.type().Equals(new LatteParser.TVoidContext()))
            {
                StateUtils.InterruptWithMessage(
                    context.start.Line,
                    context.start.Column,
                    ErrorMessages.VoidDeclaration);
            }
            
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
                if (!context.type().Equals(exprType) && !IsTypeParent(exprType, context.type()))
                {
                    StateUtils.InterruptWithMessage(
                        decl.start.Line,
                        decl.start.Column,
                        ErrorMessages.VarExprTypesMismatch(decl.ID().GetText()));
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

        private bool IsTypeParent(LatteParser.TypeContext type, LatteParser.TypeContext parentToCheck)
        {
            if (!(type is LatteParser.TTypeNameContext))
            {
                return false;
            }
            
            var classDef = _environment.NameToClassDef[type.GetText()];
            if (classDef.ParentId == null)
            {
                return false;
            }

            return classDef.ParentId == parentToCheck.GetText() ||
                   IsTypeParent(new LatteParser.TTypeNameContext(classDef.ParentId), parentToCheck);
        }
    }
}