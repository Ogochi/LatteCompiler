using Common.AST.Exprs;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class Cond : Stmt
    {
        public Expr Expr { get; set; }
        public Stmt Stmt { get; set; }

        public Cond(LatteParser.CondContext context)
        {
            Expr = Exprs.Utils.ExprFromExprContext(context.expr());
            Stmt = Utils.StmtFromStmtContext(context.stmt());
        }
    }
}