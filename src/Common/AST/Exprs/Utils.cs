using System;
using ParsingTools;

namespace Common.AST.Exprs
{
    public static class Utils
    {
        public static Expr ExprFromExprContext(LatteParser.ExprContext context)
        {
            if (context == null)
            {
                return null;
            }
            
            return context switch
            {
                LatteParser.EAddOpContext eAddOpContext => new AddOp(eAddOpContext),
                LatteParser.EAndContext eAndContext => new And(eAndContext),
                LatteParser.EFalseContext eFalseContext => new Bool(eFalseContext),
                LatteParser.EFunCallContext eFunCallContext => new FunCall(eFunCallContext),
                LatteParser.EIdContext eIdContext => new ID(eIdContext),
                LatteParser.EIntContext eIntContext => new Int(eIntContext),
                LatteParser.EMethodCallContext eMethodCallContext => new MethodCall(eMethodCallContext),
                LatteParser.EMulOpContext eMulOpContext => new MulOp(eMulOpContext),
                LatteParser.ENewObjectContext eNewObjectContext => new NewObject(eNewObjectContext),
                LatteParser.ENullContext _ => new Null(),
                LatteParser.ENullCastContext _ => new Null(),
                LatteParser.EObjectFieldContext eObjectFieldContext => new ObjectField(eObjectFieldContext),
                LatteParser.EOrContext eOrContext => new Or(eOrContext),
                LatteParser.EParenContext eParenContext => ExprFromExprContext(eParenContext.expr()),
                LatteParser.ERelOpContext eRelOpContext => new RelOp(eRelOpContext),
                LatteParser.EStrContext eStrContext => new Str(eStrContext),
                LatteParser.ETrueContext eTrueContext => new Bool(eTrueContext),
                LatteParser.EUnOpContext eUnOpContext => new UnOp(eUnOpContext),
                _ => throw new ArgumentOutOfRangeException(nameof(context))
            };
        }

        public static Expr DefaultValueForType(LatteParser.TypeContext type)
        {
            return type switch
            {
                LatteParser.TBoolContext boolContext => new Bool(),
                LatteParser.TIntContext intContext => new Int(),
                LatteParser.TStringContext stringContext => new Str(),
                LatteParser.TTypeNameContext typeNameContext => new Null {Type = typeNameContext},
                LatteParser.TVoidContext voidContext => throw new NotSupportedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }
    }
}