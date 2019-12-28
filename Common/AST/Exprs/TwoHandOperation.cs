using ParsingTools;

namespace Common.AST.Exprs
{
    public class TwoHandOperation : Expr
    {
        public Expr Lhs { get; set; }
        public Expr Rhs { get; set; }

        public TwoHandOperation(LatteParser.ExprContext e1, LatteParser.ExprContext e2)
        {
            Lhs = Utils.ExprFromExprContext(e1);
            Rhs = Utils.ExprFromExprContext(e2);
        }
    }
}