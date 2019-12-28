using Common.AST.Exprs;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class CondElse : Stmt
    {
        public Expr Expr { get; set; }
        public Stmt TStmt { get; set; }
        public Stmt FStmt { get; set; }

        public CondElse(LatteParser.CondElseContext context)
        {
            Expr = Exprs.Utils.ExprFromExprContext(context.expr());
            TStmt = Utils.StmtFromStmtContext(context.stmt()[0]);
            FStmt = Utils.StmtFromStmtContext(context.stmt()[1]);
        }
    }
}