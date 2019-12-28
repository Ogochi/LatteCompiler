using Common.AST.Exprs;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class While : Stmt
    {
        public Expr Expr { get; set; }
        public Stmt Stmt { get; set; }

        public While(LatteParser.WhileContext context)
        {
            Expr = Exprs.Utils.ExprFromExprContext(context.expr());
            Stmt = Utils.StmtFromStmtContext(context.stmt());
        }
    }
}