using ParsingTools;

namespace Common.AST.Exprs
{
    public class UnOp : Expr
    {
        public Unary Operator { get; set; }
        public Expr Expr { get; set; }

        public UnOp(LatteParser.EUnOpContext context)
        {
            Operator = context.unOp() switch
            {
                LatteParser.UnaryMinusContext _ => Unary.Minus,
                LatteParser.UnaryNegContext _ => Unary.Neg
            };

            Expr = Utils.ExprFromExprContext(context.expr());
        }
    }
}