using System;
using ParsingTools;

namespace Common.AST.Exprs
{
    public static class Utils
    {
        public static Expr ExprFromExprContext(LatteParser.ExprContext context)
        {
            return context switch
            {
                LatteParser.EAddOpContext eAddOpContext => new AddOp(eAddOpContext),
                LatteParser.EAndContext eAndContext => new And(eAndContext),
                LatteParser.EFalseContext eFalseContext => new Bool(eFalseContext),
                LatteParser.EFunCallContext eFunCallContext => new FunCall(eFunCallContext),
                LatteParser.EIdContext eIdContext => new ID(eIdContext),
                LatteParser.EIntContext eIntContext => new Int(eIntContext),
                LatteParser.EMethodCallContext eMethodCallContext => throw new NotImplementedException(),
                LatteParser.EMulOpContext eMulOpContext => new MulOp(eMulOpContext),
                LatteParser.ENewObjectContext eNewObjectContext => throw new NotImplementedException(),
                LatteParser.ENullContext eNullContext => throw new NotImplementedException(),
                LatteParser.EObjectFieldContext eObjectFieldContext => throw new NotImplementedException(),
                LatteParser.EOrContext eOrContext => new Or(eOrContext),
                LatteParser.EParenContext eParenContext => ExprFromExprContext(eParenContext.expr()),
                LatteParser.ERelOpContext eRelOpContext => new RelOp(eRelOpContext),
                LatteParser.EStrContext eStrContext => new Str(eStrContext),
                LatteParser.ETrueContext eTrueContext => new Bool(eTrueContext),
                LatteParser.EUnOpContext eUnOpContext => new UnOp(eUnOpContext),
                _ => throw new ArgumentOutOfRangeException(nameof(context))
            };
        }
    }
}