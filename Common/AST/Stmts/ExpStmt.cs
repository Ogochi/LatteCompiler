using Common.AST.Exprs;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class ExpStmt : Stmt
    {
        public Expr Expr { get; set; }

        public ExpStmt(LatteParser.SExpContext context)
        {
            Expr = Exprs.Utils.ExprFromExprContext(context.expr());
        }
    }
}