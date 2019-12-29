using Common.AST.Exprs;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class Ret : Stmt
    {
        public Expr Expr { get; set; }

        public Ret(LatteParser.VRetContext _)
        {
        }

        public Ret(LatteParser.RetContext context)
        {
            Expr = Exprs.Utils.ExprFromExprContext(context.expr());
        }
        
        public override void Accept(BaseAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}