using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override RegisterLabelContext Visit(Null @null)
        {
            return new RegisterLabelContext("null", _state.CurrentLabel, new LatteParser.TVoidContext());
        }

        public override RegisterLabelContext Visit(ObjectField objectField)
        {
            string nextRegister1 = _state.NewRegister, nextRegister2 = _state.NewRegister;
            var objectExpr = Visit(objectField.IdExpr);
            var field = _globalState.NameToClass[objectExpr.Type.GetText()].Fields[objectField.FieldId];

            _llvmGenerator.Emit($"{nextRegister1} = getelementptr %{objectExpr.Type.GetText()}, " +
                                $"%{objectExpr.Type.GetText()}* {objectExpr.Register}, i32 0, i32 {field.Number}");
            
            _llvmGenerator.Emit($"{nextRegister2} = load {AstToLlvmString.Type(field.Type)}, " +
                                $"{AstToLlvmString.Type(field.Type)}* {nextRegister1}");
            
            return new RegisterLabelContext(nextRegister2, _state.CurrentLabel, field.Type);
        }

        public override RegisterLabelContext Visit(NewObject newObject)
        {
            var nextRegister = _state.NewRegister;
            _llvmGenerator.Emit($"{nextRegister} = alloca %{newObject.Type.GetText()}");
            _llvmGenerator.Emit(
                $"call void @g_{newObject.Type.GetText()}_construct(%{newObject.Type.GetText()}* {nextRegister})");
            
            return new RegisterLabelContext(nextRegister, _state.CurrentLabel, newObject.Type);
        }

        public override RegisterLabelContext Visit(AddOp addOp)
        {
            var c1 = Visit(addOp.Lhs);
            var c2 = Visit(addOp.Rhs);
            var nextRegister = _state.NewRegister;
            
            switch (addOp.Add, c1.Type, c2.Type)
            {
                case (Add.Plus, LatteParser.TIntContext _, LatteParser.TIntContext _):
                    _llvmGenerator.Emit($"{nextRegister} = add i32 {c1.Register}, {c2.Register}");
                    return new RegisterLabelContext(nextRegister, _state.CurrentLabel, new LatteParser.TIntContext());
                
                case (Add.Minus, LatteParser.TIntContext _, LatteParser.TIntContext _):
                    _llvmGenerator.Emit($"{nextRegister} = sub i32 {c1.Register}, {c2.Register}");
                    return new RegisterLabelContext(nextRegister, _state.CurrentLabel, new LatteParser.TIntContext());
                
                case (Add.Plus, LatteParser.TStringContext _, LatteParser.TStringContext _):
                    _llvmGenerator.Emit($"{nextRegister} = call i8* @strConcat(i8* {c1.Register}, i8* {c2.Register})");
                    return new RegisterLabelContext(nextRegister, _state.CurrentLabel, new LatteParser.TStringContext());
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override RegisterLabelContext Visit(And and)
        {
            _state.ConsolidateVariables();
            string label1 = _state.NewLabel, label2 = _state.NewLabel;

            var c1 = Visit(and.Lhs);
            var firstResultLabel = _state.CurrentLabel;
            _llvmGenerator.Emit($"br i1 {c1.Register}, label %{label1}, label %{label2}");
            
            _state.CurrentLabel = label1;
            _llvmGenerator.Emit($"{label1}:");
            var c2 = Visit(and.Rhs);
            var label1ResultLabel = _state.CurrentLabel;
            _llvmGenerator.Emit($"br label %{label2}");

            _state.CurrentLabel = label2;
            _llvmGenerator.Emit($"{label2}:");
            
            _state.ConsolidateVariables();
            var nextRegister = _state.NewRegister;
            _llvmGenerator.Emit($"{nextRegister} = phi i1 [0, %{firstResultLabel}], [{c2.Register}, %{label1ResultLabel}]");

            return new RegisterLabelContext(nextRegister, _state.CurrentLabel, new LatteParser.TBoolContext());
        }

        public override RegisterLabelContext Visit(Bool @bool)
        {
            return new RegisterLabelContext(@bool.Value ? "1" : "0", _state.CurrentLabel, new LatteParser.TBoolContext());
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
            return new RegisterLabelContext(nextRegister, _state.CurrentLabel, function.Type);
        }

        public RegisterLabelContext VisitID(ID id, string registerOverride = null)
        {
            var values = _state.VarToLabelToRegister[id.Id].Values.ToList();

            if (values.Count == 1)
            {
                return new RegisterLabelContext(values[0].Register, _state.CurrentLabel, values[0].Type);
            }

            var phi = new StringBuilder();
            var isFirst = true;
            foreach (var value in values)
            {
                if (!isFirst)
                {
                    phi.Append(", ");
                }
                isFirst = false;

                phi.Append($"[{value.Register}, %{value.Label}]");
            }
            
            var nextRegister = registerOverride ?? _state.NewRegister;
            _llvmGenerator.Emit($"{nextRegister} = phi {AstToLlvmString.Type(values[0].Type)} {phi}");
            
            return new RegisterLabelContext(nextRegister, _state.CurrentLabel, values[0].Type);
        }

        public override RegisterLabelContext Visit(ID id)
        {
            return VisitID(id);
        }

        public override RegisterLabelContext Visit(Int @int)
        {
            return new RegisterLabelContext(@int.Value.ToString(), _state.CurrentLabel, new LatteParser.TIntContext());
        }

        public override RegisterLabelContext Visit(MulOp mulOp)
        {
            var c1 = Visit(mulOp.Lhs);
            var c2 = Visit(mulOp.Rhs);

            var nextRegister = _state.NewRegister;
            _llvmGenerator.Emit($"{nextRegister} = {AstToLlvmString.Mul(mulOp.Mul)} i32 {c1.Register}, {c2.Register}");

            return new RegisterLabelContext(nextRegister, _state.CurrentLabel, new LatteParser.TIntContext());
        }

        public override RegisterLabelContext Visit(Or or)
        {
            _state.ConsolidateVariables();
            string label2 = _state.NewLabel, label3 = _state.NewLabel;

            var c1 = Visit(or.Lhs);
            var label1ResultLabel = _state.CurrentLabel;
            _llvmGenerator.Emit($"br i1 {c1.Register}, label %{label3}, label %{label2}");
            
            _state.CurrentLabel = label2;
            _llvmGenerator.Emit($"{label2}:");
            var c2 = Visit(or.Rhs);
            var label2ResultLabel = _state.CurrentLabel;
            _llvmGenerator.Emit($"br label %{label3}");

            _state.CurrentLabel = label3;
            _llvmGenerator.Emit($"{label3}:");
            
            _state.ConsolidateVariables();
            var nextRegister = _state.NewRegister;
            _llvmGenerator.Emit($"{nextRegister} = phi i1 [1, %{label1ResultLabel}], [{c2.Register}, %{label2ResultLabel}]");

            return new RegisterLabelContext(nextRegister, _state.CurrentLabel, new LatteParser.TBoolContext());
        }

        public override RegisterLabelContext Visit(RelOp relOp)
        {
            var c1 = Visit(relOp.Lhs);
            var c2 = Visit(relOp.Rhs);
            var resultRegister = _state.NewRegister;
            
            switch (relOp.Rel, c1.Type, c2.Type)
            {
                case (Rel.Equals, LatteParser.TStringContext _, LatteParser.TStringContext _):
                    _llvmGenerator.Emit($"{resultRegister} = call i1 @strEq(i8* {c1.Register}, i8* {c2.Register})");
                    break;
                
                default:
                    var type = AstToLlvmString.Type(c1.Type);
                    var compOperation = AstToLlvmString.Rel(relOp.Rel);
                    _llvmGenerator.Emit(
                        $"{resultRegister} = icmp {compOperation} {type} {c1.Register}, {c2.Register}");
                    break;
            }
            
            return new RegisterLabelContext(resultRegister, _state.CurrentLabel, new LatteParser.TBoolContext());
        }

        public override RegisterLabelContext Visit(Str str)
        {
            if (!_globalState.LiteralToStringConstId.ContainsKey(str.Value))
            {
                _globalState.LiteralToStringConstId[str.Value] = _globalState.NewString;
            }

            var nextString = _globalState.LiteralToStringConstId[str.Value];
            var nextRegister = _state.NewRegister;
            var strLen = str.Value.Length + 1;
            _llvmGenerator.Emit(
                $"{nextRegister} = getelementptr [{strLen} x i8],[{strLen} x i8]* {nextString}, i64 0, i64 0");
            
            return new RegisterLabelContext(nextRegister, _state.CurrentLabel, new LatteParser.TStringContext());
        }

        public override RegisterLabelContext Visit(UnOp unOp)
        {
            var c = Visit(unOp.Expr);

            var nextRegister = _state.NewRegister;
            switch (unOp.Operator)
            {
                case Unary.Minus:
                    _llvmGenerator.Emit($"{nextRegister} = mul i32 {c.Register}, -1");
                    return new RegisterLabelContext(nextRegister, _state.CurrentLabel, new LatteParser.TIntContext());
                case Unary.Neg:
                    _llvmGenerator.Emit($"{nextRegister} = xor i1 {c.Register}, 1");
                    return new RegisterLabelContext(nextRegister, _state.CurrentLabel, new LatteParser.TBoolContext());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}