using Common.AST.Exprs;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class Ass : Stmt
    {
        public string Id { get; set; }
        public Expr Expr { get; set; }

        public Ass(LatteParser.AssContext context)
        {
            Id = context.ID().GetText();
            Expr = Exprs.Utils.ExprFromExprContext(context.expr());
        }

        public Ass(string id, Expr expr)
        {
            Id = id;
            Expr = expr;
        }

        public override void Accept(BaseAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}