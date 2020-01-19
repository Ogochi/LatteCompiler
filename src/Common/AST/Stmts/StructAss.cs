using Common.AST.Exprs;
using ParsingTools;

namespace Common.AST.Stmts
{
    public class StructAss : Stmt
    {
        public Expr IdExpr { get; set; }
        
        public Expr Expr { get; set; }
        
        public string Id { get; set; }

        public StructAss(LatteParser.StructAssContext context)
        {
            IdExpr = Exprs.Utils.ExprFromExprContext(context.expr()[0]);
            Expr = Exprs.Utils.ExprFromExprContext(context.expr()[1]);
            Id = context.ID().GetText();
        }

        public override void Accept(BaseAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}